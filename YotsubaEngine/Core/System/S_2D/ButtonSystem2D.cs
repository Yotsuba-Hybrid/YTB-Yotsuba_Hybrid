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
using YotsubaEngine.Core.Component.C_2D;

//-:cnd:noEmit
#if YTB
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;
#endif
//+:cnd:noEmit

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
//-:cnd:noEmit
#if YTB
			if (GameWontRun.GameWontRunByException) return;
#endif
//+:cnd:noEmit
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
//-:cnd:noEmit
#if YTB
			if (OperatingSystem.IsWindows())
				if (!RenderSystem2D.IsGameActive) return;

			if (GameWontRun.GameWontRunByException) return;

#endif
//+:cnd:noEmit

			if (EntityManager == null) return;
			Point MousePosition = InputManager.Mouse.Position;
            var touch = InputManager.Touch;

            Span<ButtonComponent2D> buttonComponents = EntityManager.Button2DComponents.AsSpan();
            foreach (ref Yotsuba entity in EntityManager.YotsubaEntities.AsSpan())
            {
                if (entity.HasComponent(YTBComponent.Button2D))
                {
                    ref var button = ref buttonComponents[entity.Id];

                    Rectangle Rectangle = button.EffectiveArea;

                    if (Rectangle.Contains(MousePosition) && InputManager.Mouse.WasButtonJustReleased(MouseButton.Left))
                    {
//-:cnd:noEmit
#if YTB
                        if (DebugOverlayUI.ShowButtonLogs)
                        {
                            EngineUISystem.SendLog($"[BUTTON] Entity '{entity.Name}' (ID: {entity.Id}) fue PRESIONADO");
                        }
#endif
//+:cnd:noEmit
                        if (button.Action is not null)
                        {
                            button.Action.Invoke();

                        }
                        continue;
                    }

                    else if (Rectangle.Contains(MousePosition) && !InputManager.Mouse.WasButtonJustReleased(MouseButton.Left))
                    {
//-:cnd:noEmit
#if YTB
                        if (DebugOverlayUI.ShowButtonLogs)
                        {
                            EngineUISystem.SendLog($"[BUTTON] Entity '{entity.Name}' (ID: {entity.Id}) tiene HOVER");
                        }
#endif
//+:cnd:noEmit
                        EventManager.Publish(new OnHoverButton(entity.Id));
                    }

                    if (touch.AnyTouch)
                    {
                        foreach (var touchPoint in touch.GetTouchesByState(TouchLocationState.Released))
                        {
                            if (Rectangle.Contains(touchPoint.Position))
                            {
//-:cnd:noEmit
#if YTB
                                if (DebugOverlayUI.ShowButtonLogs)
                                {
                                    EngineUISystem.SendLog($"[BUTTON] Entity '{entity.Name}' (ID: {entity.Id}) fue PRESIONADO (TOUCH)");
                                }
#endif
//+:cnd:noEmit
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
        public void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Hook de inicialización compartida (no usado en este sistema).
        /// <para>Shared entity initialization hook (unused in this system).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        public void SharedEntityInitialize(ref Yotsuba Entidad)
        {
            //throw new NotImplementedException();
        }
    }
}
