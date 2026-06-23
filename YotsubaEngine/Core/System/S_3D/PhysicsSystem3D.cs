using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events;
using YotsubaEngine.HighestPerformanceTypes;
using YotsubaEngine.Physics.RigidBody;
using YotsubaEngine.Runtime.CPR;

namespace YotsubaEngine.Core.System.S_3D
{
    /// <summary>
    /// Sistema central de Físicas 3D del Yotsuba Engine.
    /// Se encarga de mover entidades, predecir colisiones (Broad Phase), 
    /// calcular intersecciones precisas (Narrow Phase) y resolver los impactos o triggers.
    /// </summary>
    public class PhysicsSystem3D : ISystem
    {
        // Runtime encargado de dividir el mundo en una cuadrícula y evitar comparar todas las entidades contra todas (Broad Phase)
        private Collision_Prediction_Runtime_3D Collision_Prediction_Runtime;

        // Buffer reutilizable para almacenar los posibles choques de una entidad y no generar basura en la RAM (Zero Allocation)
        private YTB<int> _potentialColliders = new();

        public override void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
            Collision_Prediction_Runtime = new();
            Collision_Prediction_Runtime.InitializeSystem(entities);
            base.InitializeSystem(entities);
        }

        public override void UpdateSystem(GameTime gameTime)
        {
            // Obtenemos todos los componentes usando Spans para máxima velocidad en la CPU (Data-Oriented Design)
            Span<Yotsuba> GlobalEntities = GetEntitiesAsSpan();
            Span<int> EntitiesWithPhisics3D = Collision_Prediction_Runtime.Entities.AsSpan();
            Span<TransformComponent> transformComponents = GetTransformComponentsAsSpan();
            Span<RigidBodyComponent3D> rigidBodyComponents3D = GetRigidBody3DComponentsAsSpan();
            Span<ModelComponent3D> modelComponent3Ds = GetModelsComponentsAsSpan();
            Span<YTBModelComponent3D> YTBSpecialGeometricObject3D = GetYTBModelComponentsAsSpan();
            Span<SpriteComponent2D> spriteComponents = GetSpriteComponentsAsSpan();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // =========================================================
            // 1. INTEGRACIÓN DE MOVIMIENTO (Aplicar Velocidad)
            // =========================================================
            foreach (ref int entityId in EntitiesWithPhisics3D)
            {
                ref TransformComponent transform = ref transformComponents[entityId];
                ref RigidBodyComponent3D rigidBody = ref rigidBodyComponents3D[entityId];

                // Movimiento Euleriano: Nueva Posición = Posición Actual + (Velocidad * Tiempo)
                // IMPORTANTE: Todos los objetos se mueven si tienen velocidad, incluyendo los "NoCollision" (Triggers).
                transform.Position += rigidBody.Velocity * dt;
            }

            // =========================================================
            // 2. CICLO PRINCIPAL DE COLISIONES
            // =========================================================
            foreach (ref int entityId in EntitiesWithPhisics3D)
            {
                ref Yotsuba entity = ref GlobalEntities[entityId];
                ref TransformComponent transform = ref transformComponents[entityId];

                // BROAD PHASE: Le preguntamos a la cuadrícula espacial con qué entidades cercanas podríamos chocar
                _potentialColliders = Collision_Prediction_Runtime.IsPhysicalPossibleCollide(ref transform, entityId, _potentialColliders);

                // Si no hay nadie cerca, saltamos a la siguiente entidad inmediatamente (Gran ahorro de CPU)
                if (_potentialColliders.Count == 0) continue;

                Span<int> ents = _potentialColliders.AsSpan();

                foreach (ref int otherCollideId in ents)
                {
                    // Evitamos que la entidad choque consigo misma o procesar colisiones duplicadas (A vs B y B vs A)
                    if (otherCollideId <= entityId) continue;

                    // Pasamos a la fase de precisión matemática (Narrow Phase)
                    ApplyPhysics(
                        entity.Id,
                        otherCollideId,
                        GlobalEntities,
                        transformComponents,
                        rigidBodyComponents3D,
                        modelComponent3Ds
                    );
                }
            }
        }

