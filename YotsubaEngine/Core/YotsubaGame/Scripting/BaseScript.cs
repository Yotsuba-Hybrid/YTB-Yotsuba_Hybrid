using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using YotsubaEngine.Audio;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Events.YTBEvents;
using YotsubaEngine.Input;
using YotsubaEngine.YTB_Toolkit;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;

namespace YotsubaEngine.Core.YotsubaGame.Scripting
{
    /// <summary>
    /// Clase base para todos los objetos scriptables en el motor.
    /// Para recibir eventos, la clase derivada debe implementar las interfaces correspondientes 
    /// (ej. ICollisionListener, IKeyboardListener, etc.).
    /// </summary>
    public abstract class BaseScript
    {
        #region Field Properties
        /// <summary>
        /// Ruta de este archivo
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// Referencia a la Instancia del manejador de eventos. 
        /// Con esta variable puedes enviar notificaciones a cualquier parte 
        /// del juego y recibirlas al suscribirte a ellas.
        /// </summary>
        public EventManager EventManager = EventManager.Instance;

        /// <summary>
        /// Referencia al manejador de entidades del juego. 
        /// Con esto, puedes acceder a todas las entidades de la escena
        /// </summary>
        public EntityManager EntityManager { get; set; }

        /// <summary>
        /// Referencia a la Entidad del script
        /// </summary>
        public Yotsuba Entity { get; set; }
        #endregion

        /// <summary>
        /// Inicializa el script y suscribe las interfaces detectadas.
        /// </summary>
        public virtual void Initialize()
        {
            SubscribeToEntityEvents();
        }

        /// <summary>
        /// Verifica qué interfaces implementa la clase derivada y suscribe los eventos correspondientes.
        /// Compatible 100% con Native AOT (Sin reflexión).
        /// </summary>
        private void SubscribeToEntityEvents()
        {
            // 1. Detección de Eventos de Animación
            if (this is IAnimationListener animListener)
            {
                EventManager.Subscribe<OnAnimationDontLoopReleaseEvent>((obj) =>
                {
                    if (obj.EntityId != Entity.Id) return;
                    animListener.OnAnimationDontLoopRelease(obj);
                });

                EventManager.Subscribe<AnimationChangeEvent>((obj) =>
                {
                    if (obj.EntityId != Entity.Id) return;
                    animListener.OnAnimationChange(obj);
                });
            }

            // 2. Detección de Colisiones
            if (this is ICollisionListener colListener)
            {
                EventManager.Subscribe<OnCollitionEvent>((obj) =>
                {
                    if (obj.EntityTryMove.Id == Entity.Id)
                    {
                        colListener.OnCollision(obj);
                    }

                    if (obj.EntityImpediment.Id == Entity.Id)
                    {
                        colListener.OnCollision(obj);
                    }
                });
            }

            // 3. Detección de Teclado
            if (this is IKeyboardListener keyListener)
            {
                EventManager.Subscribe<OnKeyBoardEvent>((obj) =>
                {
                    if (obj.EntityId != Entity.Id) return;
                    keyListener.OnKeyboardInput(obj);
                });
            }

            // 4. Detección de Mouse
            if (this is IMouseListener mouseListener)
            {
                EventManager.Subscribe<OnMouseEvent>((obj) =>
                {
                    if (obj.EntityId != Entity.Id) return;
                    mouseListener.OnMouseInput(obj);
                });
            }

            // 5. Detección de GamePad
            if (this is IGamePadListener gamePadListener)
            {
                EventManager.Subscribe<OnGamePadEvent>((obj) =>
                {
                    if (obj.EntityId != Entity.Id) return;
                    gamePadListener.OnGamePadInput(obj);
                });
            }

            // 6. Detección de Thumbstick (GamePad analógico)
            if (this is IThumbstickListener thumbstickListener)
            {
                EventManager.Subscribe<OnThumbstickEvent>((obj) =>
                {
                    if (obj.EntityId != Entity.Id) return;
                    thumbstickListener.OnThumbstickInput(obj);
                });
            }

            // 7. Detección de Hover (UI)
            if (this is IHoverListener hoverListener)
            {
                EventManager.Subscribe<OnHoverButton>((obj) =>
                {
                    if (obj.entityId != Entity.Id) return;
                    hoverListener.OnHover(obj);
                });
            }
        }

