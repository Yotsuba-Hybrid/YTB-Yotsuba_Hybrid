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
    public class Hardware_Occlusion_Querie_Runtime : YTB_Runtime
    {
        private Render_Prediction_Runtime_3D RenderPredictionRuntime3D;

        private Graphics3D Graphics3D;

        private static readonly BlendState NoColorWriteBlendState = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.None
        };

        private YTB<int> EntityToReturn { get; set; } 
        private bool OcclusionQueriesOperational;
        private bool OcclusionQueriesForcedFallback;
        private int LastSubmittedQueries;
        private int LastCompletedQueries;
        private int LastVisibleFromQuery;
        private int LastConservativeFallback;

        public override void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
            Entities = new();
            EntityToReturn = new YTB<int>();
            RenderPredictionRuntime3D = new();
            Graphics3D = new();
            RenderPredictionRuntime3D.InitializeSystem(entities);
            OcclusionQueriesOperational = true;
            OcclusionQueriesForcedFallback = false;
        }

        /// <summary>
        /// Evalúa resultados de occlusion queries previamente emitidas y programa nuevas queries para el frame siguiente.
        /// Contrato: la fase de pre-pass de profundidad debe haber ocurrido antes de esta llamada en el mismo frame.
        /// Si el estado de queries no es válido, hace fallback conservador determinista (sin ocultar por query).
        /// </summary>
        private YTB<int> CalculateVisibility()
        {
            CameraComponent3D camera = EntityManager.Camera;
            YTB<int> visibility = EntityToReturn;
            EntityToReturn.Clear();
            if (camera is null) return visibility;

            var gd = YTBGlobalState.GraphicsDevice;
            Span<Yotsuba> GlobalEntities = GetEntitiesAsSpan();
            Span<TransformComponent> transformComponents = GetTransformComponentsAsSpan();
            Span<ModelComponent3D> modelComponents = GetModelsComponentsAsSpan();

            BoundingFrustum cameraFrustum = new BoundingFrustum(camera.ViewMatrix * camera.ProjectionMatrix);
            int submittedQueries = 0;
            int completedQueries = 0;
            int visibleByQuery = 0;
            int conservativeFallback = 0;

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
                    conservativeFallback++;
                    continue;
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
                    if (!OcclusionQueriesOperational || OcclusionQueriesForcedFallback)
                    {
                        visibility.Add(entityId);
                        model.IsOccluded = false;
                        model.IsQueryActive = false;
                        conservativeFallback++;
                        continue;
                    }

                    // Inicializar el query si es nuevo
                    if (model.OcclusionQuery == null)
                    {
                        try
                        {
                            model.OcclusionQuery = new OcclusionQuery(gd);
                            model.IsOccluded = false;
                            model.IsQueryActive = false;
                        }
                        catch
                        {
                            OcclusionQueriesOperational = false;
                            OcclusionQueriesForcedFallback = true;
                            visibility.Add(entityId);
                            model.IsOccluded = false;
                            model.IsQueryActive = false;
                            conservativeFallback++;
                            continue;
                        }
                    }

                    // 3. LEER RESPUESTA DEL FRAME ANTERIOR (Sin congelar CPU)
                    if (model.IsQueryActive && model.OcclusionQuery.IsComplete)
                    {
                        model.IsOccluded = (model.OcclusionQuery.PixelCount == 0);
                        model.IsQueryActive = false;
                        completedQueries++;
                    }

                    // 4. AÑADIR A LISTA DE VISIBLES
                    if (!model.IsOccluded)
                    {
                        visibility.Add(entityId);
                        visibleByQuery++;
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
                        submittedQueries++;
                    }
                }
            }

            // Restaurar estados originales de la GPU
            gd.BlendState = oldBlendState;
            gd.DepthStencilState = oldDepthStencil;
            LastSubmittedQueries = submittedQueries;
            LastCompletedQueries = completedQueries;
            LastVisibleFromQuery = visibleByQuery;
            LastConservativeFallback = conservativeFallback;

            return visibility;
        }

        public Span<int> GetEntitiesToRender()
        {
            Span<int> entities = CalculateVisibility().AsSpan();
            return entities;
        }

        public (bool QueriesOperational, bool ForcedFallback, int Submitted, int Completed, int VisibleFromQuery, int ConservativeFallback) GetDiagnostics()
            => (OcclusionQueriesOperational, OcclusionQueriesForcedFallback, LastSubmittedQueries, LastCompletedQueries, LastVisibleFromQuery, LastConservativeFallback);

        public override void Dispose()
        {
            RenderPredictionRuntime3D.Dispose();
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }
}
