using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;

namespace YotsubaEngine.Input
{
    /// <summary>
    /// Clase que nos da toda la información relevante sobre el mouse.
    /// <para>Provides all relevant information about the mouse.</para>
    /// </summary>
    public class MouseInfo
    {
        /// <summary>
        /// El estado del mouse durante el ciclo de actualización anterior.
        /// <para>The state of mouse input during the previous update cycle.</para>
        /// </summary>
        public MouseState PreviousState { get; private set; }

        /// <summary>
        /// El estado del mouse durante el ciclo de actualización actual.
        /// <para>The state of mouse input during the current update cycle.</para>
        /// </summary>
        public MouseState CurrentState { get; private set; }

        /// <summary>
        /// Obtiene o establece la posición del cursor en el espacio de pantalla.
        /// <para>Gets or sets the current position of the mouse cursor in screen space.</para>
        /// </summary>
        public Point Position
        {
            get => CurrentState.Position;
            set => SetPosition(value.X, value.Y);
        }

        /// <summary>
        /// Obtiene o establece la coordenada X del cursor en pantalla.
        /// <para>Gets or sets the current x-coordinate position of the mouse cursor in screen space.</para>
        /// </summary>
        public int X
        {
            get => CurrentState.X;
            set => SetPosition(value, CurrentState.Y);
        }

        /// <summary>
        /// Obtiene o establece la coordenada Y del cursor en pantalla.
        /// <para>Gets or sets the current y-coordinate position of the mouse cursor in screen space.</para>
        /// </summary>
        public int Y
        {
            get => CurrentState.Y;
            set => SetPosition(CurrentState.X, value);
        }

        /// <summary>
        /// Obtiene la diferencia de posición del cursor entre el frame anterior y el actual.
        /// <para>Gets the difference in the mouse cursor position between the previous and current frame.</para>
        /// </summary>
        public Point PositionDelta => CurrentState.Position - PreviousState.Position;

        /// <summary>
        /// Obtiene la diferencia en X del cursor entre el frame anterior y el actual.
        /// <para>Gets the difference in the mouse cursor x-position between the previous and current frame.</para>
        /// </summary>
        public int XDelta => CurrentState.X - PreviousState.X;

        /// <summary>
        /// Obtiene la diferencia en Y del cursor entre el frame anterior y el actual.
        /// <para>Gets the difference in the mouse cursor y-position between the previous and current frame.</para>
        /// </summary>
        public int YDelta => CurrentState.Y - PreviousState.Y;

        /// <summary>
        /// Obtiene un valor que indica si el cursor se movió entre frames.
        /// <para>Gets a value that indicates if the mouse cursor moved between the previous and current frames.</para>
        /// </summary>
        public bool WasMoved => PositionDelta != Point.Zero;

        /// <summary>
        /// Obtiene el valor acumulado de la rueda del mouse desde el inicio del juego.
        /// <para>Gets the cumulative value of the mouse scroll wheel since the start of the game.</para>
        /// </summary>
        public int ScrollWheel => CurrentState.ScrollWheelValue;

        /// <summary>
        /// Obtiene el valor de la rueda del mouse entre el frame anterior y el actual.
        /// <para>Gets the value of the scroll wheel between the previous and current frame.</para>
        /// </summary>
        public int ScrollWheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

        /// <summary>
        /// Crea una nueva instancia de MouseInfo.
        /// <para>Creates a new MouseInfo.</para>
        /// </summary>
        public MouseInfo()
        {
            PreviousState = new MouseState();
            CurrentState = Mouse.GetState();
        }

        /// <summary>
        /// Actualiza la información de estado del mouse.
        /// <para>Updates the state information about mouse input.</para>
        /// </summary>
        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Mouse.GetState();
        }

