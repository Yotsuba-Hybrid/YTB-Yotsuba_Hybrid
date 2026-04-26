using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Graphics;
using YotsubaEngine.HighestPerformanceTypes;
using YotsubaEngine.Runtime.RPR;

namespace YotsubaEngine.Runtime.OCR
{
    public class HardwareOcclusionQuerieRuntime : YTB_Runtime
    {
        private RenderPredictionRuntime3D RenderPredictionRuntime3D;

        private Graphics3D Graphics3D;

        private static readonly BlendState NoColorWriteBlendState = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.None
        };

        private YTB<int> EntityToReturn { get; set; } 
        public override void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
            Entities = new();
            RenderPredictionRuntime3D = new();
            Graphics3D = new();
            RenderPredictionRuntime3D.InitializeSystem(entities);
        }

        /// <summary>
        /// Realiza el Frustum Culling y el Occlusion Culling, devolviendo solo los IDs a dibujar.
        /// DEBE llamarse DESPUÉS de dibujar el escenario estático.
        /// </summary>
        private YTB<int> CalculateVisibility()
        {
            CameraComponent3D camera = EntityManager.Camera;
            YTB<int> visibility = EntityToReturn;
            EntityToReturn.Clear();

            var gd = YTBGlobalState.GraphicsDevice;
            Span<Yotsuba> GlobalEntities = GetEntitiesAsSpan();
            Span<TransformComponent> transformComponents = GetTransformComponentsAsSpan();
            Span<ModelComponent3D> modelComponents = GetModelsComponentsAsSpan();

            BoundingFrustum cameraFrustum = new BoundingFrustum(camera.ViewMatrix * camera.ProjectionMatrix);

            // Guardar estado actual
            var oldBlendState = gd.BlendState;
            var oldDepthStencil = gd.DepthStencilState;

            // PREPARAR GPU PARA OCLUSIÓN: 
            // - No pintar color
            // - SÍ leer y escribir en la profundidad (DepthBuffer)
            gd.BlendState = NoColorWriteBlendState;
            gd.DepthStencilState = DepthStencilState.Default;

            Span<int> entitiesSpan = RenderPredictionRuntime3D.GetEntitieIdsCanRender3D();
            foreach (int entityId in entitiesSpan)
            {
                ref Yotsuba entity = ref GlobalEntities[entityId];
                
                if(entity.HasNotComponent(YTBComponent.Model3D))
                {
                    visibility.Add(entityId);
                }

                ref TransformComponent transform = ref transformComponents[entityId];
                ref ModelComponent3D model = ref modelComponents[entityId];

                // 1. Matriz de Mundo Cacheada
                float yaw = MathHelper.ToRadians(transform.Rotation);
                Matrix worldMatrix = Matrix.CreateScale(transform.Scale)
                                   * Matrix.CreateRotationY(yaw)
                                   * Matrix.CreateTranslation(transform.Position);

                BoundingSphere entitySphere = model.GetWorldBoundingSphere(worldMatrix);

                // 2. FASE 1: FRUSTUM CULLING (CPU)
                if (cameraFrustum.Intersects(entitySphere))
                {
                    // Inicializar el query si es nuevo
                    if (model.OcclusionQuery == null)
                    {
                        model.OcclusionQuery = new OcclusionQuery(gd);
                        model.IsOccluded = false;
                        model.IsQueryActive = false;
                    }

                    // 3. LEER RESPUESTA DEL FRAME ANTERIOR (Sin congelar CPU)
                    if (model.IsQueryActive && model.OcclusionQuery.IsComplete)
                    {
                        model.IsOccluded = (model.OcclusionQuery.PixelCount == 0);
                        model.IsQueryActive = false;
                    }

                    // 4. AÑADIR A LISTA DE VISIBLES
                    if (!model.IsOccluded)
                    {
                        visibility.Add(entityId);
                    }

                    // 5. INICIAR NUEVA PRUEBA (Dibujar caja invisible a la GPU)
                    if (!model.IsQueryActive)
                    {
                        model.OcclusionQuery.Begin();

                        float boxSize = entitySphere.Radius * 2f;
                        Vector3 boxScale = new Vector3(boxSize);

                        // Dibujamos la caja que chocará con los muros del Z-Buffer
                        Graphics3D.DrawBox(entitySphere.Center, boxScale, Color.Transparent, camera.ViewMatrix, camera.ProjectionMatrix);

                        model.OcclusionQuery.End();

                        model.IsQueryActive = true;
                        model.IsOccluded = false; // Prevención de popping
                    }
                }
            }

            // Restaurar estados originales de la GPU
            gd.BlendState = oldBlendState;
            gd.DepthStencilState = oldDepthStencil;

            return visibility;
        }

        public Span<int> GetEntitiesToRender()
        {
            Span<int> entities = CalculateVisibility().AsSpan();
            return entities;
        }

        public override void Dispose()
        {
            RenderPredictionRuntime3D.Dispose();
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }
}
