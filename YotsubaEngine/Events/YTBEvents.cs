using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;

namespace YotsubaEngine.Events.YTBEvents
{

    /// <summary>
    /// Event published to change an entity animation.
    /// Evento que se publica para cambiar la animacion de una entidad.
    /// </summary>
    /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
    /// <param name="animation">Animation type. Tipo de animación.</param>
    public class AnimationChangeEvent(int entityId, AnimationType animation)
    {
        /// <summary>
        /// Gets the target entity identifier.
        /// Obtiene el identificador de la entidad objetivo.
        /// </summary>
        public int EntityId = entityId;

        /// <summary>
        /// Gets the animation name to apply.
        /// Obtiene el nombre de la animación a aplicar.
        /// </summary>
        public AnimationType AnimationName = animation;
    }

    /// <summary>
    /// Event published after each frame render.
    /// Evento que se publica al final del renderizado de cada frame.
    /// </summary>
    public class OnFrameRenderEvent
    {
        /// <summary>
        /// Game time for the rendered frame.
        /// Tiempo de juego para el frame renderizado.
        /// </summary>
        public GameTime gameTime;
    }

	/// <summary>
	/// Event published when a non-looping animation finishes.
	/// Evento que se publica cuando una animacion que no es en loop termina de reproducirse
	/// </summary>
	public class OnAnimationDontLoopReleaseEvent
	{
        /// <summary>
        /// Entity that finished the animation.
        /// Entidad que terminó la animación.
        /// </summary>
        public int EntityId;

        /// <summary>
        /// Animation name that finished.
        /// Nombre de la animación que terminó.
        /// </summary>
        public AnimationType AnimationName;
	}

	/// <summary>
	/// Event published when two entities collide.
	/// Evento que se publica si dos entidades colisionaron
	/// </summary>
	public class OnCollitionEvent
    {
        /// <summary>
        /// Game time for the collision.
        /// Tiempo de juego para la colisión.
        /// </summary>
        public GameTime GameTime;

        /// <summary>
        /// Entity attempting to move.
        /// Entidad que intenta moverse.
        /// </summary>
        public Yotsuba EntityTryMove;

        /// <summary>
        /// Entity acting as obstacle.
        /// Entidad que actúa como obstáculo.
        /// </summary>
        public Yotsuba EntityImpediment;

    }

    /// <summary>
    /// Collision direction for physics events.
    /// Dirección de colisión para eventos de física.
    /// </summary>
    public enum CollisionDirection
    {
        /// <summary>
        /// Collision from above (landing on ground).
        /// Colisión desde arriba (aterrizaje en el suelo).
        /// </summary>
        Bottom,
        /// <summary>
        /// Collision from below (hitting ceiling).
        /// Colisión desde abajo (golpeando el techo).
        /// </summary>
        Top,
        /// <summary>
        /// Collision from the left side.
        /// Colisión desde el lado izquierdo.
        /// </summary>
        Left,
        /// <summary>
        /// Collision from the right side.
        /// Colisión desde el lado derecho.
        /// </summary>
        Right
    }

    /// <summary>
    /// Event published when an entity lands on the ground (Platform mode).
    /// Evento que se publica cuando una entidad aterriza en el suelo (modo Platform).
    /// </summary>
    public class OnEntityGroundedEvent
    {
        /// <summary>
        /// Entity that landed.
        /// Entidad que aterrizó.
        /// </summary>
        public int EntityId;

        /// <summary>
        /// Game time for the event.
        /// Tiempo de juego del evento.
        /// </summary>
        public GameTime GameTime;
    }

    /// <summary>
    /// Event published when an entity leaves the ground (Platform mode).
    /// Evento que se publica cuando una entidad deja el suelo (modo Platform).
    /// </summary>
    public class OnEntityAirborneEvent
    {
        /// <summary>
        /// Entity that left the ground.
        /// Entidad que dejó el suelo.
        /// </summary>
        public int EntityId;

        /// <summary>
        /// Game time for the event.
        /// Tiempo de juego del evento.
        /// </summary>
        public GameTime GameTime;
    }

    /// <summary>
    /// Event published when an entity jumps (Platform mode).
    /// Evento que se publica cuando una entidad salta (modo Platform).
    /// </summary>
    public class OnEntityJumpEvent
    {
        /// <summary>
        /// Entity that jumped.
        /// Entidad que saltó.
        /// </summary>
        public int EntityId;

        /// <summary>
        /// Game time for the event.
        /// Tiempo de juego del evento.
        /// </summary>
        public GameTime GameTime;
    }

