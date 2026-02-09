using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection.Metadata;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using YotsubaEngine.Exceptions;

namespace YotsubaEngine.Core.System.S_2D
{
    /// <summary>
    /// 2D rendering system executed after game state updates.
    /// Sistema de renderizado de la UI. Ultimo sistema en ejecutarse.
    /// Renderiza la UI despues de que el estado del juego se actualizo por completo en los demas sistemas.
    /// </summary>
    public class RenderSystem2D
    {

#if YTB
        /// <summary>
        /// Indicates whether the game view is active in debug mode.
        /// Indica si la vista de juego está activa en modo depuración.
        /// </summary>
        public static bool IsGameActive = false;
        /// <summary>
        /// Debug draw helper system.
        /// Sistema auxiliar de debug draw.
        /// </summary>
        private DebugDrawSystem _debugDrawSystem;


        /// <summary>
        /// Specifies the offset applied to the camera in the editor view.
        /// </summary>
        public /*const*/static float EDITOR_OFFSET_CAMERA_X = 270;

        /// <summary>
        /// Specifies the vertical offset, in units, applied to the camera in the editor view.
        /// </summary>
        public /*const*/ static float EDITOR_OFFSET_CAMERA_Y = 150;

        /// <summary>
        /// Gets or sets the scale factor applied to the camera in the editor environment.
        /// </summary>
        public /*const*/ static float EDITOR_SCALE_CAMERA = 0.535f;
#endif
        /// <summary>
        /// Event manager reference.
        /// Referencia al administrador de eventos.
        /// </summary>
        private EventManager EventManager { get; set; }

        /// <summary>
        /// Entity manager reference.
        /// Referencia al administrador de entidades.
        /// </summary>
        private EntityManager EntityManager { get; set; }

        // Example of loading an effect
        //Effect exampleEffect;

        /// <summary>
        /// Initializes the render system and subscriptions.
        /// Inicializa el sistema de render y sus suscripciones.
        /// </summary>
        /// <param name="entities">Entity manager. Administrador de entidades.</param>
        public void InitializeSystem(EntityManager entities)
        {
            //exampleEffect = YTBGlobalState.ContentManager.Load<Effect>("grayscaleEffect");
#if YTB
            if (GameWontRun.GameWontRunByException) return;
#endif
            EventManager = EventManager.Instance;
            EntityManager = entities;
            EngineUISystem.SendLog(typeof(RenderSystem2D).Name + " Se inicio correctamente");



#if YTB
            EventManager.Instance.Subscribe<OnHiddeORShowGameUI>(OnHiddeORShowGameUIFunc);
            EventManager.Instance.Subscribe<OnShowGameUIHiddeEngineEditor>(OnHiddeORShowGameUIFunc);

            // Inicializar Debug Draw System
            _debugDrawSystem = new DebugDrawSystem();
            _debugDrawSystem.InitializeSystem(entities, YTBGame.Instance.GraphicsDevice);
#endif
        }

#if YTB
        /// <summary>
        /// Handles game UI visibility change events.
        /// Maneja eventos de visibilidad de la UI del juego.
        /// </summary>
        private void OnHiddeORShowGameUIFunc(OnShowGameUIHiddeEngineEditor editor)
        {
            IsGameActive = editor.ShowGameUI;
        }

        /// <summary>
        /// Toggles the game UI visibility flag.
        /// Alterna la bandera de visibilidad de la UI del juego.
        /// </summary>
        private void OnHiddeORShowGameUIFunc(OnHiddeORShowGameUI uI)
        {
            IsGameActive = !IsGameActive;
        }
#endif

