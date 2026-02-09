using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace YotsubaEngine.Input
{
    /// <summary>
    /// Provides all relevant information about touch input.
    /// Clase que nos da toda la información relevante sobre la entrada táctil (touch).
    /// Está diseñada para ser usada de forma similar a MouseInfo / KeyboardInfo.
    /// </summary>
    public class TouchInfo
    {
        /// <summary>
        /// The state of touch input during the previous update cycle.
        /// El estado del touch durante el ciclo de actualización anterior.
        /// </summary>
        public TouchCollection PreviousState { get; private set; }

        /// <summary>
        /// The state of touch input during the current update cycle.
        /// El estado del touch durante el ciclo de actualización actual.
        /// </summary>
        public TouchCollection CurrentState { get; private set; }

        /// <summary>
        /// Number of active touches.
        /// Número de toques actuales.
        /// </summary>
        public int TouchCount => CurrentState.Count;

        /// <summary>
        /// True if there is at least one touch in the current state.
        /// True si hay al menos un touch en el estado actual.
        /// </summary>
        public bool AnyTouch => TouchCount > 0;

        /// <summary>
        /// Creates a new TouchInfo instance.
        /// Crea un nuevo TouchInfo.
        /// </summary>
        public TouchInfo()
        {
            PreviousState = new TouchCollection();
            CurrentState = TouchPanel.GetState();
        }

        /// <summary>
        /// Updates the touch state (should be called every frame).
        /// Actualiza la información del touch (debe llamarse cada frame).
        /// </summary>
        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = TouchPanel.GetState();
        }

        /// <summary>
        /// Returns all current touches.
        /// Devuelve todos los touch actuales.
        /// </summary>
        public TouchCollection GetTouches()
        {
            return CurrentState;
        }

        /// <summary>
        /// Attempts to get a touch by its id.
        /// Intenta obtener un touch por su id.
        /// </summary>
        /// <param name="id">Touch id (TouchLocation.Id). Id del touch (TouchLocation.Id).</param>
        /// <param name="touch">Output touch if it exists. Salida con el touch si existe.</param>
        /// <returns>true if the touch was found in the current state. true si se encontró el touch en el estado actual.</returns>
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
        /// Attempts to get a touch by its id in the previous state.
        /// Intenta obtener un touch por su id en el estado previo.
        /// </summary>
        /// <param name="id">Touch id. Id del touch.</param>
        /// <param name="touch">Output touch if it exists. Salida con el touch si existe.</param>
        /// <returns>true if the touch was found in the previous state. true si se encontró el touch en el estado previo.</returns>
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
        /// Returns all touches that match the specified state.
        /// Devuelve todos los touches que están en el estado indicado.
        /// </summary>
        /// <param name="state">Touch state to filter. Estado de touch a filtrar.</param>
        /// <returns>Touches that match the state. Touches que coinciden con el estado.</returns>
        public IEnumerable<TouchLocation> GetTouchesByState(TouchLocationState state)
        {
            return CurrentState.Where(t => t.State == state);
        }

        /// <summary>
        /// Indicates if a touch with the given id is active (Pressed or Moved).
        /// Indica si un touch con el id dado está "activo" (Pressed o Moved).
        /// </summary>
        /// <param name="id">Touch id. Id del touch.</param>
        /// <returns>True if active. True si está activo.</returns>
        public bool IsTouchActive(int id)
        {
            if (TryGetTouch(id, out var t))
            {
                return t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved;
            }
            return false;
        }

        /// <summary>
        /// Indicates if a touch was just pressed this frame.
        /// Indica si un touch fue pulsado justo en este frame.
        /// (Existe en Current con Pressed y en Previous no existía o estaba Released/Invalid).
        /// </summary>
        /// <param name="id">Touch id. Id del touch.</param>
        /// <returns>True if just pressed. True si se presionó.</returns>
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
        /// Indicates if a touch was released this frame.
        /// Indica si un touch fue liberado justo en este frame.
        /// (En Current no existe o está Released y en Previous existía como Pressed/Moved).
        /// </summary>
        /// <param name="id">Touch id. Id del touch.</param>
        /// <returns>True if just released. True si se soltó.</returns>
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
        /// Gets the current touch position, or Vector2.Zero if not found.
        /// Obtiene la posición actual del touch. Si no existe devuelve Vector2.Zero.
        /// </summary>
        /// <param name="id">Touch id. Id del touch.</param>
        /// <returns>Current touch position. Posición actual del touch.</returns>
        public Vector2 GetTouchPosition(int id)
        {
            if (TryGetTouch(id, out var t))
                return t.Position;
            return Vector2.Zero;
        }

        /// <summary>
        /// Gets the previous touch position, or Vector2.Zero if not found.
        /// Obtiene la posición previa del touch. Si no existe devuelve Vector2.Zero.
        /// </summary>
        /// <param name="id">Touch id. Id del touch.</param>
        /// <returns>Previous touch position. Posición previa del touch.</returns>
        public Vector2 GetPreviousTouchPosition(int id)
        {
            if (TryGetPreviousTouch(id, out var t))
                return t.Position;
            return Vector2.Zero;
        }

        /// <summary>
        /// Returns the position delta (current - previous) for the specified touch.
        /// Devuelve la diferencia de posición (current - previous) para el touch indicado.
        /// Si no hay datos devuelve Vector2.Zero.
        /// </summary>
        /// <param name="id">Touch id. Id del touch.</param>
        /// <returns>Position delta. Diferencia de posición.</returns>
        public Vector2 GetTouchPositionDelta(int id)
        {
            if (TryGetTouch(id, out var curr) && TryGetPreviousTouch(id, out var prev))
            {
                return curr.Position - prev.Position;
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Indicates if the touch moved between frames.
        /// Indica si el touch cambió de posición entre frames.
        /// </summary>
        /// <param name="id">Touch id. Id del touch.</param>
        /// <returns>True if moved. True si se movió.</returns>
        public bool WasTouchMoved(int id)
        {
            if (TryGetTouch(id, out var curr) && TryGetPreviousTouch(id, out var prev))
            {
                return curr.Position != prev.Position;
            }
            return false;
        }

        /// <summary>
        /// Returns the first primary touch if it exists.
        /// Devuelve el primer touch "primario" si existe (el primero en la colección), 
        /// o null (nullable) si no hay ninguno.
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
