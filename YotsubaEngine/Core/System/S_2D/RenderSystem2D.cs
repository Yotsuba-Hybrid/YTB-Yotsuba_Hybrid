using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using YotsubaEngine.Exceptions;

namespace YotsubaEngine.Core.System.S_2D
{
    /// <summary>
    /// Sistema de renderizado 2D ejecutado después de actualizar el estado del juego.
    /// <para>2D rendering system executed after game state updates.</para>
    /// </summary>
    public class RenderSystem2D
    {

//-:cnd:noEmit
#if YTB
        /// <summary>
        /// Indica si la vista de juego está activa en modo depuración.
        /// <para>Indicates whether the game view is active in debug mode.</para>
        /// </summary>
        public static bool IsGameActive = false;
        /// <summary>
        /// Debug draw helper system.
        /// Sistema auxiliar de debug draw.
        /// </summary>
        private DebugDrawSystem _debugDrawSystem;


        /// <summary>
        /// Especifica el desplazamiento horizontal aplicado a la cámara en la vista del editor.
        /// <para>Specifies the horizontal offset applied to the camera in the editor view.</para>
        /// </summary>
        public const float EDITOR_OFFSET_CAMERA_X = 270;

        /// <summary>
        /// Especifica el desplazamiento vertical aplicado a la cámara en la vista del editor.
        /// <para>Specifies the vertical offset, in units, applied to the camera in the editor view.</para>
        /// </summary>
        public const float EDITOR_OFFSET_CAMERA_Y = 120;

        /// <summary>
        /// Obtiene o establece la escala aplicada a la cámara en el entorno del editor.
        /// <para>Gets or sets the scale factor applied to the camera in the editor environment.</para>
        /// </summary>
        public const float EDITOR_SCALE_CAMERA = 0.535f;
#endif
//+:cnd:noEmit
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
        /// Inicializa el sistema de render y sus suscripciones.
        /// <para>Initializes the render system and subscriptions.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public void InitializeSystem(EntityManager entities)
        {
            //exampleEffect = YTBGlobalState.ContentManager.Load<Effect>("grayscaleEffect");
//-:cnd:noEmit
#if YTB
            if (GameWontRun.GameWontRunByException) return;
#endif
//+:cnd:noEmit
            EventManager = EventManager.Instance;
            EntityManager = entities;
            EngineUISystem.SendLog(typeof(RenderSystem2D).Name + " Se inicio correctamente");



//-:cnd:noEmit
#if YTB
            EventManager.Instance.Subscribe<OnHiddeORShowGameUI>(OnHiddeORShowGameUIFunc);
            EventManager.Instance.Subscribe<OnShowGameUIHiddeEngineEditor>(OnHiddeORShowGameUIFunc);

            // Inicializar Debug Draw System
            _debugDrawSystem = new DebugDrawSystem();
            _debugDrawSystem.InitializeSystem(entities, YTBGame.Instance.GraphicsDevice);
#endif
//+:cnd:noEmit
        }

//-:cnd:noEmit
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
//+:cnd:noEmit

