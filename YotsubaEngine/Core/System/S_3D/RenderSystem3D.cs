using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
#if YTB
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;
#endif
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Graphics;
using YotsubaEngine.Runtime.OCR;

namespace YotsubaEngine.Core.System.S_3D
{
    public class RenderSystem3D : IRenderSystem
    {
        private int _frameCounter;
        private Graphics3D Graphics3D;
        private Hardware_Occlusion_Querie_Runtime HardwareOcclusionQuerieRuntime;

        public void SetOcclusionCullingEnabled(bool enabled) => HardwareOcclusionQuerieRuntime.UseOcrCulling = enabled;

        public override void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
            HardwareOcclusionQuerieRuntime = new();
            HardwareOcclusionQuerieRuntime.InitializeSystem(entities);
            HardwareOcclusionQuerieRuntime.UseOcrCulling = false;
            Graphics3D = new();
        }

        public override void Render3D(GameTime gameTime)
        {
            _frameCounter++;
            Span<Yotsuba> yotsubas = GetEntitiesAsSpan();
            Span<ModelComponent3D> models = EntityManager.ModelComponents3D.AsSpan();
            Span<TransformComponent> transforms = EntityManager.TransformComponents.AsSpan();
            Span<YTBModelComponent3D> ytb3D = EntityManager.YtbModelComponents.AsSpan();
            Span<ShaderComponent> shaders = EntityManager.ShaderComponents.AsSpan();
            Span<SpriteComponent2D> sprites = EntityManager.Sprite2DComponents.AsSpan();

            if (models.Length == 0 && ytb3D.Length == 0) return;

            CameraComponent3D camera = EntityManager.Camera;
            if (camera == null)
            {
#if YTB
                EngineUISystem.SendWarning($"{nameof(RenderSystem3D)}: cámara nula, se omite render 3D de forma segura.");
#endif
                return;
            }

            camera.Update();

            bool depthPrePassExecuted = false;
            bool queryPhaseExecuted = false;
            bool mainPassExecuted = false;
            int mainRendered = 0;

            HardwareOcclusionQuerieRuntime.UseOcrCulling = false;
            Span<int> entities = HardwareOcclusionQuerieRuntime.GetEntitiesToRender();
            queryPhaseExecuted = true;

            foreach (ref int entityId in entities)
            {
                ref Yotsuba entity = ref yotsubas[entityId];
                if (entity.HasNotComponent(YTBComponent.Transform)) continue;

                ref TransformComponent transform = ref transforms[entityId];

                if (entity.HasComponent(YTBComponent.Model3D))
                {
                    ref ModelComponent3D model = ref models[entityId];
                    camera.DrawModel(ref model, ref transform, entity.HasComponent(YTBComponent.Shader) ? shaders[entityId] : null, entityId);
                    mainRendered++;
                }

                if (entity.HasComponent(YTBComponent.YTBModel3D))
                {
                    ref YTBModelComponent3D obj3D = ref ytb3D[entityId];
                    if (obj3D.IsVisible)
                    {
                        Graphics3D.DrawBox(transform.Position, transform.Size, transform.Color, camera.ViewMatrix, camera.ProjectionMatrix);
                        mainRendered++;
                    }
                }

                if (entity.HasComponent(YTBComponent.Sprite))
                {
                    ref SpriteComponent2D sprite = ref sprites[entityId];
                    if (sprite.Is2_5D)
                    {
                        Graphics3D.DrawSprite2_5D(ref sprite, transform.Position, transform.Color, camera.ViewMatrix, camera.ProjectionMatrix, transform.Rotation);
                        mainRendered++;
                    }
                }
            }

            mainPassExecuted = true;

#if YTB && DEBUG
            if (DebugOverlayUI.ShowOcrOverlay)
            {
                var diag = HardwareOcclusionQuerieRuntime.GetDiagnostics();
                EngineUISystem.SendLog($"[Render3D][Frame {_frameCounter}] PrePass={depthPrePassExecuted} QueryPhase={queryPhaseExecuted} MainPass={mainPassExecuted} Rendered={mainRendered} QuerySubmitted={diag.Submitted} QueryCompleted={diag.Completed} VisibleByQuery={diag.VisibleFromQuery} Fallback={diag.ConservativeFallback} QueryOperational={diag.QueriesOperational} ForcedFallback={diag.ForcedFallback}");

                int debugDrawn = 0;
                foreach (ref int entityId in entities)
                {
                    ref Yotsuba entity = ref yotsubas[entityId];
                    if (entity.HasNotComponent(YTBComponent.Transform)) continue;

                    ref TransformComponent transform = ref transforms[entityId];
                    if (entity.HasComponent(YTBComponent.Model3D))
                    {
                        ref ModelComponent3D model = ref models[entityId];
                        Color stateColor = model.IsOccluded ? Color.Red : Color.LimeGreen;
                        float size = model.RadiusSphere > 0f ? model.RadiusSphere * 2f : 1f;
                        Graphics3D.DrawBox(transform.Position + model.SphereOffset, new Vector3(size), stateColor, camera.ViewMatrix, camera.ProjectionMatrix);
                    }
                    else
                    {
                        Graphics3D.DrawBox(transform.Position, Vector3.One * 0.25f, Color.LimeGreen, camera.ViewMatrix, camera.ProjectionMatrix);
                    }

                    debugDrawn++;
                }

                if (debugDrawn > 0)
                    EngineUISystem.SendLog($"[Render3D][Frame {_frameCounter}] DebugVis={debugDrawn} entidades visibles.");
            }
#endif
        }
    }
}
