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
    /// Evento que se publica para cambiar la animación de una entidad.
    /// <para>Event published to change an entity animation.</para>
    /// </summary>
    /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
    /// <param name="animation">Tipo de animación. <para>Animation type.</para></param>
    public class AnimationChangeEvent(int entityId, AnimationType animation)
    {
        /// <summary>
        /// Identificador de la entidad objetivo.
        /// <para>Target entity identifier.</para>
        /// </summary>
        public int EntityId = entityId;

        /// <summary>
        /// Nombre de la animación a aplicar.
        /// <para>Animation name to apply.</para>
        /// </summary>
        public AnimationType AnimationName = animation;
    }

    /// <summary>
    /// Evento que se publica al final del renderizado de cada frame.
    /// <para>Event published after each frame render.</para>
    /// </summary>
    public class OnFrameRenderEvent
    {
        /// <summary>
        /// Tiempo de juego para el frame renderizado.
        /// <para>Game time for the rendered frame.</para>
        /// </summary>
        public GameTime gameTime;
    }

	/// <summary>
	/// Evento que se publica cuando una animación sin loop termina de reproducirse.
	/// <para>Event published when a non-looping animation finishes.</para>
	/// </summary>
	public class OnAnimationDontLoopReleaseEvent
	{
        /// <summary>
        /// Entidad que terminó la animación.
        /// <para>Entity that finished the animation.</para>
        /// </summary>
        public int EntityId;

        /// <summary>
        /// Nombre de la animación que terminó.
        /// <para>Animation name that finished.</para>
        /// </summary>
        public AnimationType AnimationName;
	}

	/// <summary>
	/// Evento que se publica si dos entidades colisionaron.
	/// <para>Event published when two entities collide.</para>
	/// </summary>
	public class OnCollitionEvent
    {
        /// <summary>
        /// Tiempo de juego para la colisión.
        /// <para>Game time for the collision.</para>
        /// </summary>
        public GameTime GameTime;

        /// <summary>
        /// Entidad que intenta moverse.
        /// <para>Entity attempting to move.</para>
        /// </summary>
        public Yotsuba EntityTryMove;

        /// <summary>
        /// Entidad que actúa como obstáculo.
        /// <para>Entity acting as obstacle.</para>
        /// </summary>
        public Yotsuba EntityImpediment;

    }

    /// <summary>
    /// Dirección de colisión para eventos de física.
    /// <para>Collision direction for physics events.</para>
    /// </summary>
    public enum CollisionDirection
    {
        /// <summary>
        /// Colisión desde arriba (aterrizaje en el suelo).
        /// <para>Collision from above (landing on ground).</para>
        /// </summary>
        Bottom,
        /// <summary>
        /// Colisión desde abajo (golpeando el techo).
        /// <para>Collision from below (hitting ceiling).</para>
        /// </summary>
        Top,
        /// <summary>
        /// Colisión desde el lado izquierdo.
        /// <para>Collision from the left side.</para>
        /// </summary>
        Left,
        /// <summary>
        /// Colisión desde el lado derecho.
        /// <para>Collision from the right side.</para>
        /// </summary>
        Right
    }

    /// <summary>
    /// Evento que se publica cuando una entidad aterriza en el suelo (modo Platform).
    /// <para>Event published when an entity lands on the ground (Platform mode).</para>
    /// </summary>
    public class OnEntityGroundedEvent
    {
        /// <summary>
        /// Entidad que aterrizó.
        /// <para>Entity that landed.</para>
        /// </summary>
        public int EntityId;

        /// <summary>
        /// Tiempo de juego del evento.
        /// <para>Game time for the event.</para>
        /// </summary>
        public GameTime GameTime;
    }

    /// <summary>
    /// Evento que se publica cuando una entidad deja el suelo (modo Platform).
    /// <para>Event published when an entity leaves the ground (Platform mode).</para>
    /// </summary>
    public class OnEntityAirborneEvent
    {
        /// <summary>
        /// Entidad que dejó el suelo.
        /// <para>Entity that left the ground.</para>
        /// </summary>
        public int EntityId;

        /// <summary>
        /// Tiempo de juego del evento.
        /// <para>Game time for the event.</para>
        /// </summary>
        public GameTime GameTime;
    }

    /// <summary>
    /// Evento que se publica cuando una entidad salta (modo Platform).
    /// <para>Event published when an entity jumps (Platform mode).</para>
    /// </summary>
    public class OnEntityJumpEvent
    {
        /// <summary>
        /// Entidad que saltó.
        /// <para>Entity that jumped.</para>
        /// </summary>
        public int EntityId;

        /// <summary>
        /// Tiempo de juego del evento.
        /// <para>Game time for the event.</para>
        /// </summary>
        public GameTime GameTime;
    }

    /// <summary>
    /// Evento que se publica cuando se cambia el estado del teclado.
    /// <para>Event published when keyboard state changes.</para>
    /// </summary>
    /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
    /// <param name="key">Tecla del teclado. <para>Keyboard key.</para></param>
    /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
    /// <param name="actionEntityInput">Acción mapeada. <para>Action mapping.</para></param>
    /// <param name="typeEvent">Tipo de evento de entrada. <para>Input event type.</para></param>
    public class OnKeyBoardEvent(int entityId, Keys key, GameTime gameTime, ActionEntityInput actionEntityInput, InputEventType typeEvent) : OnInputEvent(entityId, gameTime, actionEntityInput, typeEvent)
    {
        /// <summary>
        /// Tecla asociada al evento.
        /// <para>Key associated with the event.</para>
        /// </summary>
        public Keys Key = key;
    }

    /// <summary>
    /// Define el tipo de evento de entrada.
    /// <para>Defines the kind of input event.</para>
    /// </summary>
    public enum InputEventType
    {
        /// <summary>
        /// La entrada se soltó recientemente.
        /// <para>Input was just released.</para>
        /// </summary>
        JustReleased,
        /// <summary>
        /// La entrada se presionó recientemente.
        /// <para>Input was just pressed.</para>
        /// </summary>
        JustPressed,
        /// <summary>
        /// La entrada se mantiene presionada.
        /// <para>Input is held down.</para>
        /// </summary>
        HoldDown
    }

    /// <summary>
    /// Evento que se dispara al cambiar el estado del mouse.
    /// <para>Event published when mouse state changes.</para>
    /// </summary>
    /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
    /// <param name="mouseButton">Botón del mouse. <para>Mouse button.</para></param>
    /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
    /// <param name="actionEntityInput">Acción mapeada. <para>Action mapping.</para></param>
    /// <param name="typeEvent">Tipo de evento de entrada. <para>Input event type.</para></param>
    public class OnMouseEvent(int entityId, MouseButton mouseButton, GameTime gameTime, ActionEntityInput actionEntityInput, InputEventType typeEvent) : OnInputEvent(entityId, gameTime, actionEntityInput, typeEvent)
    {
        /// <summary>
        /// Botón del mouse asociado al evento.
        /// <para>Mouse button associated with the event.</para>
        /// </summary>
        public MouseButton MouseButton = mouseButton;
    }

    /// <summary>
    /// Evento que se dispara al cambiar el estado del mando.
    /// <para>Event published when gamepad state changes.</para>
    /// </summary>
    /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
    /// <param name="gamePadButton">Botón del gamepad. <para>Gamepad button.</para></param>
    /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
    /// <param name="actionEntityInput">Acción mapeada. <para>Action mapping.</para></param>
    /// <param name="typeEvent">Tipo de evento de entrada. <para>Input event type.</para></param>
    public class OnGamePadEvent(int entityId, Buttons gamePadButton, GameTime gameTime, ActionEntityInput actionEntityInput, InputEventType typeEvent) : OnInputEvent(entityId, gameTime, actionEntityInput, typeEvent)
    {
        /// <summary>
        /// Botón del gamepad asociado al evento.
        /// <para>Gamepad button associated with the event.</para>
        /// </summary>
        public Buttons Button = gamePadButton;
    }

    /// <summary>
    /// Evento que se dispara al mover el thumbstick del gamepad.
    /// <para>Event published when gamepad thumbstick position changes.</para>
    /// </summary>
    /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
    /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
    /// <param name="leftThumbstick">Posición del stick izquierdo. <para>Left thumbstick position.</para></param>
    /// <param name="rightThumbstick">Posición del stick derecho. <para>Right thumbstick position.</para></param>
    /// <param name="playerIndex">Índice del jugador del gamepad. <para>Player index for the gamepad.</para></param>
    public class OnThumbstickEvent(int entityId, GameTime gameTime, Vector2 leftThumbstick, Vector2 rightThumbstick, PlayerIndex playerIndex)
    {
        /// <summary>
        /// Identificador de entidad asociado al evento.
        /// <para>Entity identifier associated with the event.</para>
        /// </summary>
        public int EntityId = entityId;

        /// <summary>
        /// Tiempo de juego para el evento de entrada.
        /// <para>Game time for the input event.</para>
        /// </summary>
        public GameTime GameTime = gameTime;

        /// <summary>
        /// Posición del stick izquierdo (-1 a 1 en cada eje).
        /// <para>Left thumbstick position (-1 to 1 on each axis).</para>
        /// </summary>
        public Vector2 LeftThumbstick = leftThumbstick;

        /// <summary>
        /// Posición del stick derecho (-1 a 1 en cada eje).
        /// <para>Right thumbstick position (-1 to 1 on each axis).</para>
        /// </summary>
        public Vector2 RightThumbstick = rightThumbstick;

        /// <summary>
        /// Índice del jugador del gamepad.
        /// <para>Player index for the gamepad.</para>
        /// </summary>
        public PlayerIndex PlayerIndex = playerIndex;
    }


    /// <summary>
    /// Evento que se dispara al usuario interactuar con el input del OS.
    /// <para>Base event for OS input interactions.</para>
    /// </summary>
    /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
    /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
    /// <param name="actionEntityInput">Acción mapeada. <para>Action mapping.</para></param>
    /// <param name="typeEvent">Tipo de evento de entrada. <para>Input event type.</para></param>
    public abstract class OnInputEvent(int entityId, GameTime gameTime, ActionEntityInput actionEntityInput, InputEventType typeEvent)
    {
        /// <summary>
        /// Tiempo de juego para el evento de entrada.
        /// <para>Game time for the input event.</para>
        /// </summary>
        public GameTime GameTime = gameTime;

        /// <summary>
        /// Tipo de evento de entrada.
        /// <para>Input event type.</para>
        /// </summary>
        public InputEventType Type = typeEvent;

        /// <summary>
        /// Identificador de entidad asociado al evento.
        /// <para>Entity identifier associated with the event.</para>
        /// </summary>
        public int EntityId = entityId;

        /// <summary>
        /// Mapeo de acción para la entrada.
        /// <para>Action mapping for the input.</para>
        /// </summary>
        public ActionEntityInput ActionEntityInput = actionEntityInput;
    }

    /// <summary>
    /// Evento que se dispara al solicitar cambiar la cámara del mundo.
    /// <para>Event published when a world camera is set.</para>
    /// </summary>
    /// <param name="cameraComponent">Componente de cámara. <para>Camera component.</para></param>
    public class OnCameraSet(CameraComponent3D cameraComponent)
    {
        /// <summary>
        /// Instancia del componente de cámara.
        /// <para>Camera component instance.</para>
        /// </summary>
        public CameraComponent3D camera = cameraComponent;
    }
    
    /// <summary>
    /// Evento que se dispara si se le hace hover con el mouse a un botón.
    /// <para>Event published when a button is hovered with the mouse.</para>
    /// </summary>
    /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
    public class OnHoverButton(int entityId)
    {
        /// <summary>
        /// Identificador de la entidad en hover.
        /// <para>Hovered entity identifier.</para>
        /// </summary>
        public int entityId = entityId;
    }

    /// <summary>
    /// Evento que se dispara para pausar momentáneamente la resolución de eventos del EventManager.
    /// <para>Event published to pause event processing temporarily.</para>
    /// </summary>
    /// <param name="ignoreEventsInProccess">Indica si se deben ignorar eventos durante el procesamiento. <para>Whether to ignore events during processing.</para></param>
    public class StopEvents(bool ignoreEventsInProccess = true) 
    { 
        /// <summary>
        /// Indica si los eventos deben ignorarse durante el procesamiento.
        /// <para>Indicates whether events should be ignored during processing.</para>
        /// </summary>
        public bool ignoreEventsInProccess { get; set; }
    }

    namespace EngineEvents
    {
        /// <summary>
        /// Evento propio del motor que se dispara al cambiar de versión del historial del gamefile.
        /// <para>Engine event published when a history version is restored.</para>
        /// </summary>
        /// <param name="nameVersion">Nombre de versión. <para>Version name.</para></param>
        public class OnGameFileWasReplaceByHistory(string nameVersion)
        {
            /// <summary>
            /// Nombre de la versión restaurada.
            /// <para>Version name restored.</para>
            /// </summary>
            public string NameVersion = nameVersion;
        }


#if YTB
        /// <summary>
        /// Evento que alterna la visibilidad de la UI del editor.
        /// <para>Event toggling the engine editor UI visibility.</para>
        /// </summary>
        public class OnHiddeORShowUIEngineEditor() { }

        /// <summary>
        /// Evento que alterna la visibilidad de la UI del juego.
        /// <para>Event toggling the game UI visibility.</para>
        /// </summary>
        public class OnHiddeORShowGameUI() { }

        /// <summary>
        /// Evento que especifica la visibilidad de la UI del juego y del editor.
        /// <para>Event specifying visibility for game UI and engine editor UI.</para>
        /// </summary>
        /// <param name="showGameUI">Si se muestra la UI del juego. <para>Whether to show the game UI.</para></param>
        /// <param name="showEngineEditor">Si se muestra la UI del editor del motor. <para>Whether to show the engine editor UI.</para></param>
        public class OnShowGameUIHiddeEngineEditor(bool showGameUI, bool showEngineEditor)
        {
            /// <summary>
            /// Si se muestra la UI del juego.
            /// <para>Whether to show the game UI.</para>
            /// </summary>
            public bool ShowGameUI = showGameUI;

            /// <summary>
            /// Si se muestra la UI del editor del motor.
            /// <para>Whether to show the engine editor UI.</para>
            /// </summary>
            public bool ShowEngineEditor = showEngineEditor;
        }

        /// <summary>
        /// Evento publicado cuando cambia el gestor de escenas.
        /// <para>Event published when the scene manager changes.</para>
        /// </summary>
        /// <param name="sceneManager">Instancia del gestor de escenas. <para>Scene manager instance.</para></param>
        public class OnChangeEsceneManager(SceneManager sceneManager)
        {
            /// <summary>
            /// Instancia del gestor de escenas.
            /// <para>Scene manager instance.</para>
            /// </summary>
            public SceneManager SceneManager = sceneManager;
        }
#endif
    }
} 