    /// <summary>
    /// Event published when keyboard state changes.
    /// Evento que se publica cuando se cambia el estado del teclado
    /// </summary>
    /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
    /// <param name="key">Keyboard key. Tecla del teclado.</param>
    /// <param name="gameTime">Game time. Tiempo de juego.</param>
    /// <param name="actionEntityInput">Action mapping. Acción mapeada.</param>
    /// <param name="typeEvent">Input event type. Tipo de evento de entrada.</param>
    public class OnKeyBoardEvent(int entityId, Keys key, GameTime gameTime, ActionEntityInput actionEntityInput, InputEventType typeEvent) : OnInputEvent(entityId, gameTime, actionEntityInput, typeEvent)
    {
        /// <summary>
        /// Key associated with the event.
        /// Tecla asociada al evento.
        /// </summary>
        public Keys Key = key;
    }

    /// <summary>
    /// Defines the kind of input event.
    /// Define el tipo de evento de entrada.
    /// </summary>
    public enum InputEventType
    {
        /// <summary>
        /// Input was just released.
        /// La entrada se soltó recientemente.
        /// </summary>
        JustReleased,
        /// <summary>
        /// Input was just pressed.
        /// La entrada se presionó recientemente.
        /// </summary>
        JustPressed,
        /// <summary>
        /// Input is held down.
        /// La entrada se mantiene presionada.
        /// </summary>
        HoldDown
    }

    /// <summary>
    /// Event published when mouse state changes.
    /// Evento que se dispara al cambiar el estado del Mouse
    /// </summary>
    /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
    /// <param name="mouseButton">Mouse button. Botón del mouse.</param>
    /// <param name="gameTime">Game time. Tiempo de juego.</param>
    /// <param name="actionEntityInput">Action mapping. Acción mapeada.</param>
    /// <param name="typeEvent">Input event type. Tipo de evento de entrada.</param>
    public class OnMouseEvent(int entityId, MouseButton mouseButton, GameTime gameTime, ActionEntityInput actionEntityInput, InputEventType typeEvent) : OnInputEvent(entityId, gameTime, actionEntityInput, typeEvent)
    {
        /// <summary>
        /// Mouse button associated with the event.
        /// Botón del mouse asociado al evento.
        /// </summary>
        public MouseButton MouseButton = mouseButton;
    }

    /// <summary>
    /// Event published when gamepad state changes.
    /// Evento que se dispara al cambiar el estado del Mando
    /// </summary>
    /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
    /// <param name="gamePadButton">Gamepad button. Botón del gamepad.</param>
    /// <param name="gameTime">Game time. Tiempo de juego.</param>
    /// <param name="actionEntityInput">Action mapping. Acción mapeada.</param>
    /// <param name="typeEvent">Input event type. Tipo de evento de entrada.</param>
    public class OnGamePadEvent(int entityId, Buttons gamePadButton, GameTime gameTime, ActionEntityInput actionEntityInput, InputEventType typeEvent) : OnInputEvent(entityId, gameTime, actionEntityInput, typeEvent)
    {
        /// <summary>
        /// Gamepad button associated with the event.
        /// Botón del gamepad asociado al evento.
        /// </summary>
        public Buttons Button = gamePadButton;
    }

    /// <summary>
    /// Event published when gamepad thumbstick position changes.
    /// Evento que se dispara al mover el thumbstick del gamepad.
    /// </summary>
    /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
    /// <param name="gameTime">Game time. Tiempo de juego.</param>
    /// <param name="leftThumbstick">Left thumbstick position. Posición del stick izquierdo.</param>
    /// <param name="rightThumbstick">Right thumbstick position. Posición del stick derecho.</param>
    /// <param name="playerIndex">Player index for the gamepad. Índice del jugador del gamepad.</param>
    public class OnThumbstickEvent(int entityId, GameTime gameTime, Vector2 leftThumbstick, Vector2 rightThumbstick, PlayerIndex playerIndex)
    {
        /// <summary>
        /// Entity identifier associated with the event.
        /// Identificador de entidad asociado al evento.
        /// </summary>
        public int EntityId = entityId;

        /// <summary>
        /// Game time for the input event.
        /// Tiempo de juego para el evento de entrada.
        /// </summary>
        public GameTime GameTime = gameTime;

        /// <summary>
        /// Left thumbstick position (-1 to 1 on each axis).
        /// Posición del stick izquierdo (-1 a 1 en cada eje).
        /// </summary>
        public Vector2 LeftThumbstick = leftThumbstick;

        /// <summary>
        /// Right thumbstick position (-1 to 1 on each axis).
        /// Posición del stick derecho (-1 a 1 en cada eje).
        /// </summary>
        public Vector2 RightThumbstick = rightThumbstick;

