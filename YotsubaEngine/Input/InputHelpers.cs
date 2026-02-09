using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;

namespace YotsubaEngine.Input
{
    /// <summary>
    /// Helper class providing APIs for managing entity input bindings.
    /// Clase auxiliar que provee APIs para gestionar los bindings de input de las entidades.
    /// </summary>
    public static class InputHelpers
    {
        /// <summary>
        /// Reference to the entity manager.
        /// Referencia al administrador de entidades.
        /// </summary>
        private static EntityManager _entityManager;

        /// <summary>
        /// Initializes the InputHelpers with the entity manager.
        /// Inicializa InputHelpers con el administrador de entidades.
        /// </summary>
        /// <param name="entityManager">Entity manager instance. Instancia del administrador de entidades.</param>
        public static void Initialize(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        #region Keyboard Bindings

        /// <summary>
        /// Binds a keyboard key to an action for a specific entity.
        /// Vincula una tecla del teclado a una acción para una entidad específica.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <param name="action">Action to bind. Acción a vincular.</param>
        /// <param name="key">Keyboard key. Tecla del teclado.</param>
        public static void BindKeyboard(Yotsuba entity, ActionEntityInput action, Keys key)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.KeyBoard[action] = key;
            inputComponent.AddInput(InputInUse.HasKeyboard);
        }

        /// <summary>
        /// Binds a keyboard key to an action for a specific entity by ID.
        /// Vincula una tecla del teclado a una acción para una entidad por ID.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        /// <param name="action">Action to bind. Acción a vincular.</param>
        /// <param name="key">Keyboard key. Tecla del teclado.</param>
        public static void BindKeyboard(int entityId, ActionEntityInput action, Keys key)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.KeyBoard[action] = key;
            inputComponent.AddInput(InputInUse.HasKeyboard);
        }

