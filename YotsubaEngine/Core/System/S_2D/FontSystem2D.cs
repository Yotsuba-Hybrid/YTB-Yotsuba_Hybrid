using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using YotsubaEngine.Exceptions;

namespace YotsubaEngine.Core.System.S_2D
{
    /// <summary>
    /// Renderiza componentes de fuente 2D con recorte basado en cámara.
    /// <para>Renders 2D font components with camera-aware culling.</para>
    /// </summary>
    public class FontSystem2D : ISystem
    {

//-:cnd:noEmit
#if YTB
        /// <summary>
        /// Controla si la vista del juego está activa en modo depuración.
        /// <para>Tracks whether the game view is active in debug mode.</para>
        /// </summary>
        public static bool IsGameActive = false;

#endif
//+:cnd:noEmit
        /// <summary>
        /// Stores the entity manager reference for this system.
        /// Almacena la referencia al administrador de entidades de este sistema.
        /// </summary>
        private EntityManager Entities;

        /// <summary>
        /// Obtiene la caché de fuentes sprite cargadas por nombre de recurso.
        /// <para>Gets the loaded sprite font cache by asset name.</para>
        /// </summary>
        public Dictionary<string, SpriteFont> Fuentes { get; private set; } = new Dictionary<string, SpriteFont>();

        /// <summary>
        /// Inicializa el sistema con el administrador de entidades compartido.
        /// <para>Initializes the system with the shared entity manager.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public void InitializeSystem(EntityManager entities)
        {
            Entities = entities;

//-:cnd:noEmit
#if YTB
            EventManager.Instance.Subscribe<OnHiddeORShowGameUI>(OnHiddeORShowGameUIFunc);
            EventManager.Instance.Subscribe<OnShowGameUIHiddeEngineEditor>(OnHiddeORShowGameUIFunc);
#endif
//+:cnd:noEmit
        }

        /// <summary>
        /// Carga recursos de fuente para entidades que incluyen componentes de fuente.
        /// <para>Loads font assets for entities that include font components.</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        public void SharedEntityInitialize(Yotsuba Entidad)
        {

                if (!Entidad.HasComponent(YTBComponent.Font)) return;
                ref var fuenteComp = ref Entities.Font2DComponents[Entidad];

                if (Fuentes.ContainsKey(fuenteComp.Font))
                {
                    Fuentes[fuenteComp.Font] = YTBGlobalState.ContentManager.Load<SpriteFont>(fuenteComp.Font);
                }
                else
                {
                    Fuentes.TryAdd(fuenteComp.Font, YTBGlobalState.ContentManager.Load<SpriteFont>(fuenteComp.Font));
                }
        }


//-:cnd:noEmit
#if YTB
        /// <summary>
        /// Updates debug state when the editor hides or shows the game UI.
        /// Actualiza el estado de depuración cuando el editor oculta o muestra la UI del juego.
        /// </summary>
        private void OnHiddeORShowGameUIFunc(OnShowGameUIHiddeEngineEditor editor)
        {
            IsGameActive = editor.ShowGameUI;
        }

        /// <summary>
        /// Toggles the debug game UI visibility flag.
        /// Alterna la bandera de visibilidad de la UI del juego en depuración.
        /// </summary>
        private void OnHiddeORShowGameUIFunc(OnHiddeORShowGameUI uI)
        {
            IsGameActive = !IsGameActive;
        }
#endif
//+:cnd:noEmit


        /// <summary>
        /// Dibuja todas las entidades de fuente visibles usando recorte por cámara.
        /// <para>Draws all visible font entities using camera culling.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        /// <param name="brocha">Sprite batch para dibujar. <para>Sprite batch for drawing.</para></param>
        public void DrawSystem(GameTime gameTime, SpriteBatch brocha)
        {


//-:cnd:noEmit
#if YTB
            //if (OperatingSystem.IsWindows())
                //if (!IsGameActive) return;
            if (GameWontRun.GameWontRunByException) return;
            bool canRenderUIElements = IsGameActive || !OperatingSystem.IsWindows();

#endif
//+:cnd:noEmit

            // --- Unificar lógica con RenderSystem2D ---
            float currentZoom =
//-:cnd:noEmit
#if YTB
                canRenderUIElements ? YTBGlobalState.CameraZoom : RenderSystem2D.EDITOR_SCALE_CAMERA;
#else
                    YTBGlobalState.CameraZoom;
                                    if (currentZoom <= 0.001f) currentZoom = 0.001f;
#endif
//+:cnd:noEmit

            Vector2 offset =
//-:cnd:noEmit
#if YTB
              canRenderUIElements ? YTBGlobalState.OffsetCamera : new Vector2(RenderSystem2D.EDITOR_OFFSET_CAMERA_X, RenderSystem2D.EDITOR_OFFSET_CAMERA_Y);
#else
                    YTBGlobalState.OffsetCamera;
#endif
//+:cnd:noEmit
            EntityManager entityManager = YTBGlobalState.Game.SceneManager.CurrentScene.EntityManager;
            if (entityManager == null) return;
            var cameraEntity = entityManager.Camera;
            Matrix viewMatrix = Matrix.Identity;

            // Rectángulo que representa el área del mundo que la cámara está viendo actualmente.
            Rectangle cameraWorldBounds = Rectangle.Empty;

            if (cameraEntity != null)
            {
                ref var camTransform = ref entityManager.TransformComponents[cameraEntity.EntityToFollow];
                var viewport = brocha.GraphicsDevice.Viewport;
                Vector2 screenCenter = new Vector2(viewport.Width / 2f, viewport.Height / 2f);


                if (currentZoom <= 0.001f) currentZoom = 0.001f;

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
                var vp = @brocha.GraphicsDevice.Viewport;
                cameraWorldBounds = new Rectangle(0, 0, vp.Width, vp.Height);
            }

            // --- PRIMER PASS (Objetos del Mundo afectados por la Cámara) ---
            brocha.Begin(
                transformMatrix: viewMatrix,
                //effect: exampleEffect,
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.NonPremultiplied,
                samplerState: SamplerState.PointClamp,
                depthStencilState: DepthStencilState.Default,
                rasterizerState: RasterizerState.CullCounterClockwise
            ); foreach (var entidad in Entities.YotsubaEntities)
            {
                if (!entidad.HasComponent(YTBComponent.Font) || !entidad.HasComponent(YTBComponent.Transform)) continue;
                ref var fuenteComp = ref Entities.Font2DComponents[entidad];

                if (!fuenteComp.IsVisible) continue;

                ref var transformComp = ref Entities.TransformComponents[entidad];

                brocha.DrawString(
                    Fuentes[fuenteComp.Font],
                    fuenteComp.Texto,
                    new Vector2(transformComp.Position.X, transformComp.Position.Y),
                    transformComp.Color,
                    transformComp.Rotation,
                    Vector2.Zero,
                    transformComp.Scale,
                    transformComp.SpriteEffects,
                    transformComp.Position.Z
                );
            }

            brocha.End();
        }

        /// <summary>
        /// Actualiza una entidad de fuente durante el pase de actualización compartido.
        /// <para>Updates a font entity during the shared update pass.</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
            // No per-entity update logic required for fonts
        }


        /// <summary>
        /// Actualiza el sistema de fuentes en cada frame.
        /// <para>Updates the font system each frame.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public void UpdateSystem(GameTime gameTime)
        {
            // No system-level update logic required for fonts
        }


    }
}
