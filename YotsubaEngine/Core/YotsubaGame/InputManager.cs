using Microsoft.Xna.Framework;
using YotsubaEngine.Input;

namespace YotsubaEngine.Core.YotsubaGame
{
    /// <summary>
    /// Central manager for keyboard, mouse, gamepad, and touch input.
    /// Administrador central de entrada para teclado, mouse, gamepad y touch.
    /// </summary>
    public class InputManager
    {
        
        /// <summary>
        /// Instancia del input manager
        /// </summary>
        private static InputManager instance;

        /// <summary>
        /// Instanciar el input manager como instancia unica
        /// </summary>
        public static InputManager Instance { get => instance == null ? instance = new InputManager() : instance; }

        /// <summary>
        /// Gets the state information of keyboard input.
        /// </summary>
        public KeyboardInfo Keyboard { get; private set; }

        /// <summary>
        /// Gets the state information of mouse input.
        /// </summary>
        public MouseInfo Mouse { get; private set; }

        /// <summary>
        /// Gets the state information of a gamepad.
        /// </summary>
        public GamePadInfo[] GamePads { get; private set; }

        /// <summary>
        /// Gets the state information of touchs
        /// </summary>
        public TouchInfo Touch { get; private set; }
        /// <summary>
        /// Creates a new InputManager.
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
        /// Updates the state information for the keyboard, mouse, and gamepad inputs.
        /// </summary>
        /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
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
