using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace YotsubaEngine.Input
{
    /// <summary>
    /// Tracks gamepad input state across frames.
    /// Controla el estado del gamepad entre frames.
    /// </summary>
    public class GamePadInfo
    {
        private TimeSpan _vibrationTimeRemaining = TimeSpan.Zero;

        /// <summary>
        /// Gets the index of the player this gamepad is for.
        /// Obtiene el índice del jugador asociado a este gamepad.
        /// </summary>
        public PlayerIndex PlayerIndex { get; }
        /// <summary>
        /// Gets the state of input for this gamepad during the previous update cycle.
        /// Obtiene el estado del gamepad durante el ciclo de actualización anterior.
        /// </summary>
        public GamePadState PreviousState { get; private set; }

        /// <summary>
        /// Gets the state of input for this gamepad during the current update cycle.
        /// Obtiene el estado del gamepad durante el ciclo de actualización actual.
        /// </summary>
        public GamePadState CurrentState { get; private set; }

        /// <summary>
        /// Gets a value that indicates if this gamepad is currently connected.
        /// Obtiene un valor que indica si el gamepad está conectado.
        /// </summary>
        public bool IsConnected => CurrentState.IsConnected;
        /// <summary>
        /// Gets the value of the left thumbstick of this gamepad.
        /// Obtiene el valor del stick izquierdo del gamepad.
        /// </summary>
        public Vector2 LeftThumbStick => CurrentState.ThumbSticks.Left;

        /// <summary>
        /// Gets the value of the right thumbstick of this gamepad.
        /// Obtiene el valor del stick derecho del gamepad.
        /// </summary>
        public Vector2 RightThumbStick => CurrentState.ThumbSticks.Right;

        /// <summary>
        /// Gets the value of the left trigger of this gamepad.
        /// Obtiene el valor del gatillo izquierdo del gamepad.
        /// </summary>
        public float LeftTrigger => CurrentState.Triggers.Left;

        /// <summary>
        /// Gets the value of the right trigger of this gamepad.
        /// Obtiene el valor del gatillo derecho del gamepad.
        /// </summary>
        public float RightTrigger => CurrentState.Triggers.Right;

        /// <summary>
        /// Creates a new GamePadInfo for the gamepad connected at the specified player index.
        /// Crea un nuevo GamePadInfo para el gamepad conectado al índice indicado.
        /// </summary>
        /// <param name="playerIndex">The index of the player for this gamepad. El índice del jugador para este gamepad.</param>
        public GamePadInfo(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
            PreviousState = new GamePadState();
            CurrentState = GamePad.GetState(playerIndex);
        }

        /// <summary>
        /// Updates the state information for this gamepad input.
        /// Actualiza la información de estado del gamepad.
        /// </summary>
        /// <param name="gameTime">Elapsed game time. Tiempo de juego transcurrido.</param>
        public void Update(GameTime gameTime)
        {
            PreviousState = CurrentState;
            CurrentState = GamePad.GetState(PlayerIndex);

            if (_vibrationTimeRemaining > TimeSpan.Zero)
            {
                _vibrationTimeRemaining -= gameTime.ElapsedGameTime;

                if (_vibrationTimeRemaining <= TimeSpan.Zero)
                {
                    StopVibration();
                }
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the specified gamepad button is current down.
        /// Devuelve un valor que indica si el botón está presionado.
        /// </summary>
        /// <param name="button">The gamepad button to check. El botón del gamepad a comprobar.</param>
        /// <returns>true if the specified gamepad button is currently down; otherwise, false. true si el botón está presionado; de lo contrario, false.</returns>
        public bool IsButtonDown(Buttons button)
        {
            return CurrentState.IsButtonDown(button);
        }

        /// <summary>
        /// Returns a value that indicates whether the specified gamepad button is currently up.
        /// Devuelve un valor que indica si el botón está suelto.
        /// </summary>
        /// <param name="button">The gamepad button to check. El botón del gamepad a comprobar.</param>
        /// <returns>true if the specified gamepad button is currently up; otherwise, false. true si el botón está suelto; de lo contrario, false.</returns>
        public bool IsButtonUp(Buttons button)
        {
            return CurrentState.IsButtonUp(button);
        }

        /// <summary>
        /// Returns a value that indicates whether the specified gamepad button was just pressed on the current frame.
        /// Devuelve un valor que indica si el botón se presionó en este frame.
        /// </summary>
        /// <param name="button">The gamepad button to check. El botón del gamepad a comprobar.</param>
        /// <returns>true if the specified gamepad button was just pressed on the current frame; otherwise, false. true si el botón se presionó en este frame; de lo contrario, false.</returns>
        public bool WasButtonJustPressed(Buttons button)
        {
            return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
        }

        /// <summary>
        /// Returns a value that indicates whether the specified gamepad button was just released on the current frame.
        /// Devuelve un valor que indica si el botón se soltó en este frame.
        /// </summary>
        /// <param name="button">The gamepad button to check. El botón del gamepad a comprobar.</param>
        /// <returns>true if the specified gamepad button was just released on the current frame; otherwise, false. true si el botón se soltó en este frame; de lo contrario, false.</returns>
        public bool WasButtonJustReleased(Buttons button)
        {
            return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
        }

        /// <summary>
        /// Sets the vibration for all motors of this gamepad.
        /// Establece la vibración para todos los motores del gamepad.
        /// </summary>
        /// <param name="strength">The strength of the vibration from 0.0f (none) to 1.0f (full). La fuerza de vibración de 0.0f a 1.0f.</param>
        /// <param name="time">The amount of time the vibration should occur. El tiempo durante el que la vibración debe ocurrir.</param>
        public void SetVibration(float strength, TimeSpan time)
        {
            _vibrationTimeRemaining = time;
            GamePad.SetVibration(PlayerIndex, strength, strength);
        }

        /// <summary>
        /// Stops the vibration of all motors for this gamepad.
        /// Detiene la vibración de todos los motores del gamepad.
        /// </summary>
        public void StopVibration()
        {
            GamePad.SetVibration(PlayerIndex, 0.0f, 0.0f);
        }



    }
}
