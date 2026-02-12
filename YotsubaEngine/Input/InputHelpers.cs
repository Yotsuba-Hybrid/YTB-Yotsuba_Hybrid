using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;

namespace YotsubaEngine.Input
{
    /// <summary>
    /// Clase auxiliar que provee APIs para gestionar los bindings de input de las entidades.
    /// <para>Helper class providing APIs for managing entity input bindings.</para>
    /// </summary>
    public static class InputHelpers
    {
        /// <summary>
        /// Reference to the entity manager.
        /// Referencia al administrador de entidades.
        /// </summary>
        private static EntityManager _entityManager;

        /// <summary>
        /// Inicializa InputHelpers con el administrador de entidades.
        /// <para>Initializes the InputHelpers with the entity manager.</para>
        /// </summary>
        /// <param name="entityManager">Instancia del administrador de entidades. <para>Entity manager instance.</para></param>
        public static void Initialize(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        #region Keyboard Bindings

        /// <summary>
        /// Vincula una tecla del teclado a una acción para una entidad específica.
        /// <para>Binds a keyboard key to an action for a specific entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="action">Acción a vincular. <para>Action to bind.</para></param>
        /// <param name="key">Tecla del teclado. <para>Keyboard key.</para></param>
        public static void BindKeyboard(Yotsuba entity, ActionEntityInput action, Keys key)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.KeyBoard[action] = key;
            inputComponent.AddInput(InputInUse.HasKeyboard);
        }

        /// <summary>
        /// Vincula una tecla del teclado a una acción para una entidad por ID.
        /// <para>Binds a keyboard key to an action for a specific entity by ID.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <param name="action">Acción a vincular. <para>Action to bind.</para></param>
        /// <param name="key">Tecla del teclado. <para>Keyboard key.</para></param>
        public static void BindKeyboard(int entityId, ActionEntityInput action, Keys key)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.KeyBoard[action] = key;
            inputComponent.AddInput(InputInUse.HasKeyboard);
        }

