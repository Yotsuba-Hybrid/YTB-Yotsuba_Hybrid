using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading.Tasks;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.S_2D;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using YotsubaEngine.Exceptions;
using YotsubaEngine.Input;

namespace YotsubaEngine.Core.System.S_AGNOSTIC
{
    /// <summary>
    /// System that handles user input (keyboard, mouse, gamepad).
    /// Sistema que se encarga de manejar la entrada del usuario (teclado, raton, gamepad).
    /// </summary>
    public class InputSystem : ISystem
    {
        /// <summary>
        /// Event manager reference for dispatching input events.
        /// Referencia al EventManager para manejar eventos.
        /// </summary>
        private EventManager EventManager { get; set; }

        /// <summary>
        /// Entity manager reference for entity/component access.
        /// Referencia al EntityManager para manejar entidades y componentes.
        /// </summary>
        private EntityManager EntityManager { get; set; }


        /// <summary>
        /// Input manager handling mouse, keyboard, and gamepad state.
        /// Clase que se encarga de manejar el estado de los inputs del mouse, teclado, y GamePad
        /// </summary>
        private InputManager InputManager { get; set; }

        /// <summary>
        /// Initializes the input system.
        /// Implementacion de la interfaz ISystem
        /// </summary>
        /// <param name="entities">Entity manager. Administrador de entidades.</param>
        public void InitializeSystem(EntityManager entities)
        {

#if YTB
            if (GameWontRun.GameWontRunByException) return;
#endif

            InputManager = InputManager.Instance;
            EventManager = EventManager.Instance;
            EntityManager = entities;
            EngineUISystem.SendLog(typeof(InputSystem).Name + " Se inicio correctamente");

        }

        /// <summary>
        /// Updates input state and publishes input events.
        /// Actualiza el estado de entrada y publica eventos de input.
        /// </summary>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        public void UpdateSystem(GameTime gameTime)
        {

#if YTB
            if (InputManager is not null)
                InputManager.Update(gameTime);

            KeyboardInfo KeyboardState = InputManager.Instance.Keyboard;
            bool IsKeyShiftDown = KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift);
            bool IsKeyCtrlDown = KeyboardState.IsKeyDown(Keys.LeftControl);
            bool IsKeyVJustPressed = KeyboardState.WasKeyJustPressed(Keys.V);
            bool isKeyGJustPressed = KeyboardState.WasKeyJustPressed(Keys.G);
            bool IsKeySJustPressed = KeyboardState.WasKeyJustPressed(Keys.S);

            if (IsKeyCtrlDown && IsKeySJustPressed)
            {
                EngineUISystem.SaveChanges();
                YTBGame game = (YTBGame)YTBGlobalState.Game;
                YTBGlobalState.LastSceneNameBeforeUpdate = game.SceneManager.CurrentScene.SceneName;
                Task.Run(async () => await YTBFileToGameData.UpdateStateOfSceneManager());
            }

            if (IsKeyCtrlDown && IsKeyVJustPressed && IsKeyShiftDown)
                EventManager.Publish(new OnHiddeORShowUIEngineEditor());


            if (IsKeyCtrlDown && isKeyGJustPressed && IsKeyShiftDown)
                EventManager.Publish(new OnHiddeORShowGameUI());

            



            //if (IsKeyCtrlDown && IsKeyShiftDown)
            //{

                if (KeyboardState.IsKeyDown(Keys.Space))
                {
                    YTBGlobalState.CameraZoom = 1f;
                    YTBGlobalState.OffsetCamera = Vector2.Zero;
                }
                if (KeyboardState.IsKeyDown(Keys.Right))
                {
                    YTBGlobalState.OffsetCamera += new Vector2(10f, 0f);
                    if (KeyboardState.IsKeyDown(Keys.RightShift))
                        YTBGlobalState.OffsetCamera += new Vector2(10f, 0f);

                }
                if (KeyboardState.IsKeyDown(Keys.Left))
                {
                    YTBGlobalState.OffsetCamera += new Vector2(-10f, 0f);
                    if (KeyboardState.IsKeyDown(Keys.RightShift))
                        YTBGlobalState.OffsetCamera += new Vector2(-10f, 0f);
                }
                if (KeyboardState.IsKeyDown(Keys.Up))
                {
                    YTBGlobalState.OffsetCamera += new Vector2(0f, -10f);
                    if (KeyboardState.IsKeyDown(Keys.RightShift))
                        YTBGlobalState.OffsetCamera += new Vector2(0f, -10f);
                }
                if (KeyboardState.IsKeyDown(Keys.Down))
                {
                    YTBGlobalState.OffsetCamera += new Vector2(0f, 10f);
                    if (KeyboardState.IsKeyDown(Keys.RightShift))
                        YTBGlobalState.OffsetCamera += new Vector2(0f, 10f);
                }

