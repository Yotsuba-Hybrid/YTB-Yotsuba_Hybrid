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
    /// Controla el estado del teclado entre frames.
    /// <para>Tracks keyboard input state across frames.</para>
    /// </summary>
    public class KeyboardInfo
    {
        /// <summary>
        /// Obtiene el estado del teclado durante el ciclo de actualización anterior.
        /// <para>Gets the state of keyboard input during the previous update cycle.</para>
        /// </summary>
        public KeyboardState PreviousState { get; private set; }

        /// <summary>
        /// Obtiene el estado del teclado durante el ciclo de entrada actual.
        /// <para>Gets the state of keyboard input during the current input cycle.</para>
        /// </summary>
        public KeyboardState CurrentState { get; private set; }

        /// <summary>
        /// Crea una nueva instancia de KeyboardInfo.
        /// <para>Creates a new KeyboardInfo.</para>
        /// </summary>
        public KeyboardInfo()
        {
            PreviousState = new KeyboardState();
            CurrentState = Keyboard.GetState();
        }

        /// <summary>
        /// Actualiza la información de estado del teclado.
        /// <para>Updates the state information about keyboard input.</para>
        /// </summary>
        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState();
        }

        /// <summary>
        /// Devuelve un valor que indica si la tecla especificada está presionada.
        /// <para>Returns a value that indicates if the specified key is currently down.</para>
        /// </summary>
        /// <param name="key">La tecla a comprobar. <para>The key to check.</para></param>
        /// <returns>True si la tecla está presionada; de lo contrario, false. <para>True if the specified key is currently down; otherwise, false.</para></returns>
        public bool IsKeyDown(Keys key)
        {
            return CurrentState.IsKeyDown(key);
        }

        /// <summary>
        /// Devuelve un valor que indica si la tecla especificada está suelta.
        /// <para>Returns a value that indicates whether the specified key is currently up.</para>
        /// </summary>
        /// <param name="key">La tecla a comprobar. <para>The key to check.</para></param>
        /// <returns>True si la tecla está suelta; de lo contrario, false. <para>True if the specified key is currently up; otherwise, false.</para></returns>
        public bool IsKeyUp(Keys key)
        {
            return CurrentState.IsKeyUp(key);
        }

        /// <summary>
        /// Devuelve un valor que indica si la tecla fue presionada en este frame.
        /// <para>Returns a value that indicates if the specified key was just pressed on the current frame.</para>
        /// </summary>
        /// <param name="key">La tecla a comprobar. <para>The key to check.</para></param>
        /// <returns>True si la tecla se presionó en este frame; de lo contrario, false. <para>True if the specified key was just pressed on the current frame; otherwise, false.</para></returns>
        public bool WasKeyJustPressed(Keys key)
        {
            return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
        }

        /// <summary>
        /// Devuelve un valor que indica si la tecla fue soltada en este frame.
        /// <para>Returns a value that indicates if the specified key was just released on the current frame.</para>
        /// </summary>
        /// <param name="key">La tecla a comprobar. <para>The key to check.</para></param>
        /// <returns>True si la tecla se soltó en este frame; de lo contrario, false. <para>True if the specified key was just released on the current frame; otherwise, false.</para></returns>
        public bool WasKeyJustReleased(Keys key)
        {
            return CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
        }

        /// <summary>
        /// Devuelve el valor de Caps Lock.
        /// </summary>
        /// <returns></returns>
        public bool IsCapsLockEnabled()
        {
            return CurrentState.CapsLock;
        }
    }
}