        /// <summary>
        /// Dibuja todas las entidades 2D del frame actual.
        /// <para>Draws all 2D entities for the current frame.</para>
        /// </summary>
        /// <param name="brocha">Sprite batch. <para>Sprite batch.</para></param>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public void UpdateSystem(SpriteBatch @brocha, GameTime @gameTime)
        {
//-:cnd:noEmit
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
//+:cnd:noEmit

            EntityManager entityManager = EntityManager;

//-:cnd:noEmit
#if YTB
            if (entityManager is null) return;
#endif
//+:cnd:noEmit

            var cameraEntity = EntityManager.Camera;
            Matrix viewMatrix = Matrix.Identity;

            // Rectángulo que representa el área del mundo que la cámara está viendo actualmente.
            Rectangle cameraWorldBounds = Rectangle.Empty;

            Span<TransformComponent> transformComponents = entityManager.TransformComponents.AsSpan();

            if (cameraEntity != null)
            {
                ref readonly var camTransform = ref transformComponents[cameraEntity.EntityToFollow];
                var viewport = brocha.GraphicsDevice.Viewport;

                float currentZoom =
//-:cnd:noEmit
#if YTB

              canRenderUIElements ?
#endif
//+:cnd:noEmit
              YTBGlobalState.CameraZoom
//-:cnd:noEmit
#if YTB
              :

              EDITOR_SCALE_CAMERA
#endif
//+:cnd:noEmit
              ;
                // Protección contra división por cero o zoom negativo
                if (currentZoom <= 0.001f) currentZoom = 0.001f;

                Vector2 offset =

//-:cnd:noEmit
#if YTB
                    canRenderUIElements ?
#endif
//+:cnd:noEmit
                    YTBGlobalState.OffsetCamera
//-:cnd:noEmit
#if YTB
                    : new Vector2(EDITOR_OFFSET_CAMERA_X, EDITOR_OFFSET_CAMERA_Y)
#endif
//+:cnd:noEmit
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

            Span<SpriteComponent2D> spriteComponent2Ds = entityManager.Sprite2DComponents.AsSpan();
            Span<Yotsuba> SpanEntities = EntityManager.YotsubaEntities.AsSpan();

            //-:cnd:noEmit
#if YTB
            // FEATURE: Skip UI elements rendering in Editor mode to avoid overlapping with editor tools
            // UI elements use absolute screen coordinates and would interfere with Inspector/Hierarchy panels
            // On non-Windows platforms, UI always renders since Editor mode is Windows-only
            if (canRenderUIElements)
            {
#endif
                //+:cnd:noEmit


                

                @brocha.Begin(
        sortMode: SpriteSortMode.FrontToBack,
        blendState: BlendState.NonPremultiplied,
        samplerState: SamplerState.PointClamp,
        depthStencilState: DepthStencilState.Default,
        rasterizerState: RasterizerState.CullCounterClockwise
    );
                foreach (ref Yotsuba entity in SpanEntities)
                {
                    if (!entity.HasComponent(YTBComponent.YTBUIElement)) continue;

                    ref readonly SpriteComponent2D Sprite = ref spriteComponent2Ds[entity.Id];
                    if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                    ref readonly TransformComponent Transform = ref transformComponents[entity.Id];

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
//-:cnd:noEmit
#if YTB
            }
#endif
//+:cnd:noEmit


            Span<ShaderComponent> shaderComponents = entityManager.ShaderComponents.AsSpan();

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

            foreach (ref Yotsuba entity in SpanEntities)
            {
                // Filtramos entidades que no se deben dibujar en el mundo (sin sprite, sin transform, o que son UI)
                if ((!entity.HasComponent(YTBComponent.Sprite) || !entity.HasComponent(YTBComponent.Transform)) || entity.HasComponent(YTBComponent.Button2D)) continue;

                // Skip entities with shaders - we'll draw them in a separate pass
                // Note: This lookup is intentional to filter shader entities early
                if (entity.HasComponent(YTBComponent.Shader))
                {
                    ShaderComponent shaderComp = shaderComponents[entity.Id];
                    if (shaderComp.IsActive && shaderComp.Effect != null)
                        continue;
                }

                ref readonly SpriteComponent2D Sprite = ref spriteComponent2Ds[entity.Id];

                if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                // Obtenemos el transform (ojo, es una copia struct si no usas ref, pero para lectura rápida está bien)
                ref readonly TransformComponent Transform = ref transformComponents[entity.Id];

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
            foreach (ref Yotsuba entity in SpanEntities)
            {
                if ((!entity.HasComponent(YTBComponent.Sprite) || !entity.HasComponent(YTBComponent.Transform)) || entity.HasComponent(YTBComponent.Button2D)) continue;

                if (!entity.HasComponent(YTBComponent.Shader)) continue;

                ref readonly ShaderComponent shaderComp = ref shaderComponents[entity.Id];
                if (!shaderComp.IsActive || shaderComp.Effect == null) continue;

                ref readonly SpriteComponent2D Sprite = ref spriteComponent2Ds[entity.Id];
                if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                ref readonly TransformComponent Transform = ref transformComponents[entity.Id];

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

//-:cnd:noEmit
#if YTB
            // FEATURE: Skip Button/HUD rendering in Editor mode
            // Buttons use absolute screen coordinates and would interfere with Inspector/Hierarchy panels
            if (canRenderUIElements)
            {
#endif
//+:cnd:noEmit
                // --- SEGUNDO PASS (UI / Botones) ---
                // La UI no se ve afectada por el zoom de la cámara ni su posición, así que no aplicamos el culling de cámara.
                brocha.Begin(
                    sortMode: SpriteSortMode.Deferred,
                    blendState: BlendState.NonPremultiplied,
                    samplerState: SamplerState.PointClamp,
                    depthStencilState: DepthStencilState.Default,
                    rasterizerState: RasterizerState.CullCounterClockwise
                );

                foreach (ref Yotsuba entity in SpanEntities)
                {
                    if ((!entity.HasComponent(YTBComponent.Sprite) || !entity.HasComponent(YTBComponent.Transform)) || !entity.HasComponent(YTBComponent.Button2D) || entity.HasComponent(YTBComponent.YTBUIElement)) continue;

                    ref readonly SpriteComponent2D Sprite = ref spriteComponent2Ds[entity.Id];
                    if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                    ref readonly TransformComponent Transform = ref transformComponents[entity.Id];

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
//-:cnd:noEmit
#if YTB
            }
#endif
//+:cnd:noEmit


//-:cnd:noEmit
#if YTB
            // Dibujar overlays de debug si están activos
            _debugDrawSystem?.DrawDebugOverlays(@brocha, viewMatrix, cameraWorldBounds);
#endif
//+:cnd:noEmit

            EventManager.Publish(new OnFrameRenderEvent
            {
                gameTime = gameTime
            });
        }
    }
}