                if (OperatingSystem.IsWindows())
                    if (!RenderSystem2D.IsGameActive) return;

                if (KeyboardState.WasKeyJustPressed(Keys.OemPlus))
                {
                    YTBGlobalState.CameraZoom += 0.1f;
                }
                else if (KeyboardState.WasKeyJustPressed(Keys.OemMinus))
                {
                    YTBGlobalState.CameraZoom -= 0.3f;
                }
                else if (KeyboardState.IsKeyDown(Keys.OemPlus))
                {
                    YTBGlobalState.CameraZoom += 0.03f;
                }
                else if (KeyboardState.IsKeyDown(Keys.OemMinus))
                {
                    YTBGlobalState.CameraZoom -= 0.01f;
                }
            //}

                //if (!RenderSystem2D.IsGameActive) return;
                /*if (GameWontRun.GameWontRunByException)*/
            
                return;
#endif



            InputManager.Update(gameTime);
            var entityManager = EntityManager;
            if (entityManager == null) return;


            if (entityManager.YotsubaEntities.Count > 1000)
            {
                Parallel.ForEach(entityManager.YotsubaEntities, (entity) =>
                {
                    if (entity.HasComponent(YTBComponent.Input))
                    {

                        ref InputComponent inputComponent =
                        ref entityManager.InputComponents[entity.Id];

                        ProcessKeyboard(entity.Id, ref inputComponent, gameTime);
                        ProcessMouse(entity.Id, ref inputComponent, gameTime);
                        ProcessGamePad(entity.Id, ref inputComponent, gameTime);
                        ProcessThumbsticks(entity.Id, ref inputComponent, gameTime);
                    }
                });
            }
            else
            {
                foreach (Yotsuba entity in entityManager.YotsubaEntities)
                {
                    if (entity.HasComponent(YTBComponent.Input))
                    {
                        ref InputComponent inputComponent =
                        ref entityManager.InputComponents[entity.Id];

                        ProcessKeyboard(entity.Id, ref inputComponent, gameTime);
                        ProcessMouse(entity.Id, ref inputComponent, gameTime);
                        ProcessGamePad(entity.Id, ref inputComponent, gameTime);
                        ProcessThumbsticks(entity.Id, ref inputComponent, gameTime);
                    }
                }
            }

        }

        /// <summary>
        /// Processes keyboard input and publishes events for mapped entities.
        /// Método que se encarga de procesar la entrada del teclado y 
        /// enviar eventos cada vez que es presionada una tecla y hay una entidad que la usa.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        /// <param name="inputComponent">Input component. Componente de entrada.</param>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        private void ProcessKeyboard(int entityId, ref InputComponent inputComponent, GameTime gameTime)
        {
            KeyboardInfo KeyboardState = InputManager.Keyboard;
            foreach (var (entityAction, key) in inputComponent.KeyBoard)
            {
                bool IsKeyDown = KeyboardState.IsKeyDown(key);
                bool IsKeyJustPressed = KeyboardState.WasKeyJustPressed(key);
                bool IsKeyJustRelease = KeyboardState.WasKeyJustReleased(key);


                if (IsKeyJustPressed)
                {
                    EventManager.Publish(new OnKeyBoardEvent(entityId, key, gameTime, entityAction, InputEventType.JustPressed));
                    continue;
                }

                if (IsKeyJustRelease)
                    EventManager.Publish(new OnKeyBoardEvent(entityId, key, gameTime, entityAction, InputEventType.JustReleased));

                if (IsKeyDown)
                    EventManager.Publish(new OnKeyBoardEvent(entityId, key, gameTime, entityAction, InputEventType.HoldDown));
            }
        }

        /// <summary>
        /// Processes mouse input and publishes events for mapped entities.
        /// Método que se encarga de procesar la entrada del Mouse y 
        /// enviar eventos cada vez que es presionada una tecla y hay una entidad que la usa.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        /// <param name="inputComponent">Input component. Componente de entrada.</param>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        private void ProcessMouse(int entityId, ref InputComponent inputComponent, GameTime gameTime)
        {
            MouseInfo MouseState = InputManager.Mouse;
            foreach (var (entityAction, key) in inputComponent.Mouse)
            {
                bool IsMouseButtonDown = MouseState.IsButtonDown(key);
                bool IsMouseButtonJustPressed = MouseState.WasButtonJustPressed(key);
                bool IsMouseButtonJustRelease = MouseState.WasButtonJustReleased(key);

                if (IsMouseButtonJustPressed)
                {
                    EventManager.Publish(new OnMouseEvent(entityId, key, gameTime, entityAction, InputEventType.JustPressed));
                    continue;
                }

                if (IsMouseButtonJustRelease)
                    EventManager.Publish(new OnMouseEvent(entityId, key, gameTime, entityAction, InputEventType.JustReleased));

                if (IsMouseButtonDown)
                    EventManager.Publish(new OnMouseEvent(entityId, key, gameTime, entityAction, InputEventType.HoldDown));


            }
        }

