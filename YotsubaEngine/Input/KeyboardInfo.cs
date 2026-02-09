using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YotsubaEngine.Core.System.YotsubaEngineUI;

namespace YotsubaEngine.Input
{
    /// <summary>
    /// Tracks keyboard input state across frames.
    /// Controla el estado del teclado entre frames.
    /// </summary>
    public class KeyboardInfo
    {
        /// <summary>
        /// Gets the state of keyboard input during the previous update cycle.
        /// Obtiene el estado del teclado durante el ciclo de actualización anterior.
        /// </summary>
        public KeyboardState PreviousState { get; private set; }

        /// <summary>
        /// Gets the state of keyboard input during the current input cycle.
        /// Obtiene el estado del teclado durante el ciclo de entrada actual.
        /// </summary>
        public KeyboardState CurrentState { get; private set; }

        /// <summary>
        /// Creates a new KeyboardInfo. 
        /// Crea una nueva instancia de KeyboardInfo.
        /// </summary>
        public KeyboardInfo()
        {
            PreviousState = new KeyboardState();
            CurrentState = Keyboard.GetState();
        }

        /// <summary>
        /// Updates the state information about keyboard input.
        /// Actualiza la información de estado del teclado.
        /// </summary>
        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState();
        }

        /// <summary>
        /// Returns a value that indicates if the specified key is currently down.
        /// Devuelve un valor que indica si la tecla especificada está presionada.
        /// </summary>
        /// <param name="key">The key to check. La tecla a comprobar.</param>
        /// <returns>true if the specified key is currently down; otherwise, false. true si la tecla está presionada; de lo contrario, false.</returns>
        public bool IsKeyDown(Keys key)
        {
            return CurrentState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns a value that indicates whether the specified key is currently up.
        /// Devuelve un valor que indica si la tecla especificada está suelta.
        /// </summary>
        /// <param name="key">The key to check. La tecla a comprobar.</param>
        /// <returns>true if the specified key is currently up; otherwise, false. true si la tecla está suelta; de lo contrario, false.</returns>
        public bool IsKeyUp(Keys key)
        {
            return CurrentState.IsKeyUp(key);
        }

        /// <summary>
        /// Returns a value that indicates if the specified key was just pressed on the current frame.
        /// Devuelve un valor que indica si la tecla fue presionada en este frame.
        /// </summary>
        /// <param name="key">The key to check. La tecla a comprobar.</param>
        /// <returns>true if the specified key was just pressed on the current frame; otherwise, false. true si la tecla se presionó en este frame; de lo contrario, false.</returns>
        public bool WasKeyJustPressed(Keys key)
        {
            return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
        }

        /// <summary>
        /// Returns a value that indicates if the specified key was just released on the current frame.
        /// Devuelve un valor que indica si la tecla fue soltada en este frame.
        /// </summary>
        /// <param name="key">The key to check. La tecla a comprobar.</param>
        /// <returns>true if the specified key was just released on the current frame; otherwise, false. true si la tecla se soltó en este frame; de lo contrario, false.</returns>
        public bool WasKeyJustReleased(Keys key)
        {
            return CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
        }

    }
}