        /// <summary>
        /// Actualiza la lógica del script. Se llama en cada frame.
        /// </summary>
        public virtual void Update(GameTime gametime)
        {
            // Opcional: Implementación base vacía
        }

        /// <summary>
        /// Permite dibujar contenido personalizado en la UI o pantalla. 2D
        /// </summary>
        public virtual void Draw2D(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        /// <summary>
        /// Draws the 3D content for the current frame using the specified game time information.
        /// </summary>
        /// <remarks>Override this method in a derived class to implement custom 3D rendering logic. This
        /// method is typically called once per frame during the game's draw cycle.</remarks>
        /// <param name="gameTime">An object that provides a snapshot of timing values, including elapsed and total game time, for the current
        /// frame.</param>
        public virtual void Draw3D(GameTime gameTime)
        {
        }

        /// <summary>
        /// Libera los recursos del script.
        /// </summary>
        public virtual void Cleanup()
        {
            GC.Collect();
        }

        #region Entity Manipulation Methods

        /// <summary>
        /// Método para solicitar cambiar de escena inmediatamente.
        /// </summary>
        /// <param name="sceneName">Nombre de la escena destino.</param>
        public void ChangeScene(string sceneName) => SystemCall.ChangeScene(sceneName);

        /// <summary>
        /// Método para mover la entidad a una posición específica inmediatamente.
        /// </summary>
        /// <param name="Target">Nueva posición en el mundo.</param>
        public void MoveEntityTo(Vector3 Target)
        {
            ref TransformComponent transform = ref EntityManager.TransformComponents[Entity.Id];
            transform.Position = Target;
        }

        public void MoveEntityTo(int entityId, Vector3 Target)
        {
            ref TransformComponent transform = ref EntityManager.TransformComponents[entityId];
            transform.Position = Target;
        }

        public void MoveEntityTo(Yotsuba entity, Vector3 Target)
        {
            ref TransformComponent transform = ref EntityManager.TransformComponents[entity.Id];
            transform.Position = Target;
        }

        /// <summary>
        /// Método para aplicar una fuerza de movimiento a la entidad (Requiere RigidBody).
        /// </summary>
        /// <param name="Velocity">Vector de velocidad a sumar.</param>
        public void ApplyMovement(Vector2 Velocity)
        {
            if (!Entity.HasComponent(YTBComponent.Rigibody))
            {
                throw new Exception($"La entidad {Entity.Name} con id {Entity.Id} no tiene un RigidBody2D para aplicar movimiento. Esta entidad esta en la escena: \"{YTBGlobalState.Game.SceneManager.CurrentScene.SceneName}\"");
            }
            ref RigidBodyComponent2D rigidBody = ref EntityManager.Rigidbody2DComponents[Entity.Id];
            rigidBody.Velocity += new Vector3(Velocity.X, Velocity.Y, 0);
        }

        public void ApplyMovement(int entityId, Vector2 Velocity)
        {
            var yotsuba = EntityManager.YotsubaEntities.FirstOrDefault(x => x.Id == entityId);
            if (yotsuba == null || !yotsuba.HasComponent(YTBComponent.Rigibody))
            {
                throw new Exception($"La entidad con id {entityId} no tiene un RigidBody2D para aplicar movimiento. Esta entidad puede no estar en la escena actual.");
            }
            ref RigidBodyComponent2D rigidBody = ref EntityManager.Rigidbody2DComponents[entityId];
            rigidBody.Velocity += new Vector3(Velocity.X, Velocity.Y, 0);
        }

        public void ApplyMovement(Yotsuba entity, Vector2 Velocity)
        {
            if (!entity.HasComponent(YTBComponent.Rigibody))
            {
                throw new Exception($"La entidad {entity.Name} con id {entity.Id} no tiene un RigidBody2D para aplicar movimiento. Esta entidad puede no estar en la escena actual.");
            }
            ref RigidBodyComponent2D rigidBody = ref EntityManager.Rigidbody2DComponents[entity.Id];
            rigidBody.Velocity += new Vector3(Velocity.X, Velocity.Y, 0);
        }



        /// <summary>
        /// Gets the rigid body component for the current entity.
        /// Obtiene el componente de cuerpo rígido para la entidad actual.
        /// </summary>
        public ref RigidBodyComponent2D GetRigidBody2D() => ref EntityManager.Rigidbody2DComponents[Entity.Id];
        /// <summary>
        /// Gets the rigid body component for a specific entity id.
        /// Obtiene el componente de cuerpo rígido para un id de entidad específico.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public ref RigidBodyComponent2D GetRigidBody2D(int entityId) => ref EntityManager.Rigidbody2DComponents[entityId];
        /// <summary>
        /// Gets the rigid body component for a specific entity instance.
        /// Obtiene el componente de cuerpo rígido para una instancia de entidad específica.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de la entidad.</param>
        public ref RigidBodyComponent2D GetRigidBody2D(Yotsuba entity) => ref EntityManager.Rigidbody2DComponents[entity.Id];

        /// <summary>
        /// Gets the animation component for the current entity.
        /// Obtiene el componente de animación para la entidad actual.
        /// </summary>
        public ref AnimationComponent2D GetAnimationComponent() => ref EntityManager.Animation2DComponents[Entity.Id];
        /// <summary>
        /// Gets the animation component for a specific entity id.
        /// Obtiene el componente de animación para un id de entidad específico.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public ref AnimationComponent2D GetAnimationComponent(int entityId) => ref EntityManager.Animation2DComponents[entityId];
        /// <summary>
        /// Gets the animation component for a specific entity instance.
        /// Obtiene el componente de animación para una instancia de entidad específica.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de la entidad.</param>
        public ref AnimationComponent2D GetAnimationComponent(Yotsuba entity) => ref EntityManager.Animation2DComponents[entity.Id];

        /// <summary>
        /// Gets the sprite component for the current entity.
        /// Obtiene el componente de sprite para la entidad actual.
        /// </summary>
        public ref SpriteComponent2D GetSpriteComponent() => ref EntityManager.Sprite2DComponents[Entity.Id];
        /// <summary>
        /// Gets the sprite component for a specific entity id.
        /// Obtiene el componente de sprite para un id de entidad específico.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public ref SpriteComponent2D GetSpriteComponent(int entityId) => ref EntityManager.Sprite2DComponents[entityId];
        /// <summary>
        /// Gets the sprite component for a specific entity instance.
        /// Obtiene el componente de sprite para una instancia de entidad específica.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de la entidad.</param>
        public ref SpriteComponent2D GetSpriteComponent(Yotsuba entity) => ref EntityManager.Sprite2DComponents[entity.Id];

        /// <summary>
        /// Gets the transform component for the current entity.
        /// Obtiene el componente de transformación para la entidad actual.
        /// </summary>
        public ref TransformComponent GetTransformComponent() => ref EntityManager.TransformComponents[Entity.Id];
        /// <summary>
        /// Gets the transform component for a specific entity id.
        /// Obtiene el componente de transformación para un id de entidad específico.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public ref TransformComponent GetTransformComponent(int entityId) => ref EntityManager.TransformComponents[entityId];
        /// <summary>
        /// Gets the transform component for a specific entity instance.
        /// Obtiene el componente de transformación para una instancia de entidad específica.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de la entidad.</param>
        public ref TransformComponent GetTransformComponent(Yotsuba entity) => ref EntityManager.TransformComponents[entity.Id];

        /// <summary>
        /// Gets the font component for the current entity.
        /// Obtiene el componente de fuente para la entidad actual.
        /// </summary>
        public ref FontComponent2D GetFontComponent() => ref EntityManager.Font2DComponents[Entity.Id];
        /// <summary>
        /// Gets the font component for a specific entity id.
        /// Obtiene el componente de fuente para un id de entidad específico.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public ref FontComponent2D GetFontComponent(int entityId) => ref EntityManager.Font2DComponents[entityId];
        /// <summary>
        /// Gets the font component for a specific entity instance.
        /// Obtiene el componente de fuente para una instancia de entidad específica.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de la entidad.</param>
        public ref FontComponent2D GetFontComponent(Yotsuba entity) => ref EntityManager.Font2DComponents[entity.Id];

        /// <summary>
        /// Gets the button component for the current entity.
        /// Obtiene el componente de botón para la entidad actual.
        /// </summary>
        public ButtonComponent2D GetButtonComponent() => EntityManager.Button2DComponents[Entity.Id];
        /// <summary>
        /// Gets the button component for a specific entity id.
        /// Obtiene el componente de botón para un id de entidad específico.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public ButtonComponent2D GetButtonComponent(int entityId) => EntityManager.Button2DComponents[entityId];
        /// <summary>
        /// Gets the button component for a specific entity instance.
        /// Obtiene el componente de botón para una instancia de entidad específica.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de la entidad.</param>
        public ButtonComponent2D GetButtonComponent(Yotsuba entity) => EntityManager.Button2DComponents[entity.Id];

        /// <summary>
        /// Gets the tile map component for the current entity.
        /// Obtiene el componente de tilemap para la entidad actual.
        /// </summary>
        public ref TileMapComponent2D GetTilemapComponent() => ref EntityManager.TileMapComponent2Ds[Entity.Id];
        /// <summary>
        /// Gets the tile map component for a specific entity id.
        /// Obtiene el componente de tilemap para un id de entidad específico.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public ref TileMapComponent2D GetTilemapComponent(int entityId) => ref EntityManager.TileMapComponent2Ds[entityId];
        /// <summary>
        /// Gets the tile map component for a specific entity instance.
        /// Obtiene el componente de tilemap para una instancia de entidad específica.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de la entidad.</param>
        public ref TileMapComponent2D GetTilemapComponent(Yotsuba entity) => ref EntityManager.TileMapComponent2Ds[entity.Id];

        /// <summary>
        /// Gets the shader component for the current entity.
        /// Obtiene el componente de shader para la entidad actual.
        /// </summary>
        public ref ShaderComponent GetShaderComponent() => ref EntityManager.ShaderComponents[Entity.Id];
        /// <summary>
        /// Gets the shader component for a specific entity id.
        /// Obtiene el componente de shader para un id de entidad específico.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public ref ShaderComponent GetShaderComponent(int entityId) => ref EntityManager.ShaderComponents[entityId];
        /// <summary>
        /// Gets the shader component for a specific entity instance.
        /// Obtiene el componente de shader para una instancia de entidad específica.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de la entidad.</param>
        public ref ShaderComponent GetShaderComponent(Yotsuba entity) => ref EntityManager.ShaderComponents[entity.Id];

        /// <summary>
        /// Gets the input component for the current entity.
        /// Obtiene el componente de entrada para la entidad actual.
        /// </summary>
        public ref InputComponent GetInputComponent() => ref EntityManager.InputComponents[Entity.Id];
        /// <summary>
        /// Gets the input component for a specific entity id.
        /// Obtiene el componente de entrada para un id de entidad específico.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public ref InputComponent GetInputComponent(int entityId) => ref EntityManager.InputComponents[entityId];
        /// <summary>
        /// Gets the input component for a specific entity instance.
        /// Obtiene el componente de entrada para una instancia de entidad específica.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de la entidad.</param>
        public ref InputComponent GetInputComponent(Yotsuba entity) => ref EntityManager.InputComponents[entity.Id];

        /// <summary>
        /// Gets the script component for the current entity.
        /// Obtiene el componente de script para la entidad actual.
        /// </summary>
        public ref ScriptComponent GetScriptComponent() => ref EntityManager.ScriptComponents[Entity.Id];
        /// <summary>
        /// Gets the script component for a specific entity id.
        /// Obtiene el componente de script para un id de entidad específico.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public ref ScriptComponent GetScriptComponent(int entityId) => ref EntityManager.ScriptComponents[entityId];
        /// <summary>
        /// Gets the script component for a specific entity instance.
        /// Obtiene el componente de script para una instancia de entidad específica.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de la entidad.</param>
        public ref ScriptComponent GetScriptComponent(Yotsuba entity) => ref EntityManager.ScriptComponents[entity.Id];

        /// <summary>
        /// Gets the active camera component.
        /// Obtiene el componente de cámara activo.
        /// </summary>
        public CameraComponent3D GetCamera() => EntityManager.Camera;
        /// <summary>
        /// Gets the active camera component.
        /// Obtiene el componente de cámara activo.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public CameraComponent3D GetCamera(int entityId) => EntityManager.Camera;
        /// <summary>
        /// Gets the active camera component.
        /// Obtiene el componente de cámara activo.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de la entidad.</param>
        public CameraComponent3D GetCamera(Yotsuba entity) => EntityManager.Camera;

        /// <summary>
        /// Gets the current script entity.
        /// Obtiene la entidad actual del script.
        /// </summary>
        public Yotsuba GetEntity() => Entity;

        /// <summary>
        /// Gets an entity by its identifier.
        /// Obtiene una entidad por su identificador.
        /// </summary>
        /// <param name="entityId">Entity identifier. Identificador de la entidad.</param>
        public Yotsuba GetEntity(int entityId) => EntityManager.YotsubaEntities.FirstOrDefault(x => x.Id == entityId);

        /// <summary>
        /// Gets an entity by its name.
        /// Obtiene una entidad por su nombre.
        /// </summary>
        /// <param name="entityName">Entity name. Nombre de la entidad.</param>
        public Yotsuba GetEntity(string entityName) => EntityManager.YotsubaEntities.FirstOrDefault(x => x.Name == entityName);

        #endregion

        #region Audio Methods

        /// <summary>
        /// Plays a sound effect by name with default settings.
        /// Reproduce un efecto de sonido por nombre con configuración predeterminada.
        /// </summary>
        /// <param name="name">The name of the sound effect asset.</param>
        /// <returns>True if the sound was played successfully.</returns>
        public bool PlaySound(string name) => AudioSystem.PlaySound(name);

        /// <summary>
        /// Plays a sound effect by name with specified volume.
        /// Reproduce un efecto de sonido por nombre con volumen especificado.
        /// </summary>
        /// <param name="name">The name of the sound effect asset.</param>
        /// <param name="volume">Volume (0.0 to 1.0).</param>
        /// <returns>True if the sound was played successfully.</returns>
        public bool PlaySound(string name, float volume) => AudioSystem.PlaySound(name, volume);

        /// <summary>
        /// Plays a sound effect by name with full control.
        /// Reproduce un efecto de sonido por nombre con control total.
        /// </summary>
        /// <param name="name">The name of the sound effect asset.</param>
        /// <param name="volume">Volume (0.0 to 1.0).</param>
        /// <param name="pitch">Pitch adjustment (-1.0 to 1.0).</param>
        /// <param name="pan">Pan position (-1.0 left to 1.0 right).</param>
        /// <returns>True if the sound was played successfully.</returns>
        public bool PlaySound(string name, float volume, float pitch, float pan) => AudioSystem.PlaySound(name, volume, pitch, pan);

        /// <summary>
        /// Creates a sound effect instance for advanced control (loop, pause, etc.).
        /// Crea una instancia de efecto de sonido para control avanzado (loop, pausa, etc.).
        /// </summary>
        /// <param name="name">The name of the sound effect asset.</param>
        /// <returns>A SoundEffectInstance or null if not found.</returns>
        public SoundEffectInstance CreateSoundInstance(string name) => AudioSystem.CreateSoundInstance(name);

        /// <summary>
        /// Plays a song by name.
        /// Reproduce una canción por nombre.
        /// </summary>
        /// <param name="name">The name of the song asset.</param>
        /// <param name="loop">Whether to loop the song.</param>
        /// <returns>True if the song started playing successfully.</returns>
        public bool PlayMusic(string name, bool loop = true) => AudioSystem.PlayMusic(name, loop);

        /// <summary>
        /// Stops the currently playing music.
        /// Detiene la música que se está reproduciendo actualmente.
        /// </summary>
        public void StopMusic() => AudioSystem.StopMusic();

        /// <summary>
        /// Pauses the currently playing music.
        /// Pausa la música que se está reproduciendo actualmente.
        /// </summary>
        public void PauseMusic() => AudioSystem.PauseMusic();

        /// <summary>
        /// Resumes paused music.
        /// Reanuda la música pausada.
        /// </summary>
        public void ResumeMusic() => AudioSystem.ResumeMusic();

        #endregion

        #region Input Management Methods

        /// <summary>
        /// Binds a keyboard key to an action for the current entity.
        /// Vincula una tecla del teclado a una acción para la entidad actual.
        /// </summary>
        /// <param name="action">Action to bind. Acción a vincular.</param>
        /// <param name="key">Keyboard key. Tecla del teclado.</param>
        public void BindKeyboard(ActionEntityInput action, Keys key)
            => InputHelpers.BindKeyboard(Entity, action, key);

        /// <summary>
        /// Binds a keyboard key to an action for a specific entity.
        /// Vincula una tecla del teclado a una acción para una entidad específica.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <param name="action">Action to bind. Acción a vincular.</param>
        /// <param name="key">Keyboard key. Tecla del teclado.</param>
        public void BindKeyboard(Yotsuba entity, ActionEntityInput action, Keys key)
            => InputHelpers.BindKeyboard(entity, action, key);

        /// <summary>
        /// Unbinds a keyboard key from an action for the current entity.
        /// Desvincula una tecla del teclado de una acción para la entidad actual.
        /// </summary>
        /// <param name="action">Action to unbind. Acción a desvincular.</param>
        public void UnbindKeyboard(ActionEntityInput action)
            => InputHelpers.UnbindKeyboard(Entity, action);

        /// <summary>
        /// Binds a gamepad button to an action for the current entity.
        /// Vincula un botón del gamepad a una acción para la entidad actual.
        /// </summary>
        /// <param name="action">Action to bind. Acción a vincular.</param>
        /// <param name="button">Gamepad button. Botón del gamepad.</param>
        public void BindGamePad(ActionEntityInput action, Buttons button)
            => InputHelpers.BindGamePad(Entity, action, button);

        /// <summary>
        /// Binds a gamepad button to an action for a specific entity.
        /// Vincula un botón del gamepad a una acción para una entidad específica.
        /// </summary>
        /// <param name="entity">Target entity. Entidad objetivo.</param>
        /// <param name="action">Action to bind. Acción a vincular.</param>
        /// <param name="button">Gamepad button. Botón del gamepad.</param>
        public void BindGamePad(Yotsuba entity, ActionEntityInput action, Buttons button)
            => InputHelpers.BindGamePad(entity, action, button);

        /// <summary>
        /// Unbinds a gamepad button from an action for the current entity.
        /// Desvincula un botón del gamepad de una acción para la entidad actual.
        /// </summary>
        /// <param name="action">Action to unbind. Acción a desvincular.</param>
        public void UnbindGamePad(ActionEntityInput action)
            => InputHelpers.UnbindGamePad(Entity, action);

        /// <summary>
        /// Sets the gamepad player index for the current entity.
        /// Establece el índice del jugador del gamepad para la entidad actual.
        /// </summary>
        /// <param name="playerIndex">Player index (0-3). Índice del jugador (0-3).</param>
        public void SetGamePadIndex(PlayerIndex playerIndex)
            => InputHelpers.SetGamePadIndex(Entity, playerIndex);

        /// <summary>
        /// Binds a mouse button to an action for the current entity.
        /// Vincula un botón del mouse a una acción para la entidad actual.
        /// </summary>
        /// <param name="action">Action to bind. Acción a vincular.</param>
        /// <param name="button">Mouse button. Botón del mouse.</param>
        public void BindMouse(ActionEntityInput action, MouseButton button)
            => InputHelpers.BindMouse(Entity, action, button);

        /// <summary>
        /// Unbinds a mouse button from an action for the current entity.
        /// Desvincula un botón del mouse de una acción para la entidad actual.
        /// </summary>
        /// <param name="action">Action to unbind. Acción a desvincular.</param>
        public void UnbindMouse(ActionEntityInput action)
            => InputHelpers.UnbindMouse(Entity, action);

        /// <summary>
        /// Sets up default WASD keyboard bindings for the current entity.
        /// Configura los bindings WASD por defecto para la entidad actual.
        /// </summary>
        public void SetupDefaultWASD()
            => InputHelpers.SetupDefaultWASD(Entity);

        /// <summary>
        /// Sets up default gamepad bindings for the current entity.
        /// Configura los bindings de gamepad por defecto para la entidad actual.
        /// </summary>
        /// <param name="playerIndex">Player index for the gamepad. Índice del jugador del gamepad.</param>
        public void SetupDefaultGamePad(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.SetupDefaultGamePad(Entity, playerIndex);

        /// <summary>
        /// Sets up both default WASD and gamepad bindings for the current entity.
        /// Configura los bindings de WASD y gamepad por defecto para la entidad actual.
        /// </summary>
        /// <param name="playerIndex">Player index for the gamepad. Índice del jugador del gamepad.</param>
        public void SetupDefaultControls(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.SetupDefaultControls(Entity, playerIndex);

        /// <summary>
        /// Gets the current position of the left thumbstick for a specific player.
        /// Obtiene la posición actual del thumbstick izquierdo para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <returns>Thumbstick position as Vector2. Posición del thumbstick como Vector2.</returns>
        public Vector2 GetLeftThumbstick(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.GetLeftThumbstick(playerIndex);

        /// <summary>
        /// Gets the current position of the right thumbstick for a specific player.
        /// Obtiene la posición actual del thumbstick derecho para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <returns>Thumbstick position as Vector2. Posición del thumbstick como Vector2.</returns>
        public Vector2 GetRightThumbstick(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.GetRightThumbstick(playerIndex);

        /// <summary>
        /// Checks if a gamepad is connected for a specific player.
        /// Comprueba si hay un gamepad conectado para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <returns>True if gamepad is connected. True si el gamepad está conectado.</returns>
        public bool IsGamePadConnected(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.IsGamePadConnected(playerIndex);

        /// <summary>
        /// Gets the left trigger value for a specific player.
        /// Obtiene el valor del gatillo izquierdo para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <returns>Trigger value (0.0 to 1.0). Valor del gatillo (0.0 a 1.0).</returns>
        public float GetLeftTrigger(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.GetLeftTrigger(playerIndex);

        /// <summary>
        /// Gets the right trigger value for a specific player.
        /// Obtiene el valor del gatillo derecho para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <returns>Trigger value (0.0 to 1.0). Valor del gatillo (0.0 a 1.0).</returns>
        public float GetRightTrigger(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.GetRightTrigger(playerIndex);

        /// <summary>
        /// Sets gamepad vibration for a specific player.
        /// Establece la vibración del gamepad para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        /// <param name="strength">Vibration strength (0.0 to 1.0). Fuerza de vibración (0.0 a 1.0).</param>
        /// <param name="duration">Duration in seconds. Duración en segundos.</param>
        public void SetVibration(PlayerIndex playerIndex, float strength, float duration)
            => InputHelpers.SetVibration(playerIndex, strength, duration);

        /// <summary>
        /// Stops gamepad vibration for a specific player.
        /// Detiene la vibración del gamepad para un jugador específico.
        /// </summary>
        /// <param name="playerIndex">Player index. Índice del jugador.</param>
        public void StopVibration(PlayerIndex playerIndex)
            => InputHelpers.StopVibration(playerIndex);

        #endregion

        #region Resource Methods

        /// <summary>
        /// Metodo para cargar Assets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Load<T>(string path)
        {
            return YTBGlobalState.ContentManager.Load<T>(path);
        }

        /// <summary>
        ///     This method attempts to load the asset based on the System.Globalization.CultureInfo.CurrentCulture
        ///     searching for the asset by name and appending it with with the culture name (e.g.
        ///     "assetName.en-US") or two letter ISO language name (e.g. "assetName.en"). If
        ///     unsuccessful in finding the asset with the culture information appended, it will
        ///     fall back to loading the default asset.
        ///
        ///     Before a ContentManager can load an asset, you need to add the asset to your
        ///     game project using the steps described in Adding Content - MonoGame.
        ///     </summary>
        public T LoadLocalized<T>(string path)
        {
            return YTBGlobalState.ContentManager.LoadLocalized<T>(path);
        }
        #endregion


        public void SendLog(string message, Color color)
        {
            EngineUISystem.SendLog(message, color);
        }
    }
}