        /// <summary>
        /// Processes gamepad input and publishes events for mapped entities.
        /// Metodo que se encarga de procesar el gamepad
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        /// <param name="inputComponent">Input component. Componente de entrada.</param>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        private void ProcessGamePad(int entityId, ref InputComponent inputComponent, GameTime gameTime)
        {
            GamePadInfo gamePadInfo = new GamePadInfo(inputComponent.GamePadIndex);

            foreach (var (actionEntity, gamePadButton) in inputComponent.GamePad)
            {
                bool buttonJustPressed = gamePadInfo.WasButtonJustPressed(gamePadButton);
                bool buttonDown = gamePadInfo.IsButtonDown(gamePadButton);
                bool buttonJustReleased = gamePadInfo.WasButtonJustReleased(gamePadButton);

                if (buttonJustPressed)
                {
                    EventManager.Publish(new OnGamePadEvent(entityId, gamePadButton, gameTime, actionEntity, InputEventType.JustPressed));
                    continue;
                }

                if (buttonJustReleased)
                    EventManager.Publish(new OnGamePadEvent(entityId, gamePadButton, gameTime, actionEntity, InputEventType.JustReleased));

                if (buttonDown)
                    EventManager.Publish(new OnGamePadEvent(entityId, gamePadButton, gameTime, actionEntity, InputEventType.HoldDown));
            }

        }

        /// <summary>
        /// Processes gamepad thumbstick input and publishes events for analog movement.
        /// Método que procesa los thumbsticks del gamepad para movimiento analógico.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        /// <param name="inputComponent">Input component. Componente de entrada.</param>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        private void ProcessThumbsticks(int entityId, ref InputComponent inputComponent, GameTime gameTime)
        {
            if (!inputComponent.HasInput(InputInUse.HasGamepad)) return;

            GamePadInfo gamePadInfo = InputManager.GamePads[(int)inputComponent.GamePadIndex];

            if (!gamePadInfo.IsConnected) return;

            Vector2 leftThumbstick = gamePadInfo.LeftThumbStick;
            Vector2 rightThumbstick = gamePadInfo.RightThumbStick;

            // Only publish event if there's significant thumbstick movement
            const float deadzone = 0.1f;
            if (Math.Abs(leftThumbstick.X) > deadzone || Math.Abs(leftThumbstick.Y) > deadzone ||
                Math.Abs(rightThumbstick.X) > deadzone || Math.Abs(rightThumbstick.Y) > deadzone)
            {
                EventManager.Publish(new OnThumbstickEvent(
                    entityId,
                    gameTime,
                    leftThumbstick,
                    rightThumbstick,
                    inputComponent.GamePadIndex));
            }
        }

        /// <summary>
        /// Shared entity update hook (unused in this system).
        /// Hook de actualización compartida (no usado en este sistema).
        /// </summary>
        /// <param name="Entidad">Entity instance. Instancia de entidad.</param>
        /// <param name="time">Game time. Tiempo de juego.</param>
        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Shared entity initialization hook (unused in this system).
        /// Hook de inicialización compartida (no usado en este sistema).
        /// </summary>
        /// <param name="Entidad">Entity instance. Instancia de entidad.</param>
        public void SharedEntityInitialize(Yotsuba Entidad)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Enum representing mouse buttons.
        /// Enum que representa los diferentes botones del mouse
        /// </summary>
        public enum MouseButton
        {
            /// <summary>
            /// Left mouse button.
            /// Botón izquierdo del mouse.
            /// </summary>
            Left,
            /// <summary>
            /// Middle mouse button.
            /// Botón medio del mouse.
            /// </summary>
            Middle,
            /// <summary>
            /// Right mouse button.
            /// Botón derecho del mouse.
            /// </summary>
            Right,
            /// <summary>
            /// Extra mouse button 1.
            /// Botón extra 1 del mouse.
            /// </summary>
            XButton1,
            /// <summary>
            /// Extra mouse button 2.
            /// Botón extra 2 del mouse.
            /// </summary>
            XButton2
        }
    }
}