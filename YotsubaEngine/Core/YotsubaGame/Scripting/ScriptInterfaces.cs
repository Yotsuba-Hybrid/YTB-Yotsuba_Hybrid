
// -----------------------------------------------------------------------
// DEFINICIÓN DE INTERFACES
// -----------------------------------------------------------------------

using YotsubaEngine.Events.YTBEvents;

namespace YotsubaEngine.Core.YotsubaGame.Scripting
{
    /// <summary>
    /// Interfaz para escuchar eventos relacionados con el sistema de animación.
    /// Implementa esta interfaz si tu script necesita reaccionar a cambios de frames o estados.
    /// </summary>
    public interface IAnimationListener
    {
        /// <summary>
        /// Se llama cuando una animación configurada como "No Loop" termina su reproducción.
        /// </summary>
        /// <param name="event">Datos del evento de finalización.</param>
        void OnAnimationDontLoopRelease(OnAnimationDontLoopReleaseEvent @event);

        /// <summary>
        /// Se llama cuando ocurre un cambio explícito de animación en la entidad.
        /// </summary>
        /// <param name="event">Datos del evento de cambio.</param>
        void OnAnimationChange(AnimationChangeEvent @event);
    }

    /// <summary>
    /// Interfaz para escuchar eventos de física y colisiones.
    /// </summary>
    public interface ICollisionListener
    {
        /// <summary>
        /// Se ejecuta cuando la entidad colisiona físicamente con otra entidad.
        /// </summary>
        /// <param name="event">Datos de la colisión (quién chocó con quién).</param>
        void OnCollision(OnCollitionEvent @event);
    }

    /// <summary>
    /// Interfaz para escuchar eventos de entrada del Teclado.
    /// </summary>
    public interface IKeyboardListener
    {
        /// <summary>
        /// Se ejecuta cuando se detecta una pulsación o liberación de tecla asignada a esta entidad.
        /// </summary>
        /// <param name="event">Datos del input de teclado.</param>
        void OnKeyboardInput(OnKeyBoardEvent @event);
    }

    /// <summary>
    /// Interfaz para escuchar eventos de entrada del Mouse.
    /// </summary>
    public interface IMouseListener
    {
        /// <summary>
        /// Se ejecuta cuando se detecta una acción del mouse (clic, movimiento) asignada a esta entidad.
        /// </summary>
        /// <param name="event">Datos del input de mouse.</param>
        void OnMouseInput(OnMouseEvent @event);
    }

    /// <summary>
    /// Interfaz para escuchar eventos de entrada de GamePad (Control de mando).
    /// </summary>
    public interface IGamePadListener
    {
        /// <summary>
        /// Se ejecuta cuando se detecta input de un mando conectado.
        /// </summary>
        /// <param name="event">Datos del input de GamePad.</param>
        void OnGamePadInput(OnGamePadEvent @event);
    }

    /// <summary>
    /// Interfaz para escuchar eventos de entrada del thumbstick del GamePad.
    /// Interface to listen for GamePad thumbstick input events.
    /// </summary>
    public interface IThumbstickListener
    {
        /// <summary>
        /// Se ejecuta cuando se detecta movimiento del thumbstick del mando.
        /// Called when thumbstick movement is detected from a connected gamepad.
        /// </summary>
        /// <param name="event">Datos del input del thumbstick. Thumbstick input data.</param>
        void OnThumbstickInput(OnThumbstickEvent @event);
    }

    /// <summary>
    /// Interfaz para elementos de UI que reaccionan cuando el cursor pasa por encima.
    /// </summary>
    public interface IHoverListener
    {
        /// <summary>
        /// Se ejecuta cuando el puntero del mouse entra en el área de la entidad (Hover).
        /// </summary>
        /// <param name="event">Datos del evento hover.</param>
        void OnHover(OnHoverButton @event);
    }
}