        /// <summary>
        /// Devuelve un valor que indica si el botón del mouse está presionado.
        /// <para>Returns a value that indicates whether the specified mouse button is currently down.</para>
        /// </summary>
        /// <param name="button">El botón del mouse a comprobar. <para>The mouse button to check.</para></param>
        /// <returns>True si el botón está presionado; de lo contrario, false. <para>True if the specified mouse button is currently down; otherwise, false.</para></returns>
        public bool IsButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return CurrentState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return CurrentState.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return CurrentState.RightButton == ButtonState.Pressed;
                case MouseButton.XButton1:
                    return CurrentState.XButton1 == ButtonState.Pressed;
                case MouseButton.XButton2:
                    return CurrentState.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Devuelve un valor que indica si el botón del mouse está suelto.
        /// <para>Returns a value that indicates whether the specified mouse button is currently up.</para>
        /// </summary>
        /// <param name="button">El botón del mouse a comprobar. <para>The mouse button to check.</para></param>
        /// <returns>True si el botón está suelto; de lo contrario, false. <para>True if the specified mouse button is currently up; otherwise, false.</para></returns>
        public bool IsButtonUp(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return CurrentState.LeftButton == ButtonState.Released;
                case MouseButton.Middle:
                    return CurrentState.MiddleButton == ButtonState.Released;
                case MouseButton.Right:
                    return CurrentState.RightButton == ButtonState.Released;
                case MouseButton.XButton1:
                    return CurrentState.XButton1 == ButtonState.Released;
                case MouseButton.XButton2:
                    return CurrentState.XButton2 == ButtonState.Released;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Devuelve un valor que indica si el botón se presionó en este frame.
        /// <para>Returns a value that indicates whether the specified mouse button was just pressed on the current frame.</para>
        /// </summary>
        /// <param name="button">El botón del mouse a comprobar. <para>The mouse button to check.</para></param>
        /// <returns>True si el botón se presionó en este frame; de lo contrario, false. <para>True if the specified mouse button was just pressed on the current frame; otherwise, false.</para></returns>
        public bool WasButtonJustPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return CurrentState.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released;
                case MouseButton.Middle:
                    return CurrentState.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released;
                case MouseButton.Right:
                    return CurrentState.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released;
                case MouseButton.XButton1:
                    return CurrentState.XButton1 == ButtonState.Pressed && PreviousState.XButton1 == ButtonState.Released;
                case MouseButton.XButton2:
                    return CurrentState.XButton2 == ButtonState.Pressed && PreviousState.XButton2 == ButtonState.Released;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Devuelve un valor que indica si el botón se soltó en este frame.
        /// <para>Returns a value that indicates whether the specified mouse button was just released on the current frame.</para>
        /// </summary>
        /// <param name="button">El botón del mouse a comprobar. <para>The mouse button to check.</para></param>
        /// <returns>True si el botón se soltó en este frame; de lo contrario, false. <para>True if the specified mouse button was just released on the current frame; otherwise, false.</para></returns>
        public bool WasButtonJustReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return CurrentState.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return CurrentState.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return CurrentState.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed;
                case MouseButton.XButton1:
                    return CurrentState.XButton1 == ButtonState.Released && PreviousState.XButton1 == ButtonState.Pressed;
                case MouseButton.XButton2:
                    return CurrentState.XButton2 == ButtonState.Released && PreviousState.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Establece la posición actual del cursor y actualiza el estado actual.
        /// <para>Sets the current position of the mouse cursor in screen space and updates the CurrentState with the new position.</para>
        /// </summary>
        /// <param name="x">La coordenada X del cursor en pantalla. <para>The x-coordinate location of the mouse cursor in screen space.</para></param>
        /// <param name="y">La coordenada Y del cursor en pantalla. <para>The y-coordinate location of the mouse cursor in screen space.</para></param>
        public void SetPosition(int x, int y)
        {
            Mouse.SetPosition(x, y);
            CurrentState = new MouseState(
                x,
                y,
                CurrentState.ScrollWheelValue,
                CurrentState.LeftButton,
                CurrentState.MiddleButton,
                CurrentState.RightButton,
                CurrentState.XButton1,
                CurrentState.XButton2
            );
        }


    }
}
