using Microsoft.Xna.Framework;
using YotsubaEngine.Input;

namespace YotsubaEngine.Core.YotsubaGame
{
    /// <summary>
    /// Administrador central de entrada para teclado, mouse, gamepad y touch.
    /// <para>Central manager for keyboard, mouse, gamepad, and touch input.</para>
    /// </summary>
    public class InputManager
    {
        
        /// <summary>
        /// Instancia del input manager
        /// </summary>
        private static InputManager instance;

        /// <summary>
        /// Instancia única del administrador de entrada.
        /// <para>Singleton instance of the input manager.</para>
        /// </summary>
        public static InputManager Instance { get => instance == null ? instance = new InputManager() : instance; }

        /// <summary>
        /// Obtiene el estado de entrada del teclado.
        /// <para>Gets the state information of keyboard input.</para>
        /// </summary>
        public KeyboardInfo Keyboard { get; private set; }

        /// <summary>
        /// Obtiene el estado de entrada del mouse.
        /// <para>Gets the state information of mouse input.</para>
        /// </summary>
        public MouseInfo Mouse { get; private set; }

        /// <summary>
        /// Obtiene el estado de entrada del gamepad.
        /// <para>Gets the state information of a gamepad.</para>
        /// </summary>
        public GamePadInfo[] GamePads { get; private set; }

        /// <summary>
        /// Obtiene el estado de entrada táctil.
        /// <para>Gets the state information of touch input.</para>
        /// </summary>
        public TouchInfo Touch { get; private set; }
        /// <summary>
        /// Crea una nueva instancia del administrador de entrada.
        /// <para>Creates a new InputManager.</para>
        /// </summary>
        public InputManager()
        {
            Keyboard = new KeyboardInfo();
            Mouse = new MouseInfo();
            Touch = new TouchInfo();
            GamePads = new GamePadInfo[4];
            for (int i = 0; i < 4; i++)
            {
                GamePads[i] = new GamePadInfo((PlayerIndex)i);
            }
        }

        /// <summary>
        /// Actualiza el estado de entrada del teclado, mouse, gamepad y touch.
        /// <para>Updates the state information for the keyboard, mouse, and gamepad inputs.</para>
        /// </summary>
        /// <param name="gameTime">Instantánea de tiempo del frame actual. <para>A snapshot of the timing values for the current frame.</para></param>
        public void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();
            Touch.Update();
            for (int i = 0; i < 4; i++)
            {
                GamePads[i].Update(gameTime);
            }
        }

    }
}
