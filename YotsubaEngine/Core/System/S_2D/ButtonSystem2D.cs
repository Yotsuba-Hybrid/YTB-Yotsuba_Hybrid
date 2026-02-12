using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents;
using YotsubaEngine.Exceptions;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;
#if YTB
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;
#endif

namespace YotsubaEngine.Core.System.S_2D
{
    /// <summary>
    /// Sistema encargado de gestionar los botones cuando están presionados dentro del juego.
    /// <para>System that handles button interactions in the game.</para>
    /// </summary>
    public class ButtonSystem2D : ISystem
    {
        /// <summary>
        /// Event manager reference.
        /// Referencia al EventManager para manejar eventos.
        /// </summary>
        private EventManager EventManager { get; set; }

        /// <summary>
        /// Entity manager reference.
        /// Referencia al EntityManager para manejar entidades y componentes.
        /// </summary>
        private EntityManager EntityManager { get; set; }

        /// <summary>
        /// Input manager reference.
        /// Referencia al Input manager el cual gestiona las entradas del usuario
        /// </summary>
        private InputManager InputManager { get; set; }
        
        /// <summary>
        /// Inicializa el sistema de botones.
        /// <para>Initializes the button system.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public void InitializeSystem(EntityManager @entities)
        {
#if YTB
			if (GameWontRun.GameWontRunByException) return;
#endif
			EventManager = EventManager.Instance;
            EntityManager = @entities;
            InputManager = InputManager.Instance;
            EngineUISystem.SendLog(typeof(ButtonSystem2D).Name + " Se inicio correctamente");
        }

        /// <summary>
        /// Actualiza las interacciones de botones en cada frame.
        /// <para>Updates button interactions each frame.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public void UpdateSystem(GameTime gameTime)
        {
#if YTB
			if (OperatingSystem.IsWindows())
				if (!RenderSystem2D.IsGameActive) return;

			if (GameWontRun.GameWontRunByException) return;

#endif

			if (EntityManager == null) return;
			Point MousePosition = InputManager.Mouse.Position;
            var touch = InputManager.Touch;

            foreach (var entity in EntityManager.YotsubaEntities)
            {
                if (entity.HasComponent(YTBComponent.Button2D))
                {
                    var button = EntityManager.Button2DComponents[entity.Id];

                    Rectangle Rectangle = button.EffectiveArea;

                    if (Rectangle.Contains(MousePosition) && InputManager.Mouse.WasButtonJustReleased(MouseButton.Left))
                    {
#if YTB
                        if (DebugOverlayUI.ShowButtonLogs)
                        {
                            EngineUISystem.SendLog($"[BUTTON] Entity '{entity.Name}' (ID: {entity.Id}) fue PRESIONADO");
                        }
#endif
                        if (button.Action is not null)
                        {
                            button.Action.Invoke();

                        }
                        continue;
                    }

                    else if (Rectangle.Contains(MousePosition) && !InputManager.Mouse.WasButtonJustReleased(MouseButton.Left))
                    {
#if YTB
                        if (DebugOverlayUI.ShowButtonLogs)
                        {
                            EngineUISystem.SendLog($"[BUTTON] Entity '{entity.Name}' (ID: {entity.Id}) tiene HOVER");
                        }
#endif
                        EventManager.Publish(new OnHoverButton(entity.Id));
                    }

                    if (touch.AnyTouch)
                    {
                        foreach (var touchPoint in touch.GetTouchesByState(TouchLocationState.Released))
                        {
                            if (Rectangle.Contains(touchPoint.Position))
                            {
#if YTB
                                if (DebugOverlayUI.ShowButtonLogs)
                                {
                                    EngineUISystem.SendLog($"[BUTTON] Entity '{entity.Name}' (ID: {entity.Id}) fue PRESIONADO (TOUCH)");
                                }
#endif
                                button.Action.Invoke();
                                return;
                            }
                        }
                    }
                      
                }
            }

        }

        /// <summary>
        /// Hook de actualización compartida (no usado en este sistema).
        /// <para>Shared entity update hook (unused in this system).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Hook de inicialización compartida (no usado en este sistema).
        /// <para>Shared entity initialization hook (unused in this system).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        public void SharedEntityInitialize(Yotsuba Entidad)
        {
            //throw new NotImplementedException();
        }
    }
}