        /// <summary>
        /// Desvincula una tecla del teclado de una acción para una entidad específica.
        /// <para>Unbinds a keyboard key from an action for a specific entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="action">Acción a desvincular. <para>Action to unbind.</para></param>
        public static void UnbindKeyboard(Yotsuba entity, ActionEntityInput action)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.KeyBoard.Remove(action);
        }

        /// <summary>
        /// Desvincula una tecla del teclado de una acción para una entidad por ID.
        /// <para>Unbinds a keyboard key from an action for a specific entity by ID.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <param name="action">Acción a desvincular. <para>Action to unbind.</para></param>
        public static void UnbindKeyboard(int entityId, ActionEntityInput action)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.KeyBoard.Remove(action);
        }

        /// <summary>
        /// Limpia todos los bindings de teclado para una entidad.
        /// <para>Clears all keyboard bindings for an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
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
        /// Vincula un botón del gamepad a una acción para una entidad específica.
        /// <para>Binds a gamepad button to an action for a specific entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="action">Acción a vincular. <para>Action to bind.</para></param>
        /// <param name="button">Botón del gamepad. <para>Gamepad button.</para></param>
        public static void BindGamePad(Yotsuba entity, ActionEntityInput action, Buttons button)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.GamePad[action] = button;
            inputComponent.AddInput(InputInUse.HasGamepad);
        }

        /// <summary>
        /// Vincula un botón del gamepad a una acción para una entidad por ID.
        /// <para>Binds a gamepad button to an action for a specific entity by ID.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <param name="action">Acción a vincular. <para>Action to bind.</para></param>
        /// <param name="button">Botón del gamepad. <para>Gamepad button.</para></param>
        public static void BindGamePad(int entityId, ActionEntityInput action, Buttons button)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.GamePad[action] = button;
            inputComponent.AddInput(InputInUse.HasGamepad);
        }

        /// <summary>
        /// Desvincula un botón del gamepad de una acción para una entidad específica.
        /// <para>Unbinds a gamepad button from an action for a specific entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="action">Acción a desvincular. <para>Action to unbind.</para></param>
        public static void UnbindGamePad(Yotsuba entity, ActionEntityInput action)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.GamePad.Remove(action);
        }

        /// <summary>
        /// Desvincula un botón del gamepad de una acción para una entidad por ID.
        /// <para>Unbinds a gamepad button from an action for a specific entity by ID.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <param name="action">Acción a desvincular. <para>Action to unbind.</para></param>
        public static void UnbindGamePad(int entityId, ActionEntityInput action)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.GamePad.Remove(action);
        }

        /// <summary>
        /// Limpia todos los bindings del gamepad para una entidad.
        /// <para>Clears all gamepad bindings for an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        public static void ClearGamePadBindings(Yotsuba entity)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.GamePad.Clear();
            inputComponent.RemoveInput(InputInUse.HasGamepad);
        }

        /// <summary>
        /// Establece el índice del jugador del gamepad para una entidad.
        /// <para>Sets the gamepad player index for an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="playerIndex">Índice del jugador (0-3). <para>Player index (0-3).</para></param>
        public static void SetGamePadIndex(Yotsuba entity, PlayerIndex playerIndex)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.GamePadIndex = playerIndex;
        }

        /// <summary>
        /// Establece el índice del jugador del gamepad para una entidad por ID.
        /// <para>Sets the gamepad player index for an entity by ID.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <param name="playerIndex">Índice del jugador (0-3). <para>Player index (0-3).</para></param>
        public static void SetGamePadIndex(int entityId, PlayerIndex playerIndex)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.GamePadIndex = playerIndex;
        }

        #endregion

        #region Mouse Bindings

        /// <summary>
        /// Vincula un botón del mouse a una acción para una entidad específica.
        /// <para>Binds a mouse button to an action for a specific entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="action">Acción a vincular. <para>Action to bind.</para></param>
        /// <param name="button">Botón del mouse. <para>Mouse button.</para></param>
        public static void BindMouse(Yotsuba entity, ActionEntityInput action, MouseButton button)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.Mouse[action] = button;
            inputComponent.AddInput(InputInUse.HasMouse);
        }

        /// <summary>
        /// Vincula un botón del mouse a una acción para una entidad por ID.
        /// <para>Binds a mouse button to an action for a specific entity by ID.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <param name="action">Acción a vincular. <para>Action to bind.</para></param>
        /// <param name="button">Botón del mouse. <para>Mouse button.</para></param>
        public static void BindMouse(int entityId, ActionEntityInput action, MouseButton button)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.Mouse[action] = button;
            inputComponent.AddInput(InputInUse.HasMouse);
        }

        /// <summary>
        /// Desvincula un botón del mouse de una acción para una entidad específica.
        /// <para>Unbinds a mouse button from an action for a specific entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="action">Acción a desvincular. <para>Action to unbind.</para></param>
        public static void UnbindMouse(Yotsuba entity, ActionEntityInput action)
        {
            if (!entity.HasComponent(YTBComponent.Input)) return;

            ref var inputComponent = ref _entityManager.InputComponents[entity.Id];
            inputComponent.Mouse.Remove(action);
        }

        /// <summary>
        /// Desvincula un botón del mouse de una acción para una entidad por ID.
        /// <para>Unbinds a mouse button from an action for a specific entity by ID.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <param name="action">Acción a desvincular. <para>Action to unbind.</para></param>
        public static void UnbindMouse(int entityId, ActionEntityInput action)
        {
            ref var inputComponent = ref _entityManager.InputComponents[entityId];
            inputComponent.Mouse.Remove(action);
        }

        /// <summary>
        /// Limpia todos los bindings del mouse para una entidad.
        /// <para>Clears all mouse bindings for an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
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
        /// Configura los bindings WASD por defecto para una entidad. Nota: solo se admite una tecla por acción; para flechas hay que vincularlas por separado.
        /// <para>Sets up default WASD keyboard bindings for an entity. Note: Only one key per action is supported; bind arrow keys separately.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        public static void SetupDefaultWASD(Yotsuba entity)
        {
            BindKeyboard(entity, ActionEntityInput.MoveUp, Keys.W);
            BindKeyboard(entity, ActionEntityInput.MoveDown, Keys.S);
            BindKeyboard(entity, ActionEntityInput.MoveLeft, Keys.A);
            BindKeyboard(entity, ActionEntityInput.MoveRight, Keys.D);
            BindKeyboard(entity, ActionEntityInput.Jump, Keys.Space);
        }

        /// <summary>
        /// Configura los bindings de gamepad por defecto para una entidad.
        /// <para>Sets up default gamepad bindings for an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="playerIndex">Índice del jugador del gamepad. <para>Player index for the gamepad.</para></param>
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
        /// Configura los bindings de WASD y gamepad por defecto para una entidad.
        /// <para>Sets up both default WASD and gamepad bindings for an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="playerIndex">Índice del jugador del gamepad. <para>Player index for the gamepad.</para></param>
        public static void SetupDefaultControls(Yotsuba entity, PlayerIndex playerIndex = PlayerIndex.One)
        {
            SetupDefaultWASD(entity);
            SetupDefaultGamePad(entity, playerIndex);
        }

        #endregion

        #region Input State Queries

        /// <summary>
        /// Obtiene la posición actual del thumbstick izquierdo para un jugador específico.
        /// <para>Gets the current position of the left thumbstick for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <returns>Posición del thumbstick como Vector2. <para>Thumbstick position as Vector2.</para></returns>
        public static Vector2 GetLeftThumbstick(PlayerIndex playerIndex = PlayerIndex.One)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return Vector2.Zero;
            
            var gamePad = inputManager.GamePads[(int)playerIndex];
            return gamePad.IsConnected ? gamePad.LeftThumbStick : Vector2.Zero;
        }

        /// <summary>
        /// Obtiene la posición actual del thumbstick derecho para un jugador específico.
        /// <para>Gets the current position of the right thumbstick for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <returns>Posición del thumbstick como Vector2. <para>Thumbstick position as Vector2.</para></returns>
        public static Vector2 GetRightThumbstick(PlayerIndex playerIndex = PlayerIndex.One)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return Vector2.Zero;
            
            var gamePad = inputManager.GamePads[(int)playerIndex];
            return gamePad.IsConnected ? gamePad.RightThumbStick : Vector2.Zero;
        }

        /// <summary>
        /// Comprueba si hay un gamepad conectado para un jugador específico.
        /// <para>Checks if a gamepad is connected for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <returns>True si el gamepad está conectado. <para>True if gamepad is connected.</para></returns>
        public static bool IsGamePadConnected(PlayerIndex playerIndex = PlayerIndex.One)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return false;
            
            return inputManager.GamePads[(int)playerIndex].IsConnected;
        }

        /// <summary>
        /// Obtiene el valor del gatillo izquierdo para un jugador específico.
        /// <para>Gets the left trigger value for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <returns>Valor del gatillo (0.0 a 1.0). <para>Trigger value (0.0 to 1.0).</para></returns>
        public static float GetLeftTrigger(PlayerIndex playerIndex = PlayerIndex.One)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return 0f;
            
            var gamePad = inputManager.GamePads[(int)playerIndex];
            return gamePad.IsConnected ? gamePad.LeftTrigger : 0f;
        }

        /// <summary>
        /// Obtiene el valor del gatillo derecho para un jugador específico.
        /// <para>Gets the right trigger value for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <returns>Valor del gatillo (0.0 a 1.0). <para>Trigger value (0.0 to 1.0).</para></returns>
        public static float GetRightTrigger(PlayerIndex playerIndex = PlayerIndex.One)
        {
            var inputManager = InputManager.Instance;
            if (inputManager?.GamePads == null) return 0f;
            
            var gamePad = inputManager.GamePads[(int)playerIndex];
            return gamePad.IsConnected ? gamePad.RightTrigger : 0f;
        }

        /// <summary>
        /// Establece la vibración del gamepad para un jugador específico.
        /// <para>Sets gamepad vibration for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <param name="strength">Fuerza de vibración (0.0 a 1.0). <para>Vibration strength (0.0 to 1.0).</para></param>
        /// <param name="duration">Duración en segundos. <para>Duration in seconds.</para></param>
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
        /// Detiene la vibración del gamepad para un jugador específico.
        /// <para>Stops gamepad vibration for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
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
