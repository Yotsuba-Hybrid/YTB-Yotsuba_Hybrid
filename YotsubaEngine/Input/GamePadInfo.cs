using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace YotsubaEngine.Input
{
    /// <summary>
    /// Controla el estado del gamepad entre frames.
    /// <para>Tracks gamepad input state across frames.</para>
    /// </summary>
    public class GamePadInfo
    {
        private TimeSpan _vibrationTimeRemaining = TimeSpan.Zero;

        /// <summary>
        /// Obtiene el índice del jugador asociado a este gamepad.
        /// <para>Gets the index of the player this gamepad is for.</para>
        /// </summary>
        public PlayerIndex PlayerIndex { get; }
        /// <summary>
        /// Obtiene el estado del gamepad durante el ciclo de actualización anterior.
        /// <para>Gets the state of input for this gamepad during the previous update cycle.</para>
        /// </summary>
        public GamePadState PreviousState { get; private set; }

        /// <summary>
        /// Obtiene el estado del gamepad durante el ciclo de actualización actual.
        /// <para>Gets the state of input for this gamepad during the current update cycle.</para>
        /// </summary>
        public GamePadState CurrentState { get; private set; }

        /// <summary>
        /// Obtiene un valor que indica si el gamepad está conectado.
        /// <para>Gets a value that indicates if this gamepad is currently connected.</para>
        /// </summary>
        public bool IsConnected => CurrentState.IsConnected;
        /// <summary>
        /// Obtiene el valor del stick izquierdo del gamepad.
        /// <para>Gets the value of the left thumbstick of this gamepad.</para>
        /// </summary>
        public Vector2 LeftThumbStick => CurrentState.ThumbSticks.Left;

        /// <summary>
        /// Obtiene el valor del stick derecho del gamepad.
        /// <para>Gets the value of the right thumbstick of this gamepad.</para>
        /// </summary>
        public Vector2 RightThumbStick => CurrentState.ThumbSticks.Right;

        /// <summary>
        /// Obtiene el valor del gatillo izquierdo del gamepad.
        /// <para>Gets the value of the left trigger of this gamepad.</para>
        /// </summary>
        public float LeftTrigger => CurrentState.Triggers.Left;

        /// <summary>
        /// Obtiene el valor del gatillo derecho del gamepad.
        /// <para>Gets the value of the right trigger of this gamepad.</para>
        /// </summary>
        public float RightTrigger => CurrentState.Triggers.Right;

        /// <summary>
        /// Crea un nuevo GamePadInfo para el gamepad conectado al índice indicado.
        /// <para>Creates a new GamePadInfo for the gamepad connected at the specified player index.</para>
        /// </summary>
        /// <param name="playerIndex">El índice del jugador para este gamepad. <para>The index of the player for this gamepad.</para></param>
        public GamePadInfo(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
            PreviousState = new GamePadState();
            CurrentState = GamePad.GetState(playerIndex);
        }

        /// <summary>
        /// Actualiza la información de estado del gamepad.
        /// <para>Updates the state information for this gamepad input.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego transcurrido. <para>Elapsed game time.</para></param>
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
        /// Devuelve un valor que indica si el botón está presionado.
        /// <para>Returns a value that indicates whether the specified gamepad button is current down.</para>
        /// </summary>
        /// <param name="button">El botón del gamepad a comprobar. <para>The gamepad button to check.</para></param>
        /// <returns>True si el botón está presionado; de lo contrario, false. <para>True if the specified gamepad button is currently down; otherwise, false.</para></returns>
        public bool IsButtonDown(Buttons button)
        {
            return CurrentState.IsButtonDown(button);
        }

        /// <summary>
        /// Devuelve un valor que indica si el botón está suelto.
        /// <para>Returns a value that indicates whether the specified gamepad button is currently up.</para>
        /// </summary>
        /// <param name="button">El botón del gamepad a comprobar. <para>The gamepad button to check.</para></param>
        /// <returns>True si el botón está suelto; de lo contrario, false. <para>True if the specified gamepad button is currently up; otherwise, false.</para></returns>
        public bool IsButtonUp(Buttons button)
        {
            return CurrentState.IsButtonUp(button);
        }

        /// <summary>
        /// Devuelve un valor que indica si el botón se presionó en este frame.
        /// <para>Returns a value that indicates whether the specified gamepad button was just pressed on the current frame.</para>
        /// </summary>
        /// <param name="button">El botón del gamepad a comprobar. <para>The gamepad button to check.</para></param>
        /// <returns>True si el botón se presionó en este frame; de lo contrario, false. <para>True if the specified gamepad button was just pressed on the current frame; otherwise, false.</para></returns>
        public bool WasButtonJustPressed(Buttons button)
        {
            return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
        }

        /// <summary>
        /// Devuelve un valor que indica si el botón se soltó en este frame.
        /// <para>Returns a value that indicates whether the specified gamepad button was just released on the current frame.</para>
        /// </summary>
        /// <param name="button">El botón del gamepad a comprobar. <para>The gamepad button to check.</para></param>
        /// <returns>True si el botón se soltó en este frame; de lo contrario, false. <para>True if the specified gamepad button was just released on the current frame; otherwise, false.</para></returns>
        public bool WasButtonJustReleased(Buttons button)
        {
            return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
        }

        /// <summary>
        /// Establece la vibración para todos los motores del gamepad.
        /// <para>Sets the vibration for all motors of this gamepad.</para>
        /// </summary>
        /// <param name="strength">La fuerza de vibración de 0.0f a 1.0f. <para>The strength of the vibration from 0.0f (none) to 1.0f (full).</para></param>
        /// <param name="time">El tiempo durante el que la vibración debe ocurrir. <para>The amount of time the vibration should occur.</para></param>
        public void SetVibration(float strength, TimeSpan time)
        {
            _vibrationTimeRemaining = time;
            GamePad.SetVibration(PlayerIndex, strength, strength);
        }

        /// <summary>
        /// Detiene la vibración de todos los motores del gamepad.
        /// <para>Stops the vibration of all motors for this gamepad.</para>
        /// </summary>
        public void StopVibration()
        {
            GamePad.SetVibration(PlayerIndex, 0.0f, 0.0f);
        }



    }
}
