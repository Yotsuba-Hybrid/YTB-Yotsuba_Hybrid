using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Immutable;
using System.Linq;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.S_2D;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;

namespace YotsubaEngine.Core.System.YTBDragAndDrop
{
//-:cnd:noEmit
#if YTB
    /// <summary>
    /// Sistema para arrastrar entidades de texto (FontComponent2D) en modo DEBUG.
    /// <para>System for dragging text entities (FontComponent2D) in DEBUG mode.</para>
    /// </summary>
    internal class FontDragSystem : ISystem
    {
        private EntityManager EntityManager;
        private EventManager EventManager;
        private FontSystem2D FontSystem;

        // Variables de estado del arrastre
        private int? _draggedFontEntityId = null;
        private Vector2 _dragOffset = Vector2.Zero;

        /// <summary>
        /// Inicializa el sistema de arrastre de fuentes con el administrador de entidades.
        /// <para>Initializes the font drag system with the entity manager.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
            EventManager = EventManager.Instance;
        }

        /// <summary>
        /// Establece la referencia al FontSystem2D para poder acceder a las fuentes.
        /// <para>Sets the FontSystem2D reference to access fonts.</para>
        /// </summary>
        /// <param name="fontSystem">Sistema de fuentes. <para>Font system.</para></param>
        public void SetFontSystem(FontSystem2D fontSystem)
        {
            FontSystem = fontSystem;
        }

        /// <summary>
        /// Actualiza el arrastre por entidad en cada frame.
        /// <para>Updates per-entity dragging each frame.</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        public void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time)
        {
            // Solo procesar entidades con FontComponent2D
            if (!Entidad.HasComponent(YTBComponent.Font)) return;

            FontEntityDrag(ref Entidad);
        }

        /// <summary>
        /// Inicializa el estado por entidad (sin configuración adicional).
        /// <para>Initializes per-entity state (no additional setup).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        public void SharedEntityInitialize(ref Yotsuba Entidad)
        {
            // No se requiere inicialización especial
        }

