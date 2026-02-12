using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace YotsubaEngine.Input
{
    /// <summary>
    /// Clase que nos da toda la información relevante sobre la entrada táctil (touch) y está diseñada para usarse de forma similar a MouseInfo / KeyboardInfo.
    /// <para>Provides all relevant information about touch input and is designed to be used similarly to MouseInfo / KeyboardInfo.</para>
    /// </summary>
    public class TouchInfo
    {
        /// <summary>
        /// El estado del touch durante el ciclo de actualización anterior.
        /// <para>The state of touch input during the previous update cycle.</para>
        /// </summary>
        public TouchCollection PreviousState { get; private set; }

        /// <summary>
        /// El estado del touch durante el ciclo de actualización actual.
        /// <para>The state of touch input during the current update cycle.</para>
        /// </summary>
        public TouchCollection CurrentState { get; private set; }

        /// <summary>
        /// Número de toques actuales.
        /// <para>Number of active touches.</para>
        /// </summary>
        public int TouchCount => CurrentState.Count;

        /// <summary>
        /// True si hay al menos un touch en el estado actual.
        /// <para>True if there is at least one touch in the current state.</para>
        /// </summary>
        public bool AnyTouch => TouchCount > 0;

        /// <summary>
        /// Crea un nuevo TouchInfo.
        /// <para>Creates a new TouchInfo instance.</para>
        /// </summary>
        public TouchInfo()
        {
            PreviousState = new TouchCollection();
            CurrentState = TouchPanel.GetState();
        }

        /// <summary>
        /// Actualiza la información del touch (debe llamarse cada frame).
        /// <para>Updates the touch state (should be called every frame).</para>
        /// </summary>
        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = TouchPanel.GetState();
        }

        /// <summary>
        /// Devuelve todos los touch actuales.
        /// <para>Returns all current touches.</para>
        /// </summary>
        public TouchCollection GetTouches()
        {
            return CurrentState;
        }

        /// <summary>
        /// Intenta obtener un touch por su id.
        /// <para>Attempts to get a touch by its id.</para>
        /// </summary>
        /// <param name="id">Id del touch (TouchLocation.Id). <para>Touch id (TouchLocation.Id).</para></param>
        /// <param name="touch">Salida con el touch si existe. <para>Output touch if it exists.</para></param>
        /// <returns>True si se encontró el touch en el estado actual. <para>True if the touch was found in the current state.</para></returns>
        public bool TryGetTouch(int id, out TouchLocation touch)
        {
            foreach (var t in CurrentState)
            {
                if (t.Id == id)
                {
                    touch = t;
                    return true;
                }
            }
            touch = default;
            return false;
        }

        /// <summary>
        /// Intenta obtener un touch por su id en el estado previo.
        /// <para>Attempts to get a touch by its id in the previous state.</para>
        /// </summary>
        /// <param name="id">Id del touch. <para>Touch id.</para></param>
        /// <param name="touch">Salida con el touch si existe. <para>Output touch if it exists.</para></param>
        /// <returns>True si se encontró el touch en el estado previo. <para>True if the touch was found in the previous state.</para></returns>
        public bool TryGetPreviousTouch(int id, out TouchLocation touch)
        {
            foreach (var t in PreviousState)
            {
                if (t.Id == id)
                {
                    touch = t;
                    return true;
                }
            }
            touch = default;
            return false;
        }

        /// <summary>
        /// Devuelve todos los touches que están en el estado indicado.
        /// <para>Returns all touches that match the specified state.</para>
        /// </summary>
        /// <param name="state">Estado de touch a filtrar. <para>Touch state to filter.</para></param>
        /// <returns>Touches que coinciden con el estado. <para>Touches that match the state.</para></returns>
        public IEnumerable<TouchLocation> GetTouchesByState(TouchLocationState state)
        {
            return CurrentState.Where(t => t.State == state);
        }

        /// <summary>
        /// Indica si un touch con el id dado está "activo" (Pressed o Moved).
        /// <para>Indicates if a touch with the given id is active (Pressed or Moved).</para>
        /// </summary>
        /// <param name="id">Id del touch. <para>Touch id.</para></param>
        /// <returns>True si está activo. <para>True if active.</para></returns>
        public bool IsTouchActive(int id)
        {
            if (TryGetTouch(id, out var t))
            {
                return t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved;
            }
            return false;
        }

        /// <summary>
        /// Indica si un touch fue pulsado justo en este frame (existe en Current con Pressed y en Previous no existía o estaba Released/Invalid).
        /// <para>Indicates if a touch was just pressed this frame (present in Current with Pressed and absent or Released/Invalid in Previous).</para>
        /// </summary>
        /// <param name="id">Id del touch. <para>Touch id.</para></param>
        /// <returns>True si se presionó. <para>True if just pressed.</para></returns>
        public bool WasTouchJustPressed(int id)
        {
            if (TryGetTouch(id, out var curr))
            {
                if (curr.State != TouchLocationState.Pressed)
                    return false;

                if (!TryGetPreviousTouch(id, out var prev))
                    return true; // apareció este frame como Pressed

                return prev.State == TouchLocationState.Released || prev.State == TouchLocationState.Invalid;
            }
            return false;
        }

        /// <summary>
        /// Indica si un touch fue liberado justo en este frame (en Current no existe o está Released y en Previous existía como Pressed/Moved).
        /// <para>Indicates if a touch was released this frame (missing or Released in Current and Pressed/Moved in Previous).</para>
        /// </summary>
        /// <param name="id">Id del touch. <para>Touch id.</para></param>
        /// <returns>True si se soltó. <para>True if just released.</para></returns>
        public bool WasTouchJustReleased(int id)
        {
            bool currExists = TryGetTouch(id, out var curr);
            bool prevExists = TryGetPreviousTouch(id, out var prev);

            if (!currExists && prevExists)
            {
                // dejó de aparecer en Current (posible que TouchPanel no incluya released) -> consideramos liberado
                return prev.State == TouchLocationState.Pressed || prev.State == TouchLocationState.Moved;
            }

            if (currExists && prevExists)
            {
                return curr.State == TouchLocationState.Released && (prev.State == TouchLocationState.Pressed || prev.State == TouchLocationState.Moved);
            }

            return false;
        }

        /// <summary>
        /// Obtiene la posición actual del touch. Si no existe devuelve Vector2.Zero.
        /// <para>Gets the current touch position, or Vector2.Zero if not found.</para>
        /// </summary>
        /// <param name="id">Id del touch. <para>Touch id.</para></param>
        /// <returns>Posición actual del touch. <para>Current touch position.</para></returns>
        public Vector2 GetTouchPosition(int id)
        {
            if (TryGetTouch(id, out var t))
                return t.Position;
            return Vector2.Zero;
        }

        /// <summary>
        /// Obtiene la posición previa del touch. Si no existe devuelve Vector2.Zero.
        /// <para>Gets the previous touch position, or Vector2.Zero if not found.</para>
        /// </summary>
        /// <param name="id">Id del touch. <para>Touch id.</para></param>
        /// <returns>Posición previa del touch. <para>Previous touch position.</para></returns>
        public Vector2 GetPreviousTouchPosition(int id)
        {
            if (TryGetPreviousTouch(id, out var t))
                return t.Position;
            return Vector2.Zero;
        }

        /// <summary>
        /// Devuelve la diferencia de posición (current - previous) para el touch indicado. Si no hay datos devuelve Vector2.Zero.
        /// <para>Returns the position delta (current - previous) for the specified touch, or Vector2.Zero if unavailable.</para>
        /// </summary>
        /// <param name="id">Id del touch. <para>Touch id.</para></param>
        /// <returns>Diferencia de posición. <para>Position delta.</para></returns>
        public Vector2 GetTouchPositionDelta(int id)
        {
            if (TryGetTouch(id, out var curr) && TryGetPreviousTouch(id, out var prev))
            {
                return curr.Position - prev.Position;
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Indica si el touch cambió de posición entre frames.
        /// <para>Indicates if the touch moved between frames.</para>
        /// </summary>
        /// <param name="id">Id del touch. <para>Touch id.</para></param>
        /// <returns>True si se movió. <para>True if moved.</para></returns>
        public bool WasTouchMoved(int id)
        {
            if (TryGetTouch(id, out var curr) && TryGetPreviousTouch(id, out var prev))
            {
                return curr.Position != prev.Position;
            }
            return false;
        }

        /// <summary>
        /// Devuelve el primer touch "primario" si existe (el primero en la colección), o null (nullable) si no hay ninguno.
        /// <para>Returns the first primary touch if it exists (first in the collection), or null if none.</para>
        /// </summary>
        public TouchLocation? PrimaryTouch
        {
            get
            {
                foreach (var t in CurrentState)
                    return t;
                return null;
            }
        }
    }
}
