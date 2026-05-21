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
#endif
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Exceptions;
using YotsubaEngine.Graphics;
using YotsubaEngine.Input;
using YotsubaEngine.Runtime.OCR;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;
namespace YotsubaEngine.Core.System.S_3D
{
    /// <summary>
    /// Sistema que gestiona todo lo que se ve en pantalla y que sea 3D específicamente, renderizar modelos 3D.
    /// <para>System that manages everything rendered in 3D, including 3D models.</para>
    /// </summary>
    public class RenderSystem3D : IRenderSystem
    {
        private int _frameCounter;
        /// <summary>
        /// Referencia al EventManager para manejar eventos.
        /// </summary>
        private EventManager EventManager { get; set; }

        /// <summary>
        /// Referencia a la clase Graphics3D para realizar operaciones de renderizado 3D.
        /// </summary>
        private Graphics3D Graphics3D;

        private Hardware_Occlusion_Querie_Runtime HardwareOcclusionQuerieRuntime;

        /// <summary>
        /// Inicializa el sistema de renderizado 3D.
        /// <para>Initializes the 3D render system.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public override void InitializeSystem(EntityManager entities)
        {
            HardwareOcclusionQuerieRuntime = new();
            EntityManager = entities;
            HardwareOcclusionQuerieRuntime.InitializeSystem(entities);
            Graphics3D = new();
            EventManager = EventManager.Instance;
#if YTB
            EngineUISystem.SendLog(typeof(RenderSystem3D).Name + " Se inicio correctamente");
#endif

        }