        /// <summary>
        /// Player index for the gamepad.
        /// Índice del jugador del gamepad.
        /// </summary>
        public PlayerIndex PlayerIndex = playerIndex;
    }


    /// <summary>
    /// Base event for OS input interactions.
    /// Evento que se dispara al usuario interactual con el input del OS
    /// </summary>
    /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
    /// <param name="gameTime">Game time. Tiempo de juego.</param>
    /// <param name="actionEntityInput">Action mapping. Acción mapeada.</param>
    /// <param name="typeEvent">Input event type. Tipo de evento de entrada.</param>
    public abstract class OnInputEvent(int entityId, GameTime gameTime, ActionEntityInput actionEntityInput, InputEventType typeEvent)
    {
        /// <summary>
        /// Game time for the input event.
        /// Tiempo de juego para el evento de entrada.
        /// </summary>
        public GameTime GameTime = gameTime;

        /// <summary>
        /// Input event type.
        /// Tipo de evento de entrada.
        /// </summary>
        public InputEventType Type = typeEvent;

        /// <summary>
        /// Entity identifier associated with the event.
        /// Identificador de entidad asociado al evento.
        /// </summary>
        public int EntityId = entityId;

        /// <summary>
        /// Action mapping for the input.
        /// Mapeo de acción para la entrada.
        /// </summary>
        public ActionEntityInput ActionEntityInput = actionEntityInput;
    }

    /// <summary>
    /// Event published when a world camera is set.
    /// Evento que se dispara al solicitar cambiar la camara del mundo
    /// </summary>
    /// <param name="cameraComponent">Camera component. Componente de cámara.</param>
    public class OnCameraSet(CameraComponent3D cameraComponent)
    {
        /// <summary>
        /// Camera component instance.
        /// Instancia del componente de cámara.
        /// </summary>
        public CameraComponent3D camera = cameraComponent;
    }
    
    /// <summary>
    /// Event published when a button is hovered with the mouse.
    /// Evento que se dispara si se le hace hover con el mouse a un boton
    /// </summary>
    /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
    public class OnHoverButton(int entityId)
    {
        /// <summary>
        /// Hovered entity identifier.
        /// Identificador de la entidad en hover.
        /// </summary>
        public int entityId = entityId;
    }

    /// <summary>
    /// Event published to pause event processing temporarily.
    /// Evento que se dispara para pausar momentaneamente la resolucion de eventos del EventManager
    /// </summary>
    public class StopEvents(bool ignoreEventsInProccess = true) 
    { 
        /// <summary>
        /// Indicates whether events should be ignored during processing.
        /// Indica si los eventos deben ignorarse durante el procesamiento.
        /// </summary>
        public bool ignoreEventsInProccess { get; set; }
    }

    namespace EngineEvents
    {
        /// <summary>
        /// Engine event published when a history version is restored.
        /// Evento propio del motor que se dispara al cambiar de version del historial del gamefile
        /// </summary>
        /// <param name="nameVersion">Version name. Nombre de versión.</param>
        public class OnGameFileWasReplaceByHistory(string nameVersion)
        {
            /// <summary>
            /// Version name restored.
            /// Nombre de la versión restaurada.
            /// </summary>
            public string NameVersion = nameVersion;
        }


#if YTB
        /// <summary>
        /// Event toggling the engine editor UI visibility.
        /// Evento que alterna la visibilidad de la UI del editor.
        /// </summary>
        public class OnHiddeORShowUIEngineEditor() { }

        /// <summary>
        /// Event toggling the game UI visibility.
        /// Evento que alterna la visibilidad de la UI del juego.
        /// </summary>
        public class OnHiddeORShowGameUI() { }

        /// <summary>
        /// Event specifying visibility for game UI and engine editor UI.
        /// Evento que especifica la visibilidad de la UI del juego y del editor.
        /// </summary>
        public class OnShowGameUIHiddeEngineEditor(bool showGameUI, bool showEngineEditor)
        {
            /// <summary>
            /// Whether to show the game UI.
            /// Si se muestra la UI del juego.
            /// </summary>
            public bool ShowGameUI = showGameUI;

            /// <summary>
            /// Whether to show the engine editor UI.
            /// Si se muestra la UI del editor del motor.
            /// </summary>
            public bool ShowEngineEditor = showEngineEditor;
        }

        /// <summary>
        /// Event published when the scene manager changes.
        /// Evento publicado cuando cambia el gestor de escenas.
        /// </summary>
        public class OnChangeEsceneManager(SceneManager sceneManager)
        {
            /// <summary>
            /// Scene manager instance.
            /// Instancia del gestor de escenas.
            /// </summary>
            public SceneManager SceneManager = sceneManager;
        }
#endif
    }
} 