        private void ApplyPhysics(
            int entityIdA,
            int entityIdB,
            Span<Yotsuba> GlobalEntities,
            Span<TransformComponent> transformComponents,
            Span<RigidBodyComponent3D> rigidBodyComponents3D,
            Span<ModelComponent3D> modelComponent3Ds
            )
        {
            ref RigidBodyComponent3D rigidA = ref rigidBodyComponents3D[entityIdA];
            ref RigidBodyComponent3D rigidB = ref rigidBodyComponents3D[entityIdB];

            // =========================================================
            // FILTRO DE CAPAS (Bitmasking)
            // =========================================================
            // La operación lógica AND (&) revisa los bits. Si el resultado es 0, significa que no comparten ninguna capa.
            if ((rigidA.CollisionLayer & rigidB.CollisionLayer) == 0) return;

            PhysicType typeA = CheckType(ref GlobalEntities[entityIdA]);
            PhysicType typeB = CheckType(ref GlobalEntities[entityIdB]);

            if (typeA == PhysicType.None || typeB == PhysicType.None) return;

            // =========================================================
            // PATRÓN DE ORDEN CANÓNICO (Simplificación de la Arquitectura)
            // =========================================================
            // Forzamos a que la Entidad A siempre sea el tipo de geometría más compleja (Ej. Model).
            // Esto evita tener que escribir código duplicado para el "viceversa".
            if (typeB > typeA)
            {
                (entityIdA, entityIdB) = (entityIdB, entityIdA);
                (typeA, typeB) = (typeB, typeA);
                // Como intercambiamos los IDs, actualizamos las referencias de los cuerpos rígidos
                rigidA = ref rigidBodyComponents3D[entityIdA];
                rigidB = ref rigidBodyComponents3D[entityIdB];
            }

            ref TransformComponent transformA = ref transformComponents[entityIdA];
            ref TransformComponent transformB = ref transformComponents[entityIdB];

            bool exactCollisionDetected = false;
            Vector3 collisionNormal = Vector3.Zero;  // Hacia dónde empujar (El ángulo del impacto)
            float penetrationDepth = 0f;             // Qué tan adentro se metió un objeto en el otro
            string hitPartA = null;                  // Para saber si golpeó el brazo, la cabeza, etc.
            string hitPartB = null;

            // =========================================================
            // EMBUDO DE FASES DE COLISIÓN (NARROW PHASE)
            // =========================================================

            if (typeA.HasFlag(PhysicType.Model))
            {
                ref ModelComponent3D modelA = ref modelComponent3Ds[entityIdA];
                Matrix matrixA = CreateWorldMatrix(ref transformA, ref rigidA);

                // Esfera global que envuelve todo el modelo A
                BoundingSphere sphereA = modelA.GetWorldBoundingSphere(matrixA);

                if (typeB.HasFlag(PhysicType.Model))
                {
                    // ---------------------------------------------
                    // ESCENARIO 1: MODEL vs MODEL
                    // ---------------------------------------------
                    ref ModelComponent3D modelB = ref modelComponent3Ds[entityIdB];
                    Matrix matrixB = CreateWorldMatrix(ref transformB, ref rigidB);
                    BoundingSphere sphereB = modelB.GetWorldBoundingSphere(matrixB);

                    // FASE 1: Choque de esferas (Descarte rápido). Si se rozan, generamos cajas y evaluamos FASE 2.
                    if (sphereA.Intersects(sphereB) && BoundingBox.CreateFromSphere(sphereA).Intersects(BoundingBox.CreateFromSphere(sphereB)))
                    {
                        // FASE 3: Choque exacto ("Sin aire"). Comprobamos las sub-esferas de los huesos de la Entidad A...
                        foreach (ModelMesh meshA in modelA.Model.Meshes)
                        {
                            BoundingSphere exactPartA = meshA.BoundingSphere.Transform(modelA.BoneTransforms[meshA.ParentBone.Index] * matrixA);

                                // contra las sub-esferas de los huesos de la Entidad B.
                            foreach (ModelMesh meshB in modelB.Model.Meshes)
                            {
                                BoundingSphere exactPartB = meshB.BoundingSphere.Transform(modelB.BoneTransforms[meshB.ParentBone.Index] * matrixB);
                                if (exactPartA.Intersects(exactPartB))
                                {

                                    //if (CheckExactGeometry(meshA, ref matrixA, meshB, ref matrixB, out collisionNormal, out penetrationDepth))
                                    {
                                        CalculateSpherePenetration(exactPartA, exactPartB, out collisionNormal, out penetrationDepth);
                                        exactCollisionDetected = penetrationDepth > 0f;
                                        hitPartA = meshA.Name; // Registramos exactamente qué malla chocó
                                        hitPartB = meshB.Name;
                                        break;
                                    }
                                }
                            }
                            if (exactCollisionDetected) break; // Si ya detectamos choque, dejamos de iterar huesos
                        }
                    }
                }
                else
                {
                    // ---------------------------------------------
                    // ESCENARIO 2: MODEL vs CAJA SÓLIDA (YTBModel o Sprite 2.5D)
                    // ---------------------------------------------
                    // Obtenemos la caja rectangular del Sprite/YTBModel incluyendo el OffSet
                    BoundingBox boxB = GetBoundingBox(ref transformB, ref rigidB);

                    // FASE 1 y 2: Convertimos la esfera principal del modelo en caja y cruzamos contra la caja B
                    if (BoundingBox.CreateFromSphere(sphereA).Intersects(boxB))
                    {
                        // FASE 3: Los huesos individuales del modelo contra la Caja Sólida B
                        foreach (ModelMesh meshA in modelA.Model.Meshes)
                        {
                            BoundingSphere exactPartA = meshA.BoundingSphere.Transform(modelA.BoneTransforms[meshA.ParentBone.Index] * matrixA);
                            if (boxB.Intersects(exactPartA))
                            {
                                CalculateSphereVsBoxPenetration(exactPartA, boxB, out collisionNormal, out penetrationDepth);
                                exactCollisionDetected = true;
                                hitPartA = meshA.Name;
                                hitPartB = typeB.HasFlag(PhysicType.YTBModel) ? "YTB_Box" : "Sprite_Box";
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // ---------------------------------------------
                // ESCENARIO 3: CAJA vs CAJA (Cualquier combinación que no involucre un modelo 3D complejo)
                // ---------------------------------------------
                BoundingBox boxA = GetBoundingBox(ref transformA, ref rigidA);
                BoundingBox boxB = GetBoundingBox(ref transformB, ref rigidB);

                // Comprobación directa de intersección (AABB vs AABB)
                if (boxA.Intersects(boxB))
                {
                    CalculateBoxVsBoxPenetration(boxA, boxB, out collisionNormal, out penetrationDepth);
                    exactCollisionDetected = true;
                    hitPartA = typeA.HasFlag(PhysicType.YTBModel) ? "YTB_Box" : "Sprite_Box";
                    hitPartB = typeB.HasFlag(PhysicType.YTBModel) ? "YTB_Box" : "Sprite_Box";
                }
            }

            // =========================================================
            // RESOLUCIÓN DE COLISIONES Y EVENTOS
            // =========================================================
            if (exactCollisionDetected && penetrationDepth > 0)
            {
                // Disparamos SIEMPRE el evento para notificar al gameplay que hubo contacto
                EventManager.Instance.Publish(new OnEntity3DCollide(entityIdA, entityIdB, collisionNormal, penetrationDepth, hitPartA, hitPartB));

                // LÓGICA DE TRIGGER (Trigger):
                // Si ambos objetos son Solid (Sólidos), se empujan mutuamente.
                // Si ALGUNO de los dos es Trigger, se tratan como fantasmas/triggers, pueden atravesarse y NO llamamos a ResolveCollision.
                if (rigidA.Collide == CollisionLevel.Solid && rigidB.Collide == CollisionLevel.Solid)
                {
                    ResolveCollision(ref transformA, ref rigidA, ref transformB, ref rigidB, collisionNormal, penetrationDepth);
                }
            }
        }

        //private bool CheckExactGeometry(ModelMesh meshA, ref Matrix matrixA, ModelMesh meshB, ref Matrix matrixB, out Vector3 collisionNormal, out float penetrationDepth)
        //{
        //    bool exactCollisionDetected;

        //    VertexPositionNormalTexture[] positionNormalTextureA;
        //    VertexPositionNormalTexture[] positionNormalTextureB;

        //    for (int iA = 0; iA < meshA.MeshParts.Count; iA++)
        //    {

        //        ModelMeshPart part = meshA.MeshParts[iA];
        //        positionNormalTextureA = new VertexPositionNormalTexture[part.NumVertices];
        //        int stride = part.VertexBuffer.VertexDeclaration.VertexStride;
        //        part.VertexBuffer.GetData(part.VertexOffset * stride, positionNormalTextureA, 0, part.NumVertices, stride);

        //        ushort[] indices = new ushort[part.PrimitiveCount * 3];
        //        part.IndexBuffer.GetData(part.StartIndex * 2, indices, 0, part.PrimitiveCount * 3);


        //        for (int i = 0; i < indices.Length; i+= 3)
        //        {
        //            ushort vertexA = indices[i];
        //            ushort vertexB = indices[i + 1];
        //            ushort vertexC = indices[i + 2];

        //            Vector3 vA = Vector3.Transform(positionNormalTextureA[vertexA].Position, matrixA);
        //            Vector3 vB = Vector3.Transform(positionNormalTextureA[vertexB].Position, matrixA);
        //            Vector3 vC = Vector3.Transform(positionNormalTextureA[vertexC].Position, matrixA);


        //            for (int iB = 0; iB < meshB.MeshParts.Count; iB+= 3)
        //            {
        //                var partB = meshB.MeshParts[iB];
        //                positionNormalTextureB = new VertexPositionNormalTexture[partB.NumVertices];
        //                int strideB = partB.VertexBuffer.VertexDeclaration.VertexStride;
        //                partB.VertexBuffer.GetData(partB.VertexOffset * strideB, positionNormalTextureB, 0, partB.NumVertices, strideB);

        //                ushort[] indicesb = new ushort[partB.PrimitiveCount * 3];
        //                partB.IndexBuffer.GetData(partB.StartIndex * 2, indicesb, 0, partB.PrimitiveCount * 3);
        //                ushort vertexA_b = indicesb[iB];
        //                ushort vertexB_b = indicesb[iB + 1];
        //                ushort vertexC_b = indicesb[iB + 2];

                        

        //                Vector3 vA_b = Vector3.Transform(positionNormalTextureB[vertexA_b].Position, matrixB);
        //                Vector3 vB_b = Vector3.Transform(positionNormalTextureB[vertexB_b].Position, matrixB);
        //                Vector3 vC_b = Vector3.Transform(positionNormalTextureB[vertexC_b].Position, matrixB);

        //                if (TriangleIntersect(ref vA, ref vB, ref vC, ref vA_b, ref vB_b, ref vC_b, out collisionNormal, out penetrationDepth))
        //                {
        //                    exactCollisionDetected = true;
        //                    break;
        //                }
        //            }

        //        }

        //    }

        //    return exactCollisionDetected;
        //}

        // =========================================================
        // FUNCIONES DE MATEMÁTICA Y RESOLUCIÓN FÍSICA
        // =========================================================

        /// <summary>
        /// Separa dos entidades sólidas que se han penetrado y cancela la velocidad en el eje del impacto.
        /// (Solo se ejecuta si ambas entidades son CollisionLevel.Solid)
        /// Distribución de fuerza proporcional a la masa (objetos ligeros reciben más impacto).
        /// </summary>
        private void ResolveCollision(ref TransformComponent tA, ref RigidBodyComponent3D rbA, ref TransformComponent tB, ref RigidBodyComponent3D rbB, Vector3 normal, float penetration)
        {
            // Calcular distribución de fuerza basada en masa física (no comunista 50/50).
            // Objetos más pesados reciben MENOS fuerza, objetos más ligeros reciben MÁS.
            float massA = rbA.Mass;
            float massB = rbB.Mass;

            // Evitar división por cero
            float totalMass = massA + massB;
            if (totalMass == 0) totalMass = 1f;

            // Ratio inverso: objeto MÁS PESADO recibe MENOS impacto
            // Ejemplo: A(4kg) vs B(1kg) → A recibe 20%, B recibe 80%
            float ratioA = massB / totalMass; // Objeto A recibe % de masa B
            float ratioB = massA / totalMass; // Objeto B recibe % de masa A

            // 1. Separar objetos proporcionalmente a sus masas
            tA.Position += normal * (penetration * ratioA);
            tB.Position -= normal * (penetration * ratioB);

            // 2. Fricción vectorial (Producto Punto).
            // Averiguamos cuánta de la velocidad de A va en dirección directa hacia B
            float impactSpeedA = Vector3.Dot(rbA.Velocity, normal);
            if (impactSpeedA < 0)
            {
                // Cancelamos SOLAMENTE la velocidad que va contra el obstáculo, dejando que pueda deslizarse a los lados
                rbA.Velocity -= normal * impactSpeedA;
            }

            // Repetimos lo mismo para B, pero invertimos la normal porque el choque es desde la perspectiva contraria
            float impactSpeedB = Vector3.Dot(rbB.Velocity, -normal);
            if (impactSpeedB < 0)
            {
                rbB.Velocity -= -normal * impactSpeedB;
            }
        }

        /// <summary>
        /// Calcula la penetración entre dos formas circulares 3D
        /// </summary>
        private void CalculateSpherePenetration(BoundingSphere a, BoundingSphere b, out Vector3 normal, out float depth)
        {
            Vector3 dir = a.Center - b.Center;
            float dist = dir.Length();
            if (dist > 0)
            {
                normal = dir / dist; // Vector direccional unitario del impacto
                depth = (a.Radius + b.Radius) - dist; // La cantidad de espacio que se superponen
            }
            else
            {
                // Edge-case: Están exactamente en el mismo pixel (Centro perfecto)
                normal = Vector3.Up;
                depth = a.Radius + b.Radius;
            }
        }

        /// <summary>
        /// Calcula la penetración entre un círculo(esfera) y un rectángulo sólido(caja).
        /// Ideal para disparos (esfera) contra un muro (caja).
        /// </summary>
        private void CalculateSphereVsBoxPenetration(BoundingSphere sphere, BoundingBox box, out Vector3 normal, out float depth)
        {
            // Usamos Clamp (Límites) para encontrar exactamente en qué punto de la superficie de la caja tocó la esfera
            Vector3 closestPoint = new Vector3(
                MathHelper.Clamp(sphere.Center.X, box.Min.X, box.Max.X),
                MathHelper.Clamp(sphere.Center.Y, box.Min.Y, box.Max.Y),
                MathHelper.Clamp(sphere.Center.Z, box.Min.Z, box.Max.Z)
            );

            Vector3 dir = sphere.Center - closestPoint;
            float dist = dir.Length();
            if (dist > 0)
            {
                normal = dir / dist;
                depth = sphere.Radius - dist;
            }
            else
            {
                // El centro de la esfera está dentro de la caja: elegimos eje mínimo de salida.
                float toMinX = sphere.Center.X - box.Min.X;
                float toMaxX = box.Max.X - sphere.Center.X;
                float toMinY = sphere.Center.Y - box.Min.Y;
                float toMaxY = box.Max.Y - sphere.Center.Y;
                float toMinZ = sphere.Center.Z - box.Min.Z;
                float toMaxZ = box.Max.Z - sphere.Center.Z;

                float minDistance = toMinX;
                normal = new Vector3(-1f, 0f, 0f);

                if (toMaxX < minDistance) { minDistance = toMaxX; normal = new Vector3(1f, 0f, 0f); }
                if (toMinY < minDistance) { minDistance = toMinY; normal = new Vector3(0f, -1f, 0f); }
                if (toMaxY < minDistance) { minDistance = toMaxY; normal = new Vector3(0f, 1f, 0f); }
                if (toMinZ < minDistance) { minDistance = toMinZ; normal = new Vector3(0f, 0f, -1f); }
                if (toMaxZ < minDistance) { minDistance = toMaxZ; normal = new Vector3(0f, 0f, 1f); }

                depth = sphere.Radius + Math.Max(0f, minDistance);
            }
        }

        /// <summary>
        /// Calcula el impacto entre dos bloques rectangulares (AABB).
        /// </summary>
        private void CalculateBoxVsBoxPenetration(BoundingBox a, BoundingBox b, out Vector3 normal, out float depth)
        {
            Vector3 centerA = (a.Min + a.Max) / 2f;
            Vector3 centerB = (b.Min + b.Max) / 2f;
            Vector3 extentsA = (a.Max - a.Min) / 2f;
            Vector3 extentsB = (b.Max - b.Min) / 2f;

            Vector3 delta = centerA - centerB;

            // Calculamos cuánto se hunden en el eje X, Y y Z
            float overlapX = extentsA.X + extentsB.X - Math.Abs(delta.X);
            float overlapY = extentsA.Y + extentsB.Y - Math.Abs(delta.Y);
            float overlapZ = extentsA.Z + extentsB.Z - Math.Abs(delta.Z);

            // La penetración real y la normal del impacto se determinan por el eje donde el hundimiento fue MENOR
            depth = Math.Min(overlapX, Math.Min(overlapY, overlapZ));

            if (depth == overlapX) normal = new Vector3(delta.X >= 0 ? 1f : -1f, 0f, 0f);
            else if (depth == overlapY) normal = new Vector3(0f, delta.Y >= 0 ? 1f : -1f, 0f);
            else normal = new Vector3(0f, 0f, delta.Z >= 0 ? 1f : -1f);
        }

        /// <summary>
        /// Genera una caja delimitadora exacta sumando el OffSetCollision definido por el usuario, 
        /// evitando que las físicas queden desfasadas del sprite/modelo visual.
        /// </summary>
        private BoundingBox GetBoundingBox(ref TransformComponent t, ref RigidBodyComponent3D rb)
        {
            Vector3 center = t.Position + rb.OffSetCollision;
            Vector3 half = t.Size / 2f;
            return new BoundingBox(center - half, center + half); // Retorna desde la esquina inferior trasera hasta la superior delantera
        }

        /// <summary>
        /// Crea la Matriz de Mundo S-R-T (Scale, Rotation, Translation) aplicando el offset físico
        /// </summary>
        public static Matrix CreateWorldMatrix(ref TransformComponent transform, ref RigidBodyComponent3D rigidBody)
        {
            Matrix scale = Matrix.CreateScale(transform.Scale);
            Matrix rotation = Matrix.CreateRotationY(transform.Rotation);
            Vector3 finalPosition = transform.Position + rigidBody.OffSetCollision;
            Matrix translation = Matrix.CreateTranslation(finalPosition);

            return scale * rotation * translation;
        }

        public override void Dispose()
        {
            Collision_Prediction_Runtime.Dispose();
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        /// <summary>
        /// Lee los componentes de la entidad para decidir bajo qué reglas geométricas debe ser tratada en las físicas.
        /// </summary>
        private PhysicType CheckType(ref Yotsuba entity)
        {
            PhysicType result = PhysicType.None;
            if (entity.HasComponent(YTBComponent.Sprite)) result |= PhysicType.Sprite2_5D;
            if (entity.HasComponent(YTBComponent.YTBModel3D)) result |= PhysicType.YTBModel;
            if (entity.HasComponent(YTBComponent.Model3D)) result |= PhysicType.Model;
            return result; // Usamos banderas (|) porque una entidad podría teóricamente tener más de un componente visual
        }

        [Flags]
        public enum PhysicType : byte
        {
            None = 0,
            Sprite2_5D = 1 << 0, // Geometría Tipo 1 (La más simple)
            YTBModel = 1 << 1,   // Geometría Tipo 2 
            Model = 1 << 2       // Geometría Tipo 4 (La más compleja, tiene huesos)
        }
    }
}