        /// <summary>
        /// Actualiza el renderizado 3D en cada frame.
        /// <para>Updates 3D rendering each frame.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public override void Render3D(GameTime gameTime)
        {
            _frameCounter++;

            //-:cnd:noEmit
#if YTB
            if (YTBGlobalState.IsDesktop)
                if (GameWontRun.GameWontRunByException) return;
#endif
            //+:cnd:noEmit

            Span<int> entities = HardwareOcclusionQuerieRuntime.GetEntitiesToRender();
            Span<Yotsuba> Yotsubas = GetEntitiesAsSpan();
            Span<ModelComponent3D> Models = EntityManager.ModelComponents3D.AsSpan();
            Span<TransformComponent> transformComponents = EntityManager.TransformComponents.AsSpan();
            Span<YTBModelComponent3D> ytb3DComponents = EntityManager.YtbModelComponents.AsSpan();

            // TODO: 3D rendering is currently incomplete (Coming Soon)
            // Skip if no 3D models are loaded to avoid errors
            if (Models.Length is 0 && ytb3DComponents.Length is 0) return;

            CameraComponent3D camera = EntityManager.Camera;
            if (camera is null) return;
            camera.Update();
            var gd = YTBGlobalState.GraphicsDevice;

            //-:cnd:noEmit
#if YTB
            // Selección múltiple de modelos 3D en modo engine via ray picking
            if (YTBGlobalState.EngineShortcutsMode)
            {
                MouseInfo mouse = InputManager.Instance.Mouse;
                KeyboardInfo keyboard = InputManager.Instance.Keyboard;
                // Click izquierdo: toggle selección del modelo bajo el cursor
                if (mouse.WasButtonJustPressed(MouseButton.Left) && keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    Viewport viewport = YTBGlobalState.GraphicsDevice.Viewport;
                    Vector3 nearPoint = viewport.Unproject(
                        new Vector3(mouse.X, mouse.Y, 0f),
                        camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
                    Vector3 farPoint = viewport.Unproject(
                        new Vector3(mouse.X, mouse.Y, 1f),
                        camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

                    Vector3 direction = farPoint - nearPoint;
                    direction.Normalize();
                    Ray ray = new Ray(nearPoint, direction);

                    float closestDistance = float.MaxValue;
                    int closestEntityId = -1;
                    
                    foreach (ref Yotsuba entity in Yotsubas)
                    {

                        if (entity.HasNotComponent(YTBComponent.Model3D)  || entity.HasNotComponent(YTBComponent.Transform)) continue;
                        ref TransformComponent transform = ref transformComponents[entity.Id];

                        ref ModelComponent3D model = ref Models[entity.Id];
                        

                        float sphereRadius = model.RadiusSphere;
                        foreach (ModelMesh mesh in model.Model.Meshes)
                        {
                            BoundingSphere sphere = mesh.BoundingSphere;
                            sphere = sphere.Transform(
                                Matrix.CreateTranslation(transform.Position))
                                ;
                            if (sphereRadius >= 0)
                            {
                                sphere.Radius = sphereRadius;
                            }

                            sphere.Center += model.SphereOffset;

                            float? dist = ray.Intersects(sphere);
                            if (dist.HasValue && dist.Value < closestDistance)
                            {
                                closestDistance = dist.Value;
                                closestEntityId = entity.Id;
                            }
                        }
                    }

                    if (closestEntityId != -1)
                    {
                        var selected = YTBGlobalState.SelectedModel3DEntityIds;
                        if (selected.Contains(closestEntityId))
                        {
                            selected.Remove(closestEntityId);
                            EngineUISystem._instance?.ShowModeSwitchAlert("Modelo deseleccionado");
                        }
                        else
                        {
                            selected.Add(closestEntityId);
                            EngineUISystem._instance?.ShowModeSwitchAlert(
                                $"{selected.Count} modelo(s) seleccionado(s)");
                        }
                    }
                }
            }
            else
            {
                // Al salir de modo engine, limpiar selección
                YTBGlobalState.SelectedModel3DEntityIds.Clear();
            }
#endif
            //+:cnd:noEmit

            Span<ShaderComponent> shaderComponents = EntityManager.ShaderComponents.AsSpan();
            Span<SpriteComponent2D> spriteComponent2Ds = EntityManager.Sprite2DComponents.AsSpan();
            bool depthPrePassExecuted = false;
            bool queryPhaseExecuted = false;
            bool mainPassExecuted = false;
            int mainRendered = 0;
            int debugDrawn = 0;

            // 1) Pre-pass de profundidad/occluders: poblar depth buffer antes de queries.
            gd.BlendState = BlendState.Opaque;
            gd.DepthStencilState = DepthStencilState.Default;
            gd.RasterizerState = RasterizerState.CullCounterClockwise;
            depthPrePassExecuted = true;

            // 2) Evaluación de occlusion queries (consume frame anterior y emite nuevas).
            entities = HardwareOcclusionQuerieRuntime.GetEntitiesToRender();
            queryPhaseExecuted = true;

            foreach (ref int entityId in entities)
            {
                ref Yotsuba entity = ref Yotsubas[entityId];
                if (entity.HasNotComponent(YTBComponent.Transform)) continue;

                ref TransformComponent transform = ref transformComponents[entity.Id];

                if (entity.HasComponent(YTBComponent.Model3D))
                {
                    ref var model = ref Models[entity.Id];

                        camera.DrawModel(ref model, ref transform,
                            entity.HasComponent(YTBComponent.Shader) ? shaderComponents[entity.Id] : null,
                            entity.Id);
                        mainRendered++;
                }

                if (entity.HasComponent(YTBComponent.YTBModel3D))
                {
                    ref YTBModelComponent3D obj3D = ref ytb3DComponents[entity.Id];
                    if (!obj3D.IsVisible) continue;
                    Graphics3D.DrawBox(transform.Position, transform.Size, transform.Color, camera.ViewMatrix, camera.ProjectionMatrix);
                    mainRendered++;
                }

                if (!entity.HasComponent(YTBComponent.Sprite)) continue;

                ref SpriteComponent2D sprite = ref spriteComponent2Ds[entity.Id];
                if (!sprite.Is2_5D) continue;

                Graphics3D.DrawSprite2_5D(ref sprite, transform.Position, transform.Color, camera.ViewMatrix, camera.ProjectionMatrix, transform.Rotation);
                mainRendered++;
            }
            mainPassExecuted = true;

            // 4) Debug visualization/logs (solo YTB).
#if YTB
            var diag = HardwareOcclusionQuerieRuntime.GetDiagnostics();
            EngineUISystem.SendLog(
                $"[Render3D][Frame {_frameCounter}] PrePass={depthPrePassExecuted} QueryPhase={queryPhaseExecuted} MainPass={mainPassExecuted} " +
                $"Rendered={mainRendered} QuerySubmitted={diag.Submitted} QueryCompleted={diag.Completed} " +
                $"VisibleByQuery={diag.VisibleFromQuery} Fallback={diag.ConservativeFallback} QueryOperational={diag.QueriesOperational} ForcedFallback={diag.ForcedFallback}");

            if (YTBGlobalState.EngineShortcutsMode)
            {
                foreach (ref int entityId in entities)
                {
                    ref Yotsuba entity = ref Yotsubas[entityId];
                    if (entity.HasNotComponent(YTBComponent.Transform)) continue;
                    ref TransformComponent transform = ref transformComponents[entityId];
                    Graphics3D.DrawBox(transform.Position, Vector3.One * 0.25f, Color.LimeGreen, camera.ViewMatrix, camera.ProjectionMatrix);
                    debugDrawn++;
                }

                if (debugDrawn > 0)
                    EngineUISystem.SendLog($"[Render3D][Frame {_frameCounter}] DebugVis={debugDrawn} entidades visibles.");
            }
#endif
        }
    }
}
