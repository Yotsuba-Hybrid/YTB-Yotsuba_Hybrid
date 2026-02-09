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
    /// Provides all relevant information about the mouse.
    /// Clase que nos da toda la informacion relevante sobre el mouse
    /// </summary>
    public class MouseInfo
    {
        /// <summary>
        /// The state of mouse input during the previous update cycle.
        /// El estado del mouse durante el ciclo de actualización anterior.
        /// </summary>
        public MouseState PreviousState { get; private set; }

        /// <summary>
        /// The state of mouse input during the current update cycle.
        /// El estado del mouse durante el ciclo de actualización actual.
        /// </summary>
        public MouseState CurrentState { get; private set; }

        /// <summary>
        /// Gets or Sets the current position of the mouse cursor in screen space.
        /// Obtiene o establece la posición del cursor en el espacio de pantalla.
        /// </summary>
        public Point Position
        {
            get => CurrentState.Position;
            set => SetPosition(value.X, value.Y);
        }

        /// <summary>
        /// Gets or Sets the current x-coordinate position of the mouse cursor in screen space.
        /// Obtiene o establece la coordenada X del cursor en pantalla.
        /// </summary>
        public int X
        {
            get => CurrentState.X;
            set => SetPosition(value, CurrentState.Y);
        }

        /// <summary>
        /// Gets or Sets the current y-coordinate position of the mouse cursor in screen space.
        /// Obtiene o establece la coordenada Y del cursor en pantalla.
        /// </summary>
        public int Y
        {
            get => CurrentState.Y;
            set => SetPosition(CurrentState.X, value);
        }

        /// <summary>
        /// Gets the difference in the mouse cursor position between the previous and current frame.
        /// Obtiene la diferencia de posición del cursor entre el frame anterior y el actual.
        /// </summary>
        public Point PositionDelta => CurrentState.Position - PreviousState.Position;

        /// <summary>
        /// Gets the difference in the mouse cursor x-position between the previous and current frame.
        /// Obtiene la diferencia en X del cursor entre el frame anterior y el actual.
        /// </summary>
        public int XDelta => CurrentState.X - PreviousState.X;

        /// <summary>
        /// Gets the difference in the mouse cursor y-position between the previous and current frame.
        /// Obtiene la diferencia en Y del cursor entre el frame anterior y el actual.
        /// </summary>
        public int YDelta => CurrentState.Y - PreviousState.Y;

        /// <summary>
        /// Gets a value that indicates if the mouse cursor moved between the previous and current frames.
        /// Obtiene un valor que indica si el cursor se movió entre frames.
        /// </summary>
        public bool WasMoved => PositionDelta != Point.Zero;

        /// <summary>
        /// Gets the cumulative value of the mouse scroll wheel since the start of the game.
        /// Obtiene el valor acumulado de la rueda del mouse desde el inicio del juego.
        /// </summary>
        public int ScrollWheel => CurrentState.ScrollWheelValue;

        /// <summary>
        /// Gets the value of the scroll wheel between the previous and current frame.
        /// Obtiene el valor de la rueda del mouse entre el frame anterior y el actual.
        /// </summary>
        public int ScrollWheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

        /// <summary>
        /// Creates a new MouseInfo.
        /// Crea una nueva instancia de MouseInfo.
        /// </summary>
        public MouseInfo()
        {
            PreviousState = new MouseState();
            CurrentState = Mouse.GetState();
        }

        /// <summary>
        /// Updates the state information about mouse input.
        /// Actualiza la información de estado del mouse.
        /// </summary>
        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Mouse.GetState();
        }

        /// <summary>
        /// Returns a value that indicates whether the specified mouse button is currently down.
        /// Devuelve un valor que indica si el botón del mouse está presionado.
        /// </summary>
        /// <param name="button">The mouse button to check. El botón del mouse a comprobar.</param>
        /// <returns>true if the specified mouse button is currently down; otherwise, false. true si el botón está presionado; de lo contrario, false.</returns>
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
        /// Returns a value that indicates whether the specified mouse button is current up.
        /// Devuelve un valor que indica si el botón del mouse está suelto.
        /// </summary>
        /// <param name="button">The mouse button to check. El botón del mouse a comprobar.</param>
        /// <returns>true if the specified mouse button is currently up; otherwise, false. true si el botón está suelto; de lo contrario, false.</returns>
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
        /// Returns a value that indicates whether the specified mouse button was just pressed on the current frame.
        /// Devuelve un valor que indica si el botón se presionó en este frame.
        /// </summary>
        /// <param name="button">The mouse button to check. El botón del mouse a comprobar.</param>
        /// <returns>true if the specified mouse button was just pressed on the current frame; otherwise, false. true si el botón se presionó en este frame; de lo contrario, false.</returns>
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
        /// Returns a value that indicates whether the specified mouse button was just released on the current frame.
        /// Devuelve un valor que indica si el botón se soltó en este frame.
        /// </summary>
        /// <param name="button">The mouse button to check. El botón del mouse a comprobar.</param>
        /// <returns>true if the specified mouse button was just released on the current frame; otherwise, false. true si el botón se soltó en este frame; de lo contrario, false.</returns>
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
        /// Sets the current position of the mouse cursor in screen space and updates the CurrentState with the new position.
        /// Establece la posición actual del cursor y actualiza el estado actual.
        /// </summary>
        /// <param name="x">The x-coordinate location of the mouse cursor in screen space. La coordenada X del cursor en pantalla.</param>
        /// <param name="y">The y-coordinate location of the mouse cursor in screen space. La coordenada Y del cursor en pantalla.</param>
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
