using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Text;
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
        public readonly struct OcclusionInstrumentationFrame
        {
            public int FrameIndex { get; init; }
            public int RprPredictedCount { get; init; }
            public int OcrVisibleCount { get; init; }
            public int OcrOccludedCount { get; init; }
            public int OcclusionTransitionsCount { get; init; }
            public int PendingQueriesCount { get; init; }
            public string ToLogLine() => $"[OCR][Frame {FrameIndex}] RPR={RprPredictedCount} OCR Visible={OcrVisibleCount} Occluded={OcrOccludedCount} PendingQueries={PendingQueriesCount} IsOccludedTransitions={OcclusionTransitionsCount}";
        }

        public static int DebugVisibleCount { get; private set; }
        public static int DebugOccludedCount { get; private set; }
        public static int DebugActiveQueriesCount { get; private set; }
        public static int DebugCompletedQueriesCount { get; private set; }

        public bool UseOcrCulling { get; set; } = false;
        public bool InstrumentationEnabled { get; set; }
        public OcclusionInstrumentationFrame LastFrameInstrumentation { get; private set; }

        private bool OcclusionQueriesOperational = true;
        private bool OcclusionQueriesForcedFallback = false;
        private int LastSubmittedQueries;
        private int LastCompletedQueries;
        private int LastVisibleFromQuery;
        private int LastConservativeFallback;
        private int _frameIndex;
        private readonly StringBuilder _transitionLogBuilder = new();

        private const int OcclusionHideConfirmationFrames = 3;
        private Render_Prediction_Runtime_3D RenderPredictionRuntime3D;
        private Graphics3D Graphics3D;
        private YTB<int> EntityToReturn;

        private static readonly BlendState NoColorWriteBlendState = new() { ColorWriteChannels = ColorWriteChannels.None };

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

        private YTB<int> CalculateVisibility()
        {
            YTB<int> visibility = EntityToReturn;
            visibility.Clear();

            CameraComponent3D camera = EntityManager.Camera;
            Span<int> entitiesSpan = RenderPredictionRuntime3D.GetEntitieIdsCanRender3D();
            Span<Yotsuba> globalEntities = GetEntitiesAsSpan();
            Span<TransformComponent> transforms = GetTransformComponentsAsSpan();
            Span<ModelComponent3D> models = GetModelsComponentsAsSpan();

            int submittedQueries = 0, completedQueries = 0, visibleByQuery = 0, conservativeFallback = 0;
            int visibleCount = 0, occludedCount = 0, transitions = 0, pendingQueries = 0;
            DebugVisibleCount = 0; DebugOccludedCount = 0; DebugActiveQueriesCount = 0; DebugCompletedQueriesCount = 0;
            _frameIndex++; _transitionLogBuilder.Clear();

            if (camera == null)
            {
                for (int i = 0; i < entitiesSpan.Length; i++) visibility.Add(entitiesSpan[i]);
                return visibility;
            }

            BoundingFrustum frustum = new(camera.ViewMatrix * camera.ProjectionMatrix);
            var gd = YTBGlobalState.GraphicsDevice;
            var oldBlend = gd.BlendState;
            var oldDepth = gd.DepthStencilState;
            gd.BlendState = NoColorWriteBlendState;
            gd.DepthStencilState = DepthStencilState.Default;

            foreach (int entityId in entitiesSpan)
            {
                ref Yotsuba entity = ref globalEntities[entityId];
                if (entity.HasNotComponent(YTBComponent.Model3D)) { visibility.Add(entityId); continue; }

                ref TransformComponent transform = ref transforms[entityId];
                ref ModelComponent3D model = ref models[entityId];
                bool previousOccludedState = model.IsOccluded;

                if (!UseOcrCulling || !OcclusionQueriesOperational || OcclusionQueriesForcedFallback)
                {
                    visibility.Add(entityId);
                    model.IsOccluded = false; model.IsQueryActive = false; model.OccludedFrameStreak = 0; model.OcclusionUncertain = true;
                    conservativeFallback++; visibleCount++; DebugVisibleCount++;
                    continue;
                }

                Matrix world = Matrix.CreateScale(transform.Scale) * Matrix.CreateRotationY(transform.Rotation) * Matrix.CreateTranslation(transform.Position);
                BoundingSphere sphere = model.GetWorldBoundingSphere(world);
                if (!frustum.Intersects(sphere)) continue;

                if (model.IsQueryActive && model.OcclusionQuery != null && model.OcclusionQuery.IsComplete)
                {
                    bool occludedThisFrame = model.OcclusionQuery.PixelCount == 0;
                    if (occludedThisFrame)
                    {
                        model.OccludedFrameStreak++;
                        model.IsOccluded = model.OccludedFrameStreak >= OcclusionHideConfirmationFrames;
                    }
                    else
                    {
                        model.OccludedFrameStreak = 0;
                        model.IsOccluded = false;
                    }

                    model.OcclusionUncertain = false;
                    model.IsQueryActive = false;
                    completedQueries++; DebugCompletedQueriesCount++;
                }
                else if (model.IsQueryActive && model.OcclusionQuery == null)
                {
                    model.IsQueryActive = false;
                    model.IsOccluded = false;
                    model.OccludedFrameStreak = 0;
                    model.OcclusionUncertain = true;
                }
                else if (model.IsQueryActive)
                {
                    pendingQueries++;
                    model.OcclusionUncertain = true;
                }

                if (previousOccludedState != model.IsOccluded)
                {
                    transitions++;
                    _transitionLogBuilder.Append($" entityId={entityId}:{previousOccludedState}->{model.IsOccluded};");
                }

                if (model.OcclusionUncertain || !model.IsOccluded)
                {
                    visibleCount++; visibility.Add(entityId); DebugVisibleCount++;
                }
                else
                {
                    occludedCount++; DebugOccludedCount++; visibleByQuery++;
                }

                if (!model.IsQueryActive && model.OcclusionQuery != null)
                {
                    model.OcclusionQuery.Begin();
                    float boxSize = sphere.Radius * 2f;
                    Graphics3D.DrawBox(sphere.Center, new Vector3(boxSize), Color.Transparent, camera.ViewMatrix, camera.ProjectionMatrix);
                    model.OcclusionQuery.End();
                    model.IsQueryActive = true;
                    model.OcclusionUncertain = true;
                    submittedQueries++;
                }

                if (model.IsQueryActive) DebugActiveQueriesCount++;
            }

            gd.BlendState = oldBlend;
            gd.DepthStencilState = oldDepth;

            LastSubmittedQueries = submittedQueries;
            LastCompletedQueries = completedQueries;
            LastVisibleFromQuery = visibleByQuery;
            LastConservativeFallback = conservativeFallback;
            LastFrameInstrumentation = new OcclusionInstrumentationFrame { FrameIndex = _frameIndex, RprPredictedCount = entitiesSpan.Length, OcrVisibleCount = visibleCount, OcrOccludedCount = occludedCount, OcclusionTransitionsCount = transitions, PendingQueriesCount = pendingQueries };

            if (InstrumentationEnabled)
            {
                Debug.WriteLine(LastFrameInstrumentation.ToLogLine());
            }

            return visibility;
        }

        public Span<int> GetEntitiesToRender() => CalculateVisibility().AsSpan();

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