        /// <summary>
        /// Draws all 2D entities for the current frame.
        /// Dibuja todas las entidades 2D del frame actual.
        /// </summary>
        /// <param name="brocha">Sprite batch. Sprite batch.</param>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        public void UpdateSystem(SpriteBatch @brocha, GameTime @gameTime)
        {
#if YTB
            // FEATURE: Allow rendering in Editor mode for real-time scene preview
            // The previous early return prevented any rendering in Editor mode.
            // Now we only skip UI/Button rendering in Editor mode (handled below).
            if (GameWontRun.GameWontRunByException) return;

            // FEATURE: Skip UI elements rendering in Editor mode to avoid overlapping with editor tools
            // UI elements use absolute screen coordinates and would interfere with Inspector/Hierarchy panels
            // On non-Windows platforms, UI always renders since Editor mode is Windows-only
            bool canRenderUIElements = IsGameActive || !OperatingSystem.IsWindows();
#endif

            EntityManager entityManager = EntityManager;
            var cameraEntity = EntityManager.Camera;
            Matrix viewMatrix = Matrix.Identity;

            // Rectángulo que representa el área del mundo que la cámara está viendo actualmente.
            Rectangle cameraWorldBounds = Rectangle.Empty;

            if (cameraEntity != null)
            {
                ref var camTransform = ref EntityManager.TransformComponents[cameraEntity.EntityToFollow];
                var viewport = brocha.GraphicsDevice.Viewport;

                float currentZoom =
#if YTB

              canRenderUIElements ?
#endif
              YTBGlobalState.CameraZoom
#if YTB
              :

              EDITOR_SCALE_CAMERA
#endif
              ;
                // Protección contra división por cero o zoom negativo
                if (currentZoom <= 0.001f) currentZoom = 0.001f;

                Vector2 offset =

#if YTB
                    canRenderUIElements ?
#endif
                    YTBGlobalState.OffsetCamera
#if YTB
                    : new Vector2(EDITOR_OFFSET_CAMERA_X, EDITOR_OFFSET_CAMERA_Y)
#endif
                    ;

                Vector2 screenCenter = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
                viewMatrix = cameraEntity.Get2DViewMatrix(
                    new Vector2(camTransform.Position.X, camTransform.Position.Y) + offset,
                    screenCenter,
                    currentZoom,
                    0f
                );

                // 2. CÁLCULO DEL RECTÁNGULO DE VISIÓN (Frustum Culling 2D)
                // Cuanto mayor es el zoom, MENOS mundo vemos. Por eso dividimos el tamaño de pantalla entre el zoom.
                float visibleWorldWidth = viewport.Width / currentZoom;
                float visibleWorldHeight = viewport.Height / currentZoom;

                // Calculamos la esquina superior izquierda (Top-Left) en coordenadas del mundo.
                // Asumimos que la posición de la cámara es el CENTRO de la vista.
                // Importante: Incluir el offset de cámara para que el cálculo sea consistente con viewMatrix
                Vector2 cameraCenter = new Vector2(camTransform.Position.X, camTransform.Position.Y) + offset;
                float camLeft = cameraCenter.X - (visibleWorldWidth / 2f);
                float camTop = cameraCenter.Y - (visibleWorldHeight / 2f);

                // 3. Margen de seguridad (Padding)
                // Importante para objetos que están rotados o sprite sheets grandes que apenas tocan el borde.
                int padding = 100;

                cameraWorldBounds = new Rectangle(
                    (int)(camLeft - padding),
                    (int)(camTop - padding),
                    (int)(visibleWorldWidth + (padding * 2)),
                    (int)(visibleWorldHeight + (padding * 2))
                );
            }
            else
            {
                // Si no hay cámara, asumimos que el área visible es el tamaño de la ventana sin transformación (Zoom 1)
                var vp = brocha.GraphicsDevice.Viewport;
                cameraWorldBounds = new Rectangle(0, 0, vp.Width, vp.Height);
            }


#if YTB
            // FEATURE: Skip UI elements rendering in Editor mode to avoid overlapping with editor tools
            // UI elements use absolute screen coordinates and would interfere with Inspector/Hierarchy panels
            // On non-Windows platforms, UI always renders since Editor mode is Windows-only
            if (canRenderUIElements)
            {
#endif
                @brocha.Begin(
        sortMode: SpriteSortMode.FrontToBack,
        blendState: BlendState.NonPremultiplied,
        samplerState: SamplerState.PointClamp,
        depthStencilState: DepthStencilState.Default,
        rasterizerState: RasterizerState.CullCounterClockwise
    );
                foreach (var entity in EntityManager.YotsubaEntities)
                {
                    if (!entity.HasComponent(YTBComponent.YTBUIElement)) continue;

                    ref SpriteComponent2D Sprite = ref entityManager.Sprite2DComponents[entity.Id];
                    if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                    ref TransformComponent Transform = ref entityManager.TransformComponents[entity.Id];

                    // Use destinationRectangle for UI elements (pixel texture scaled to Size)
                    Rectangle destinationRect = new Rectangle(
                        (int)Transform.Position.X,
                        (int)Transform.Position.Y,
                        (int)Transform.Size.X,
                        (int)Transform.Size.Y
                    );

                    @brocha.Draw(
                        Sprite.Texture,
                        destinationRect,
                        null,  // sourceRectangle - null uses entire texture
                        Transform.Color,
                        Transform.Rotation,
                        Vector2.Zero,  // origin at top-left for UI
                        Transform.SpriteEffects,
                        0
                    );
                }

                @brocha.End();
#if YTB
            }
#endif

            // --- PRIMER PASS (Objetos del Mundo afectados por la Cámara) ---
            // Primero dibujamos entidades SIN shader para máxima eficiencia de batch
            brocha.Begin(
                transformMatrix: viewMatrix,
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.NonPremultiplied,
                samplerState: SamplerState.PointClamp,
                depthStencilState: DepthStencilState.Default,
                rasterizerState: RasterizerState.CullCounterClockwise
            );

            foreach (var entity in EntityManager.YotsubaEntities)
            {
                // Filtramos entidades que no se deben dibujar en el mundo (sin sprite, sin transform, o que son UI)
                if ((!entity.HasComponent(YTBComponent.Sprite) || !entity.HasComponent(YTBComponent.Transform)) || entity.HasComponent(YTBComponent.Button2D)) continue;

                // Skip entities with shaders - we'll draw them in a separate pass
                // Note: This lookup is intentional to filter shader entities early
                if (entity.HasComponent(YTBComponent.Shader))
                {
                    ShaderComponent shaderComp = entityManager.ShaderComponents[entity.Id];
                    if (shaderComp.IsActive && shaderComp.Effect != null)
                        continue;
                }

                SpriteComponent2D Sprite = entityManager.Sprite2DComponents[entity.Id];

                if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                // Obtenemos el transform (ojo, es una copia struct si no usas ref, pero para lectura rápida está bien)
                TransformComponent Transform = entityManager.TransformComponents[entity.Id];

                // --- LÓGICA DE CULLING ---

                // Calculamos el tamaño real de la entidad en el mundo (tamaño base * escala local)
                float entWidth = Transform.Size.X * Transform.Scale;
                float entHeight = Transform.Size.Y * Transform.Scale;

                // Creamos el bounding box de la entidad.
                // Como el Draw usa el origen en el centro (Size * 0.5), Transform.Position es el centro del objeto.
                // Debemos restar la mitad del ancho/alto para obtener la esquina superior izquierda del Rect.
                Rectangle entityBounds = new Rectangle(
                    (int)(Transform.Position.X - entWidth / 2f),
                    (int)(Transform.Position.Y - entHeight / 2f),
                    (int)entWidth,
                    (int)entHeight
                );

                // Si el rectángulo de la cámara NO toca el rectángulo de la entidad, NO DIBUJAR.
                if (!cameraWorldBounds.Intersects(entityBounds))
                {
                    continue; // Skip draw call
                }
                // -------------------------

                @brocha.Draw(
                    Sprite.Texture,
                    new Vector2(Transform.Position.X, Transform.Position.Y),
                    Sprite.SourceRectangle,
                    Transform.Color,
                    Transform.Rotation,
                    new Vector2(Transform.Size.X * 0.5f, Transform.Size.Y * 0.5f), // Origin en el centro
                    Transform.Scale,
                    Transform.SpriteEffects,
                    Transform.LayerDepth
                );
            }

            @brocha.End();

            // --- SEGUNDO PASS (Entidades CON shader) ---
            // Dibujamos entidades con shaders en pases separados
            foreach (var entity in EntityManager.YotsubaEntities)
            {
                if ((!entity.HasComponent(YTBComponent.Sprite) || !entity.HasComponent(YTBComponent.Transform)) || entity.HasComponent(YTBComponent.Button2D)) continue;

                if (!entity.HasComponent(YTBComponent.Shader)) continue;

                ShaderComponent shaderComp = entityManager.ShaderComponents[entity.Id];
                if (!shaderComp.IsActive || shaderComp.Effect == null) continue;

                SpriteComponent2D Sprite = entityManager.Sprite2DComponents[entity.Id];
                if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                TransformComponent Transform = entityManager.TransformComponents[entity.Id];

                // Culling check
                float entWidth = Transform.Size.X * Transform.Scale;
                float entHeight = Transform.Size.Y * Transform.Scale;
                Rectangle entityBounds = new Rectangle(
                    (int)(Transform.Position.X - entWidth / 2f),
                    (int)(Transform.Position.Y - entHeight / 2f),
                    (int)entWidth,
                    (int)entHeight
                );

                if (!cameraWorldBounds.Intersects(entityBounds))
                    continue;


                // Begin con el shader específico
                // Note: Each shader entity breaks batching - this is a design trade-off
                // for flexibility. Group entities by shader for better performance.
                brocha.Begin(
                    transformMatrix: viewMatrix,
                    effect: shaderComp.Effect,
                    sortMode: SpriteSortMode.Deferred,
                    blendState: BlendState.NonPremultiplied,
                    samplerState: SamplerState.PointClamp,
                    depthStencilState: DepthStencilState.Default,
                    rasterizerState: RasterizerState.CullCounterClockwise
                );

                @brocha.Draw(
                    Sprite.Texture,
                    new Vector2(Transform.Position.X, Transform.Position.Y),
                    Sprite.SourceRectangle,
                    Transform.Color,
                    Transform.Rotation,
                    new Vector2(Transform.Size.X * 0.5f, Transform.Size.Y * 0.5f),
                    Transform.Scale,
                    Transform.SpriteEffects,
                    Transform.LayerDepth
                );

                @brocha.End();
            }

#if YTB
            // FEATURE: Skip Button/HUD rendering in Editor mode
            // Buttons use absolute screen coordinates and would interfere with Inspector/Hierarchy panels
            if (canRenderUIElements)
            {
#endif
                // --- SEGUNDO PASS (UI / Botones) ---
                // La UI no se ve afectada por el zoom de la cámara ni su posición, así que no aplicamos el culling de cámara.
                brocha.Begin(
                    sortMode: SpriteSortMode.Deferred,
                    blendState: BlendState.NonPremultiplied,
                    samplerState: SamplerState.PointClamp,
                    depthStencilState: DepthStencilState.Default,
                    rasterizerState: RasterizerState.CullCounterClockwise
                );

                foreach (var entity in EntityManager.YotsubaEntities)
                {
                    if ((!entity.HasComponent(YTBComponent.Sprite) || !entity.HasComponent(YTBComponent.Transform)) || !entity.HasComponent(YTBComponent.Button2D) || entity.HasComponent(YTBComponent.YTBUIElement)) continue;

                    SpriteComponent2D Sprite = entityManager.Sprite2DComponents[entity.Id];
                    if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                    TransformComponent Transform = entityManager.TransformComponents[entity.Id];

                    @brocha.Draw(
                        Sprite.Texture,
                        new Vector2(Transform.Position.X, Transform.Position.Y),
                        Sprite.SourceRectangle,
                        Transform.Color,
                        Transform.Rotation,
                        new Vector2(Transform.Size.X * 0.5f, Transform.Size.Y * 0.5f),
                        Transform.Scale,
                        Transform.SpriteEffects,
                        Transform.LayerDepth
                    );
                }

                @brocha.End();
#if YTB
            }
#endif


#if YTB
            // Dibujar overlays de debug si están activos
            _debugDrawSystem?.DrawDebugOverlays(@brocha, viewMatrix, cameraWorldBounds);
#endif

            EventManager.Publish(new OnFrameRenderEvent
            {
                gameTime = gameTime
            });
        }
    }
}