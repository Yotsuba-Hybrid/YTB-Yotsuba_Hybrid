
// -----------------------------------------------------------------------
// DEFINICIÓN DE INTERFACES
// -----------------------------------------------------------------------

using YotsubaEngine.Events.YTBEvents;

namespace YotsubaEngine.Core.YotsubaGame.Scripting
{
    /// <summary>
    /// Interfaz para escuchar eventos relacionados con el sistema de animación.
    /// <para>Interface to listen for animation system events.</para>
    /// </summary>
    public interface IAnimationListener
    {
        /// <summary>
        /// Se llama cuando una animación configurada como "No Loop" termina su reproducción.
        /// <para>Called when a "No Loop" animation finishes playing.</para>
        /// </summary>
        /// <param name="event">Datos del evento de finalización. <para>Completion event data.</para></param>
        void OnAnimationDontLoopRelease(OnAnimationDontLoopReleaseEvent @event);

        /// <summary>
        /// Se llama cuando ocurre un cambio explícito de animación en la entidad.
        /// <para>Called when an explicit animation change occurs on the entity.</para>
        /// </summary>
        /// <param name="event">Datos del evento de cambio. <para>Change event data.</para></param>
        void OnAnimationChange(AnimationChangeEvent @event);
    }

    /// <summary>
    /// Interfaz para escuchar eventos de física y colisiones.
    /// <para>Interface to listen for physics and collision events.</para>
    /// </summary>
    public interface ICollisionListener
    {
        /// <summary>
        /// Se ejecuta cuando la entidad colisiona físicamente con otra entidad.
        /// <para>Called when the entity physically collides with another entity.</para>
        /// </summary>
        /// <param name="event">Datos de la colisión. <para>Collision data.</para></param>
        void OnCollision(OnCollitionEvent @event);
    }

    /// <summary>
    /// Interfaz para escuchar eventos de entrada del teclado.
    /// <para>Interface to listen for keyboard input events.</para>
    /// </summary>
    public interface IKeyboardListener
    {
        /// <summary>
        /// Se ejecuta cuando se detecta una pulsación o liberación de tecla asignada a esta entidad.
        /// <para>Called when a key press or release assigned to this entity is detected.</para>
        /// </summary>
        /// <param name="event">Datos del input de teclado. <para>Keyboard input data.</para></param>
        void OnKeyboardInput(OnKeyBoardEvent @event);
    }

    /// <summary>
    /// Interfaz para escuchar eventos de entrada del mouse.
    /// <para>Interface to listen for mouse input events.</para>
    /// </summary>
    public interface IMouseListener
    {
        /// <summary>
        /// Se ejecuta cuando se detecta una acción del mouse (clic, movimiento) asignada a esta entidad.
        /// <para>Called when a mouse action (click, movement) assigned to this entity is detected.</para>
        /// </summary>
        /// <param name="event">Datos del input de mouse. <para>Mouse input data.</para></param>
        void OnMouseInput(OnMouseEvent @event);
    }

    /// <summary>
    /// Interfaz para escuchar eventos de entrada de GamePad (control de mando).
    /// <para>Interface to listen for gamepad input events.</para>
    /// </summary>
    public interface IGamePadListener
    {
        /// <summary>
        /// Se ejecuta cuando se detecta input de un mando conectado.
        /// <para>Called when input from a connected gamepad is detected.</para>
        /// </summary>
        /// <param name="event">Datos del input de GamePad. <para>Gamepad input data.</para></param>
        void OnGamePadInput(OnGamePadEvent @event);
    }

    /// <summary>
    /// Interfaz para escuchar eventos de entrada del thumbstick del GamePad.
    /// <para>Interface to listen for GamePad thumbstick input events.</para>
    /// </summary>
    public interface IThumbstickListener
    {
        /// <summary>
        /// Se ejecuta cuando se detecta movimiento del thumbstick del mando.
        /// <para>Called when thumbstick movement is detected from a connected gamepad.</para>
        /// </summary>
        /// <param name="event">Datos del input del thumbstick. <para>Thumbstick input data.</para></param>
        void OnThumbstickInput(OnThumbstickEvent @event);
    }

    /// <summary>
    /// Interfaz para elementos de UI que reaccionan cuando el cursor pasa por encima.
    /// <para>Interface for UI elements that react when the cursor hovers over them.</para>
    /// </summary>
    public interface IHoverListener
    {
        /// <summary>
        /// Se ejecuta cuando el puntero del mouse entra en el área de la entidad (Hover).
        /// <para>Called when the mouse pointer enters the entity area (hover).</para>
        /// </summary>
        /// <param name="event">Datos del evento hover. <para>Hover event data.</para></param>
        void OnHover(OnHoverButton @event);
    }
}