        /// <summary>
        /// Ejecuta la actualización global del sistema (sin lógica adicional).
        /// <para>Runs the global system update (no additional logic).</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public void UpdateSystem(GameTime gameTime)
        {
            // No se requiere lógica global por frame
        }
        private void FontEntityDrag(ref Yotsuba entity)
        {
            // Si aún no tenemos la referencia al FontSystem, salimos
            if (FontSystem == null) return;

            var input = InputManager.Instance;
            var mouseState = input.Mouse.CurrentState;
            var keyboard = input.Keyboard;

            // Verificar que la entidad tenga Transform
            if (!entity.HasComponent(YTBComponent.Transform)) return;

            Span<TransformComponent> transformComponentsSpan = EntityManager.TransformComponents.AsSpan();

            ref TransformComponent transform = ref transformComponentsSpan[entity.Id];

            // Obtener referencia al componente de fuente para calcular el bounding box del texto
            ref var fontComponent = ref EntityManager.Font2DComponents[entity.Id];

            // Obtener la fuente para medir el texto
            if (!FontSystem.Fuentes.ContainsKey(fontComponent.Font))
                return;

            var font = FontSystem.Fuentes[fontComponent.Font];
            Vector2 textSize = font.MeasureString(fontComponent.Texto);
            transform.Size = new Vector3(textSize, 0);

            // Datos básicos de la entidad (Asumimos que Position es la esquina superior izquierda)
            Vector2 entityPos = new Vector2(transform.Position.X, transform.Position.Y);
            Vector2 scaledTextSize = textSize * transform.Scale;

            // --- VARIABLES DE CÁLCULO ---
            Vector2 currentMousePos;

            // -----------------------------------------------------------------------
            // CASO: TEXTO EN EL MUNDO (afectado por cámara)
            // -----------------------------------------------------------------------
            Vector2 screenMousePos = new Vector2(mouseState.X, mouseState.Y);

            if (EntityManager.Camera != null)
            {
//-:cnd:noEmit
#if YTB
                bool canRenderUIElements = RenderSystem2D.IsGameActive || !OperatingSystem.IsWindows();
#else
                bool canRenderUIElements = !OperatingSystem.IsWindows();
#endif
//+:cnd:noEmit

                float currentZoom =
//-:cnd:noEmit
#if YTB
                    canRenderUIElements ?
#endif
//+:cnd:noEmit
                    YTBGlobalState.CameraZoom
//-:cnd:noEmit
#if YTB
                    : RenderSystem2D.EDITOR_SCALE_CAMERA
#endif
//+:cnd:noEmit
                    ;

                Vector2 offset =
//-:cnd:noEmit
#if YTB
                    canRenderUIElements ?
#endif
//+:cnd:noEmit
                    YTBGlobalState.OffsetCamera
//-:cnd:noEmit
#if YTB
                    : new Vector2(RenderSystem2D.EDITOR_OFFSET_CAMERA_X, RenderSystem2D.EDITOR_OFFSET_CAMERA_Y)
#endif
//+:cnd:noEmit
                    ;

                ref var camTransform = ref transformComponentsSpan[EntityManager.Camera.EntityToFollow];
                var viewport = ((YTBGame)YTBGame.Instance).GraphicsDevice.Viewport;
                Vector2 screenCenter = new Vector2(viewport.Width / 2f, viewport.Height / 2f);

                Matrix viewMatrix = EntityManager.Camera.Get2DViewMatrix(
                    new Vector2(camTransform.Position.X, camTransform.Position.Y) + offset,
                    screenCenter,
                    currentZoom,
                    0f
                );

                Matrix inverseView = Matrix.Invert(viewMatrix);
                currentMousePos = Vector2.Transform(screenMousePos, inverseView);
            }
            else
            {
                currentMousePos = screenMousePos;
            }

            // -----------------------------------------------------------------------
            // CAMBIO: Usar el tamaño completo del texto como área de arrastre
            // -----------------------------------------------------------------------
            Rectangle handleRect = new Rectangle(
                (int)entityPos.X,
                (int)entityPos.Y,
                (int)scaledTextSize.X,
                (int)scaledTextSize.Y
            );

            // -----------------------------------------------------------------------
            // LÓGICA DE ARRASTRE (solo en modo Engine)
            // -----------------------------------------------------------------------
            if (!YTBGlobalState.EngineShortcutsMode) return;
            bool isCtrlShiftHeld = keyboard.IsKeyDown(Keys.LeftControl) && keyboard.IsKeyDown(Keys.LeftShift);

            // 1. INICIO DEL ARRASTRE
            if (_draggedFontEntityId == null)
            {
                if (input.Mouse.IsButtonDown(MouseButton.Left) && isCtrlShiftHeld && handleRect.Contains(currentMousePos))
                {
                    _draggedFontEntityId = entity.Id;
                    _dragOffset = entityPos - currentMousePos;
                }
            }
            // 2. DURANTE EL ARRASTRE
            else if (_draggedFontEntityId == entity.Id)
            {
                if (isCtrlShiftHeld)
                {
                    Vector2 newPos = currentMousePos + _dragOffset;
                    transform.SetPosition(newPos.X, newPos.Y, transform.Position.Z);
                }
                else
                {
                    // 3. SOLTAR EL ARRASTRE
                    GuardarCambiosEnEscena(entity, transform);
                    _draggedFontEntityId = null;
                }
            }
        }

        /// <summary>
        /// Guarda los cambios de posición del texto en el archivo .ytb de la escena
        /// </summary>
        private void GuardarCambiosEnEscena(Yotsuba entity, TransformComponent transform)
        {
            var gameInfo = EngineUISystem.GameInfo;
            var gameClass = (YTBGame)YTBGame.Instance;
            string sceneName = gameClass.SceneManager.CurrentScene.SceneName;

            var currentScene = gameInfo.Scene.FirstOrDefault(x => x.Name == sceneName);
            if (currentScene == null) return;

            var entityData = currentScene.Entities.FirstOrDefault(x => x.Name == entity.Name);
            if (entityData == null) return;

            var transformComponentData = entityData.Components.FirstOrDefault(x => x.ComponentName == nameof(TransformComponent));
            if (transformComponentData == null) return;

            foreach (var prop in transformComponentData.Propiedades.ToImmutableArray())
            {
                if (prop.Item1 == "Position")
                {
                    UpdateProperty(transformComponentData, prop.Item1, $"{transform.Position.X},{transform.Position.Y},{transform.Position.Z}");
                }
            }
        }

        /// <summary>
        /// Actualiza una propiedad específica del componente
        /// </summary>
        private void UpdateProperty(YTBComponents component, string propertyName, string newValue)
        {
            int index = component.Propiedades.FindIndex(x => x.Item1 == propertyName);
            if (index >= 0)
            {
                component.Propiedades[index] = new(propertyName, newValue);
            }
        }
    }
#endif
//+:cnd:noEmit
}