        /// <summary>
        /// Unbinds a keyboard key from an action for a specific entity.
        /// Desvincula una tecla del teclado de una acción para una entidad específica.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <param name="action">Action to unbind. Acción a desvincular.</param>
        public static void UnbindKeyboard(Yotsuba entity, ActionEntityInput action)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.KeyBoard.Remove(action);
        }

        /// <summary>
        /// Unbinds a keyboard key from an action for a specific entity by ID.
        /// Desvincula una tecla del teclado de una acción para una entidad por ID.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        /// <param name="action">Action to unbind. Acción a desvincular.</param>
        public static void UnbindKeyboard(int entityId, ActionEntityInput action)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.KeyBoard.Remove(action);
        }

        /// <summary>
        /// Clears all keyboard bindings for an entity.
        /// Limpia todos los bindings de teclado para una entidad.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        public static void ClearKeyboardBindings(Yotsuba entity)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.KeyBoard.Clear();
            inputComponent.RemoveInput(InputInUse.HasKeyboard);
        }

        #endregion

        #region GamePad Bindings

        /// <summary>
        /// Binds a gamepad button to an action for a specific entity.
        /// Vincula un botón del gamepad a una acción para una entidad específica.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <param name="action">Action to bind. Acción a vincular.</param>
        /// <param name="button">Gamepad button. Botón del gamepad.</param>
        public static void BindGamePad(Yotsuba entity, ActionEntityInput action, Buttons button)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.GamePad[action] = button;
            inputComponent.AddInput(InputInUse.HasGamepad);
        }

        /// <summary>
        /// Binds a gamepad button to an action for a specific entity by ID.
        /// Vincula un botón del gamepad a una acción para una entidad por ID.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        /// <param name="action">Action to bind. Acción a vincular.</param>
        /// <param name="button">Gamepad button. Botón del gamepad.</param>
        public static void BindGamePad(int entityId, ActionEntityInput action, Buttons button)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.GamePad[action] = button;
            inputComponent.AddInput(InputInUse.HasGamepad);
        }

        /// <summary>
        /// Unbinds a gamepad button from an action for a specific entity.
        /// Desvincula un botón del gamepad de una acción para una entidad específica.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <param name="action">Action to unbind. Acción a desvincular.</param>
        public static void UnbindGamePad(Yotsuba entity, ActionEntityInput action)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.GamePad.Remove(action);
        }

        /// <summary>
        /// Unbinds a gamepad button from an action for a specific entity by ID.
        /// Desvincula un botón del gamepad de una acción para una entidad por ID.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        /// <param name="action">Action to unbind. Acción a desvincular.</param>
        public static void UnbindGamePad(int entityId, ActionEntityInput action)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.GamePad.Remove(action);
        }

        /// <summary>
        /// Clears all gamepad bindings for an entity.
        /// Limpia todos los bindings del gamepad para una entidad.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        public static void ClearGamePadBindings(Yotsuba entity)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.GamePad.Clear();
            inputComponent.RemoveInput(InputInUse.HasGamepad);
        }

        /// <summary>
        /// Sets the gamepad player index for an entity.
        /// Establece el índice del jugador del gamepad para una entidad.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <param name="playerIndex">Player index (0-3). Índice del jugador (0-3).</param>
        public static void SetGamePadIndex(Yotsuba entity, PlayerIndex playerIndex)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.GamePadIndex = playerIndex;
        }

        /// <summary>
        /// Sets the gamepad player index for an entity by ID.
        /// Establece el índice del jugador del gamepad para una entidad por ID.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        /// <param name="playerIndex">Player index (0-3). Índice del jugador (0-3).</param>
        public static void SetGamePadIndex(int entityId, PlayerIndex playerIndex)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.GamePadIndex = playerIndex;
        }

        #endregion

        #region Mouse Bindings

        /// <summary>
        /// Binds a mouse button to an action for a specific entity.
        /// Vincula un botón del mouse a una acción para una entidad específica.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <param name="action">Action to bind. Acción a vincular.</param>
        /// <param name="button">Mouse button. Botón del mouse.</param>
        public static void BindMouse(Yotsuba entity, ActionEntityInput action, MouseButton button)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.Mouse[action] = button;
            inputComponent.AddInput(InputInUse.HasMouse);
        }

        /// <summary>
        /// Binds a mouse button to an action for a specific entity by ID.
        /// Vincula un botón del mouse a una acción para una entidad por ID.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        /// <param name="action">Action to bind. Acción a vincular.</param>
        /// <param name="button">Mouse button. Botón del mouse.</param>
        public static void BindMouse(int entityId, ActionEntityInput action, MouseButton button)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.Mouse[action] = button;
            inputComponent.AddInput(InputInUse.HasMouse);
        }

        /// <summary>
        /// Unbinds a mouse button from an action for a specific entity.
        /// Desvincula un botón del mouse de una acción para una entidad específica.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <param name="action">Action to unbind. Acción a desvincular.</param>
        public static void UnbindMouse(Yotsuba entity, ActionEntityInput action)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.Mouse.Remove(action);
        }

        /// <summary>
        /// Unbinds a mouse button from an action for a specific entity by ID.
        /// Desvincula un botón del mouse de una acción para una entidad por ID.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        /// <param name="action">Action to unbind. Acción a desvincular.</param>
        public static void UnbindMouse(int entityId, ActionEntityInput action)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.Mouse.Remove(action);
        }

        /// <summary>
        /// Clears all mouse bindings for an entity.
        /// Limpia todos los bindings del mouse para una entidad.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        public static void ClearMouseBindings(Yotsuba entity)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.Mouse.Clear();
            inputComponent.RemoveInput(InputInUse.HasMouse);
        }

        #endregion

        #region Default WASD Bindings

        /// <summary>
        /// Sets up default WASD keyboard bindings for an entity.
        /// Configura los bindings WASD por defecto para una entidad.
        /// Note: Only one key per action is supported. For arrow key support, bind them separately.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        public static void SetupDefaultWASD(Yotsuba entity)
        {
            BindKeyboard(entity, ActionEntityInput.MoveUp, Keys.W);
            BindKeyboard(entity, ActionEntityInput.MoveDown, Keys.S);
            BindKeyboard(entity, ActionEntityInput.MoveLeft, Keys.A);
            BindKeyboard(entity, ActionEntityInput.MoveRight, Keys.D);
            BindKeyboard(entity, ActionEntityInput.Jump, Keys.Space);
        }

        /// <summary>
        /// Sets up default gamepad bindings for an entity.
        /// Configura los bindings de gamepad por defecto para una entidad.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <param name="playerIndex">Player index for the gamepad. Índice del jugador del gamepad.</param>
        public static void SetupDefaultGamePad(Yotsuba entity, PlayerIndex playerIndex = PlayerIndex.One)
        {
            SetGamePadIndex(entity, playerIndex);
            BindGamePad(entity, ActionEntityInput.MoveUp, Buttons.DPadUp);
            BindGamePad(entity, ActionEntityInput.MoveDown, Buttons.DPadDown);
            BindGamePad(entity, ActionEntityInput.MoveLeft, Buttons.DPadLeft);
            BindGamePad(entity, ActionEntityInput.MoveRight, Buttons.DPadRight);
            BindGamePad(entity, ActionEntityInput.Jump, Buttons.A);
            BindGamePad(entity, ActionEntityInput.Attack, Buttons.X);
            BindGamePad(entity, ActionEntityInput.Dash, Buttons.B);
        }

        /// <summary>
        /// Sets up both default WASD and gamepad bindings for an entity.
        /// Configura los bindings de WASD y gamepad por defecto para una entidad.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <param name="playerIndex">Player index for the gamepad. Índice del jugador del gamepad.</param>
        public static void SetupDefaultControls(Yotsuba entity, PlayerIndex playerIndex = PlayerIndex.One)
        {
            SetupDefaultWASD(entity);
            SetupDefaultGamePad(entity, playerIndex);
        }

        #endregion

        #region Input State Queries

        /// <summary>
        /// Gets the current position of the left thumbstick for a specific player.
        /// Obtiene la posición actual del thumbstick izquierdo para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <returns>Thumbstick position as Vector2. Posición del thumbstick como Vector2.</returns>
        public static Vector2 GetLeftThumbstick(PlayerIndex playerIndex = PlayerIndex.One)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return Vector2.Zero;
            
            var gamePad = inputManager.GamePads[(int)playerIndex];
            return gamePad.IsConnected ? gamePad.LeftThumbStick : Vector2.Zero;
        }

        /// <summary>
        /// Gets the current position of the right thumbstick for a specific player.
        /// Obtiene la posición actual del thumbstick derecho para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <returns>Thumbstick position as Vector2. Posición del thumbstick como Vector2.</returns>
        public static Vector2 GetRightThumbstick(PlayerIndex playerIndex = PlayerIndex.One)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return Vector2.Zero;
            
            var gamePad = inputManager.GamePads[(int)playerIndex];
            return gamePad.IsConnected ? gamePad.RightThumbStick : Vector2.Zero;
        }

        /// <summary>
        /// Checks if a gamepad is connected for a specific player.
        /// Comprueba si hay un gamepad conectado para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <returns>True if gamepad is connected. True si el gamepad está conectado.</returns>
        public static bool IsGamePadConnected(PlayerIndex playerIndex = PlayerIndex.One)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return false;
            
            return inputManager.GamePads[(int)playerIndex].IsConnected;
        }

        /// <summary>
        /// Gets the left trigger value for a specific player.
        /// Obtiene el valor del gatillo izquierdo para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <returns>Trigger value (0.0 to 1.0). Valor del gatillo (0.0 a 1.0).</returns>
        public static float GetLeftTrigger(PlayerIndex playerIndex = PlayerIndex.One)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return 0f;
            
            var gamePad = inputManager.GamePads[(int)playerIndex];
            return gamePad.IsConnected ? gamePad.LeftTrigger : 0f;
        }

        /// <summary>
        /// Gets the right trigger value for a specific player.
        /// Obtiene el valor del gatillo derecho para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <returns>Trigger value (0.0 to 1.0). Valor del gatillo (0.0 a 1.0).</returns>
        public static float GetRightTrigger(PlayerIndex playerIndex = PlayerIndex.One)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return 0f;
            
            var gamePad = inputManager.GamePads[(int)playerIndex];
            return gamePad.IsConnected ? gamePad.RightTrigger : 0f;
        }

        /// <summary>
        /// Sets gamepad vibration for a specific player.
        /// Establece la vibración del gamepad para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <param name="strength">Vibration strength (0.0 to 1.0). Fuerza de vibración (0.0 a 1.0).</param>
        /// <param name="duration">Duration in seconds. Duración en segundos.</param>
        public static void SetVibration(PlayerIndex playerIndex, float strength, float duration)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return;
            
            var gamePad = inputManager.GamePads[(int)playerIndex];
            if (gamePad.IsConnected)
            {
                gamePad.SetVibration(strength, System.TimeSpan.FromSeconds(duration));
            }
        }

        /// <summary>
        /// Stops gamepad vibration for a specific player.
        /// Detiene la vibración del gamepad para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        public static void StopVibration(PlayerIndex playerIndex)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return;
            
            var gamePad = inputManager.GamePads[(int)playerIndex];
            if (gamePad.IsConnected)
            {
                gamePad.StopVibration();
            }
        }

        #endregion
    }
}
