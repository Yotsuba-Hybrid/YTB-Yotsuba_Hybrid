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
    /// Clase base para todos los objetos scriptables en el motor. Para recibir eventos, la clase derivada debe implementar las interfaces correspondientes (ej. ICollisionListener, IKeyboardListener, etc.).
    /// <para>Base class for all scriptable objects in the engine. To receive events, the derived class must implement the corresponding interfaces (e.g., ICollisionListener, IKeyboardListener, etc.).</para>
    /// </summary>
    public abstract class BaseScript
    {
        #region Field Properties
        /// <summary>
        /// Ruta de este archivo.
        /// <para>Path to this file.</para>
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// Referencia a la Instancia del manejador de eventos. 
        /// Con esta variable puedes enviar notificaciones a cualquier parte 
        /// del juego y recibirlas al suscribirte a ellas.
        /// <para>Reference to the event manager instance so you can publish and receive notifications.</para>
        /// </summary>
        public EventManager EventManager = EventManager.Instance;

        /// <summary>
        /// Referencia al manejador de entidades del juego. 
        /// Con esto, puedes acceder a todas las entidades de la escena
        /// <para>Reference to the game entity manager to access all scene entities.</para>
        /// </summary>
        public EntityManager EntityManager { get; set; }

        /// <summary>
        /// Referencia a la entidad del script.
        /// <para>Reference to the script entity.</para>
        /// </summary>
        public Yotsuba Entity { get; set; }
        #endregion

        /// <summary>
        /// Inicializa el script y suscribe las interfaces detectadas.
        /// <para>Initializes the script and subscribes detected interfaces.</para>
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
        /// <para>Updates the script logic and is called every frame.</para>
        /// </summary>
        /// <param name="gametime">Tiempo de juego. <para>Game time.</para></param>
        public virtual void Update(GameTime gametime)
        {
            // Opcional: Implementación base vacía
        }

        /// <summary>
        /// Permite dibujar contenido personalizado en la UI o pantalla en 2D.
        /// <para>Allows drawing custom 2D content in the UI or screen.</para>
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch compartido. <para>Shared sprite batch.</para></param>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public virtual void Draw2D(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        /// <summary>
        /// Dibuja el contenido 3D del frame actual usando la información de tiempo especificada.
        /// <para>Draws the 3D content for the current frame using the specified game time information.</para>
        /// </summary>
        /// <remarks>Override this method in a derived class to implement custom 3D rendering logic. This
        /// method is typically called once per frame during the game's draw cycle.</remarks>
        /// <param name="gameTime">Tiempo de juego del frame actual. <para>An object that provides a snapshot of timing values, including elapsed and total game time, for the current frame.</para></param>
        public virtual void Draw3D(GameTime gameTime)
        {
        }

        /// <summary>
        /// Libera los recursos del script.
        /// <para>Releases script resources.</para>
        /// </summary>
        public virtual void Cleanup()
        {
            GC.Collect();
        }

        #region Entity Manipulation Methods

        /// <summary>
        /// Solicita cambiar de escena inmediatamente.
        /// <para>Requests an immediate scene change.</para>
        /// </summary>
        /// <param name="sceneName">Nombre de la escena destino. <para>Target scene name.</para></param>
        public void ChangeScene(string sceneName) => SystemCall.ChangeScene(sceneName);

        /// <summary>
        /// Mueve la entidad actual a una posición específica inmediatamente.
        /// <para>Moves the current entity to a specific position immediately.</para>
        /// </summary>
        /// <param name="Target">Nueva posición en el mundo. <para>New world position.</para></param>
        public void MoveEntityTo(Vector3 Target)
        {
            ref TransformComponent transform = ref EntityManager.TransformComponents[Entity.Id];
            transform.Position = Target;
        }

        /// <summary>
        /// Mueve una entidad por id a una posición específica inmediatamente.
        /// <para>Moves an entity by id to a specific position immediately.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <param name="Target">Nueva posición en el mundo. <para>New world position.</para></param>
        public void MoveEntityTo(int entityId, Vector3 Target)
        {
            ref TransformComponent transform = ref EntityManager.TransformComponents[entityId];
            transform.Position = Target;
        }

        /// <summary>
        /// Mueve una entidad específica a una posición inmediatamente.
        /// <para>Moves a specific entity to a position immediately.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="Target">Nueva posición en el mundo. <para>New world position.</para></param>
        public void MoveEntityTo(Yotsuba entity, Vector3 Target)
        {
            ref TransformComponent transform = ref EntityManager.TransformComponents[entity.Id];
            transform.Position = Target;
        }

        /// <summary>
        /// Aplica una fuerza de movimiento a la entidad actual (requiere RigidBody).
        /// <para>Applies a movement force to the current entity (requires a RigidBody).</para>
        /// </summary>
        /// <param name="Velocity">Vector de velocidad a sumar. <para>Velocity vector to add.</para></param>
        public void ApplyMovement(Vector2 Velocity)
        {
            if (!Entity.HasComponent(YTBComponent.Rigibody))
            {
                throw new Exception($"La entidad {Entity.Name} con id {Entity.Id} no tiene un RigidBody2D para aplicar movimiento. Esta entidad esta en la escena: \"{YTBGlobalState.Game.SceneManager.CurrentScene.SceneName}\"");
            }
            ref RigidBodyComponent2D rigidBody = ref EntityManager.Rigidbody2DComponents[Entity.Id];
            rigidBody.Velocity += new Vector3(Velocity.X, Velocity.Y, 0);
        }

        /// <summary>
        /// Aplica una fuerza de movimiento a una entidad por id (requiere RigidBody).
        /// <para>Applies a movement force to an entity by id (requires a RigidBody).</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <param name="Velocity">Vector de velocidad a sumar. <para>Velocity vector to add.</para></param>
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

        /// <summary>
        /// Aplica una fuerza de movimiento a una entidad específica (requiere RigidBody).
        /// <para>Applies a movement force to a specific entity (requires a RigidBody).</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="Velocity">Vector de velocidad a sumar. <para>Velocity vector to add.</para></param>
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
        /// Obtiene el componente de cuerpo rígido para la entidad actual.
        /// <para>Gets the rigid body component for the current entity.</para>
        /// </summary>
        /// <returns>Componente de cuerpo rígido. <para>Rigid body component.</para></returns>
        public ref RigidBodyComponent2D GetRigidBody2D() => ref EntityManager.Rigidbody2DComponents[Entity.Id];
        /// <summary>
        /// Obtiene el componente de cuerpo rígido para un id de entidad específico.
        /// <para>Gets the rigid body component for a specific entity id.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Componente de cuerpo rígido. <para>Rigid body component.</para></returns>
        public ref RigidBodyComponent2D GetRigidBody2D(int entityId) => ref EntityManager.Rigidbody2DComponents[entityId];
        /// <summary>
        /// Obtiene el componente de cuerpo rígido para una instancia de entidad específica.
        /// <para>Gets the rigid body component for a specific entity instance.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        /// <returns>Componente de cuerpo rígido. <para>Rigid body component.</para></returns>
        public ref RigidBodyComponent2D GetRigidBody2D(Yotsuba entity) => ref EntityManager.Rigidbody2DComponents[entity.Id];

        /// <summary>
        /// Obtiene el componente de animación para la entidad actual.
        /// <para>Gets the animation component for the current entity.</para>
        /// </summary>
        /// <returns>Componente de animación. <para>Animation component.</para></returns>
        public ref AnimationComponent2D GetAnimationComponent() => ref EntityManager.Animation2DComponents[Entity.Id];
        /// <summary>
        /// Obtiene el componente de animación para un id de entidad específico.
        /// <para>Gets the animation component for a specific entity id.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Componente de animación. <para>Animation component.</para></returns>
        public ref AnimationComponent2D GetAnimationComponent(int entityId) => ref EntityManager.Animation2DComponents[entityId];
        /// <summary>
        /// Obtiene el componente de animación para una instancia de entidad específica.
        /// <para>Gets the animation component for a specific entity instance.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        /// <returns>Componente de animación. <para>Animation component.</para></returns>
        public ref AnimationComponent2D GetAnimationComponent(Yotsuba entity) => ref EntityManager.Animation2DComponents[entity.Id];

        /// <summary>
        /// Obtiene el componente de sprite para la entidad actual.
        /// <para>Gets the sprite component for the current entity.</para>
        /// </summary>
        /// <returns>Componente de sprite. <para>Sprite component.</para></returns>
        public ref SpriteComponent2D GetSpriteComponent() => ref EntityManager.Sprite2DComponents[Entity.Id];
        /// <summary>
        /// Obtiene el componente de sprite para un id de entidad específico.
        /// <para>Gets the sprite component for a specific entity id.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Componente de sprite. <para>Sprite component.</para></returns>
        public ref SpriteComponent2D GetSpriteComponent(int entityId) => ref EntityManager.Sprite2DComponents[entityId];
        /// <summary>
        /// Obtiene el componente de sprite para una instancia de entidad específica.
        /// <para>Gets the sprite component for a specific entity instance.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        /// <returns>Componente de sprite. <para>Sprite component.</para></returns>
        public ref SpriteComponent2D GetSpriteComponent(Yotsuba entity) => ref EntityManager.Sprite2DComponents[entity.Id];

        /// <summary>
        /// Obtiene el componente de transformación para la entidad actual.
        /// <para>Gets the transform component for the current entity.</para>
        /// </summary>
        /// <returns>Componente de transformación. <para>Transform component.</para></returns>
        public ref TransformComponent GetTransformComponent() => ref EntityManager.TransformComponents[Entity.Id];
        /// <summary>
        /// Obtiene el componente de transformación para un id de entidad específico.
        /// <para>Gets the transform component for a specific entity id.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Componente de transformación. <para>Transform component.</para></returns>
        public ref TransformComponent GetTransformComponent(int entityId) => ref EntityManager.TransformComponents[entityId];
        /// <summary>
        /// Obtiene el componente de transformación para una instancia de entidad específica.
        /// <para>Gets the transform component for a specific entity instance.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        /// <returns>Componente de transformación. <para>Transform component.</para></returns>
        public ref TransformComponent GetTransformComponent(Yotsuba entity) => ref EntityManager.TransformComponents[entity.Id];

        /// <summary>
        /// Obtiene el componente de fuente para la entidad actual.
        /// <para>Gets the font component for the current entity.</para>
        /// </summary>
        /// <returns>Componente de fuente. <para>Font component.</para></returns>
        public ref FontComponent2D GetFontComponent() => ref EntityManager.Font2DComponents[Entity.Id];
        /// <summary>
        /// Obtiene el componente de fuente para un id de entidad específico.
        /// <para>Gets the font component for a specific entity id.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Componente de fuente. <para>Font component.</para></returns>
        public ref FontComponent2D GetFontComponent(int entityId) => ref EntityManager.Font2DComponents[entityId];
        /// <summary>
        /// Obtiene el componente de fuente para una instancia de entidad específica.
        /// <para>Gets the font component for a specific entity instance.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        /// <returns>Componente de fuente. <para>Font component.</para></returns>
        public ref FontComponent2D GetFontComponent(Yotsuba entity) => ref EntityManager.Font2DComponents[entity.Id];

        /// <summary>
        /// Obtiene el componente de botón para la entidad actual.
        /// <para>Gets the button component for the current entity.</para>
        /// </summary>
        /// <returns>Componente de botón. <para>Button component.</para></returns>
        public ButtonComponent2D GetButtonComponent() => EntityManager.Button2DComponents[Entity.Id];
        /// <summary>
        /// Obtiene el componente de botón para un id de entidad específico.
        /// <para>Gets the button component for a specific entity id.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Componente de botón. <para>Button component.</para></returns>
        public ButtonComponent2D GetButtonComponent(int entityId) => EntityManager.Button2DComponents[entityId];
        /// <summary>
        /// Obtiene el componente de botón para una instancia de entidad específica.
        /// <para>Gets the button component for a specific entity instance.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        /// <returns>Componente de botón. <para>Button component.</para></returns>
        public ButtonComponent2D GetButtonComponent(Yotsuba entity) => EntityManager.Button2DComponents[entity.Id];

        /// <summary>
        /// Obtiene el componente de tilemap para la entidad actual.
        /// <para>Gets the tile map component for the current entity.</para>
        /// </summary>
        /// <returns>Componente de tilemap. <para>Tile map component.</para></returns>
        public ref TileMapComponent2D GetTilemapComponent() => ref EntityManager.TileMapComponent2Ds[Entity.Id];
        /// <summary>
        /// Obtiene el componente de tilemap para un id de entidad específico.
        /// <para>Gets the tile map component for a specific entity id.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Componente de tilemap. <para>Tile map component.</para></returns>
        public ref TileMapComponent2D GetTilemapComponent(int entityId) => ref EntityManager.TileMapComponent2Ds[entityId];
        /// <summary>
        /// Obtiene el componente de tilemap para una instancia de entidad específica.
        /// <para>Gets the tile map component for a specific entity instance.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        /// <returns>Componente de tilemap. <para>Tile map component.</para></returns>
        public ref TileMapComponent2D GetTilemapComponent(Yotsuba entity) => ref EntityManager.TileMapComponent2Ds[entity.Id];

        /// <summary>
        /// Obtiene el componente de shader para la entidad actual.
        /// <para>Gets the shader component for the current entity.</para>
        /// </summary>
        /// <returns>Componente de shader. <para>Shader component.</para></returns>
        public ref ShaderComponent GetShaderComponent() => ref EntityManager.ShaderComponents[Entity.Id];
        /// <summary>
        /// Obtiene el componente de shader para un id de entidad específico.
        /// <para>Gets the shader component for a specific entity id.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Componente de shader. <para>Shader component.</para></returns>
        public ref ShaderComponent GetShaderComponent(int entityId) => ref EntityManager.ShaderComponents[entityId];
        /// <summary>
        /// Obtiene el componente de shader para una instancia de entidad específica.
        /// <para>Gets the shader component for a specific entity instance.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        /// <returns>Componente de shader. <para>Shader component.</para></returns>
        public ref ShaderComponent GetShaderComponent(Yotsuba entity) => ref EntityManager.ShaderComponents[entity.Id];

        /// <summary>
        /// Obtiene el componente de entrada para la entidad actual.
        /// <para>Gets the input component for the current entity.</para>
        /// </summary>
        /// <returns>Componente de entrada. <para>Input component.</para></returns>
        public ref InputComponent GetInputComponent() => ref EntityManager.InputComponents[Entity.Id];
        /// <summary>
        /// Obtiene el componente de entrada para un id de entidad específico.
        /// <para>Gets the input component for a specific entity id.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Componente de entrada. <para>Input component.</para></returns>
        public ref InputComponent GetInputComponent(int entityId) => ref EntityManager.InputComponents[entityId];
        /// <summary>
        /// Obtiene el componente de entrada para una instancia de entidad específica.
        /// <para>Gets the input component for a specific entity instance.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        /// <returns>Componente de entrada. <para>Input component.</para></returns>
        public ref InputComponent GetInputComponent(Yotsuba entity) => ref EntityManager.InputComponents[entity.Id];

        /// <summary>
        /// Obtiene el componente de script para la entidad actual.
        /// <para>Gets the script component for the current entity.</para>
        /// </summary>
        /// <returns>Componente de script. <para>Script component.</para></returns>
        public ref ScriptComponent GetScriptComponent() => ref EntityManager.ScriptComponents[Entity.Id];
        /// <summary>
        /// Obtiene el componente de script para un id de entidad específico.
        /// <para>Gets the script component for a specific entity id.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Componente de script. <para>Script component.</para></returns>
        public ref ScriptComponent GetScriptComponent(int entityId) => ref EntityManager.ScriptComponents[entityId];
        /// <summary>
        /// Obtiene el componente de script para una instancia de entidad específica.
        /// <para>Gets the script component for a specific entity instance.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        /// <returns>Componente de script. <para>Script component.</para></returns>
        public ref ScriptComponent GetScriptComponent(Yotsuba entity) => ref EntityManager.ScriptComponents[entity.Id];

        /// <summary>
        /// Obtiene el componente de cámara activo.
        /// <para>Gets the active camera component.</para>
        /// </summary>
        /// <returns>Componente de cámara activo. <para>Active camera component.</para></returns>
        public CameraComponent3D GetCamera() => EntityManager.Camera;
        /// <summary>
        /// Obtiene el componente de cámara activo.
        /// <para>Gets the active camera component.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Componente de cámara activo. <para>Active camera component.</para></returns>
        public CameraComponent3D GetCamera(int entityId) => EntityManager.Camera;
        /// <summary>
        /// Obtiene el componente de cámara activo.
        /// <para>Gets the active camera component.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        /// <returns>Componente de cámara activo. <para>Active camera component.</para></returns>
        public CameraComponent3D GetCamera(Yotsuba entity) => EntityManager.Camera;

        /// <summary>
        /// Obtiene la entidad actual del script.
        /// <para>Gets the current script entity.</para>
        /// </summary>
        /// <returns>Entidad del script. <para>Script entity.</para></returns>
        public Yotsuba GetEntity() => Entity;

        /// <summary>
        /// Obtiene una entidad por su identificador.
        /// <para>Gets an entity by its identifier.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <returns>Entidad encontrada o null. <para>Found entity or null.</para></returns>
        public Yotsuba GetEntity(int entityId) => EntityManager.YotsubaEntities.FirstOrDefault(x => x.Id == entityId);

        /// <summary>
        /// Obtiene una entidad por su nombre.
        /// <para>Gets an entity by its name.</para>
        /// </summary>
        /// <param name="entityName">Nombre de la entidad. <para>Entity name.</para></param>
        /// <returns>Entidad encontrada o null. <para>Found entity or null.</para></returns>
        public Yotsuba GetEntity(string entityName) => EntityManager.YotsubaEntities.FirstOrDefault(x => x.Name == entityName);

        #endregion

        #region Audio Methods

        /// <summary>
        /// Reproduce un efecto de sonido por nombre con configuración predeterminada.
        /// <para>Plays a sound effect by name with default settings.</para>
        /// </summary>
        /// <param name="name">Nombre del asset de sonido. <para>The name of the sound effect asset.</para></param>
        /// <returns>True si el sonido se reprodujo correctamente. <para>True if the sound was played successfully.</para></returns>
        public bool PlaySound(string name) => AudioSystem.PlaySound(name);

        /// <summary>
        /// Reproduce un efecto de sonido por nombre con volumen especificado.
        /// <para>Plays a sound effect by name with specified volume.</para>
        /// </summary>
        /// <param name="name">Nombre del asset de sonido. <para>The name of the sound effect asset.</para></param>
        /// <param name="volume">Volumen (0.0 a 1.0). <para>Volume (0.0 to 1.0).</para></param>
        /// <returns>True si el sonido se reprodujo correctamente. <para>True if the sound was played successfully.</para></returns>
        public bool PlaySound(string name, float volume) => AudioSystem.PlaySound(name, volume);

        /// <summary>
        /// Reproduce un efecto de sonido por nombre con control total.
        /// <para>Plays a sound effect by name with full control.</para>
        /// </summary>
        /// <param name="name">Nombre del asset de sonido. <para>The name of the sound effect asset.</para></param>
        /// <param name="volume">Volumen (0.0 a 1.0). <para>Volume (0.0 to 1.0).</para></param>
        /// <param name="pitch">Ajuste de tono (-1.0 a 1.0). <para>Pitch adjustment (-1.0 to 1.0).</para></param>
        /// <param name="pan">Panorámica (-1.0 izquierda a 1.0 derecha). <para>Pan position (-1.0 left to 1.0 right).</para></param>
        /// <returns>True si el sonido se reprodujo correctamente. <para>True if the sound was played successfully.</para></returns>
        public bool PlaySound(string name, float volume, float pitch, float pan) => AudioSystem.PlaySound(name, volume, pitch, pan);

        /// <summary>
        /// Crea una instancia de efecto de sonido para control avanzado (loop, pausa, etc.).
        /// <para>Creates a sound effect instance for advanced control (loop, pause, etc.).</para>
        /// </summary>
        /// <param name="name">Nombre del asset de sonido. <para>The name of the sound effect asset.</para></param>
        /// <returns>Instancia de sonido o null si no se encuentra. <para>A SoundEffectInstance or null if not found.</para></returns>
        public SoundEffectInstance CreateSoundInstance(string name) => AudioSystem.CreateSoundInstance(name);

        /// <summary>
        /// Reproduce una canción por nombre.
        /// <para>Plays a song by name.</para>
        /// </summary>
        /// <param name="name">Nombre del asset de la canción. <para>The name of the song asset.</para></param>
        /// <param name="loop">Indica si debe repetirse. <para>Whether to loop the song.</para></param>
        /// <returns>True si la canción empezó a reproducirse. <para>True if the song started playing successfully.</para></returns>
        public bool PlayMusic(string name, bool loop = true) => AudioSystem.PlayMusic(name, loop);

        /// <summary>
        /// Detiene la música que se está reproduciendo actualmente.
        /// <para>Stops the currently playing music.</para>
        /// </summary>
        public void StopMusic() => AudioSystem.StopMusic();

        /// <summary>
        /// Pausa la música que se está reproduciendo actualmente.
        /// <para>Pauses the currently playing music.</para>
        /// </summary>
        public void PauseMusic() => AudioSystem.PauseMusic();

        /// <summary>
        /// Reanuda la música pausada.
        /// <para>Resumes paused music.</para>
        /// </summary>
        public void ResumeMusic() => AudioSystem.ResumeMusic();

        #endregion

        #region Input Management Methods

        /// <summary>
        /// Vincula una tecla del teclado a una acción para la entidad actual.
        /// <para>Binds a keyboard key to an action for the current entity.</para>
        /// </summary>
        /// <param name="action">Acción a vincular. <para>Action to bind.</para></param>
        /// <param name="key">Tecla del teclado. <para>Keyboard key.</para></param>
        public void BindKeyboard(ActionEntityInput action, Keys key)
            => InputHelpers.BindKeyboard(Entity, action, key);

        /// <summary>
        /// Vincula una tecla del teclado a una acción para una entidad específica.
        /// <para>Binds a keyboard key to an action for a specific entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="action">Acción a vincular. <para>Action to bind.</para></param>
        /// <param name="key">Tecla del teclado. <para>Keyboard key.</para></param>
        public void BindKeyboard(Yotsuba entity, ActionEntityInput action, Keys key)
            => InputHelpers.BindKeyboard(entity, action, key);

        /// <summary>
        /// Desvincula una tecla del teclado de una acción para la entidad actual.
        /// <para>Unbinds a keyboard key from an action for the current entity.</para>
        /// </summary>
        /// <param name="action">Acción a desvincular. <para>Action to unbind.</para></param>
        public void UnbindKeyboard(ActionEntityInput action)
            => InputHelpers.UnbindKeyboard(Entity, action);

        /// <summary>
        /// Vincula un botón del gamepad a una acción para la entidad actual.
        /// <para>Binds a gamepad button to an action for the current entity.</para>
        /// </summary>
        /// <param name="action">Acción a vincular. <para>Action to bind.</para></param>
        /// <param name="button">Botón del gamepad. <para>Gamepad button.</para></param>
        public void BindGamePad(ActionEntityInput action, Buttons button)
            => InputHelpers.BindGamePad(Entity, action, button);

        /// <summary>
        /// Vincula un botón del gamepad a una acción para una entidad específica.
        /// <para>Binds a gamepad button to an action for a specific entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="action">Acción a vincular. <para>Action to bind.</para></param>
        /// <param name="button">Botón del gamepad. <para>Gamepad button.</para></param>
        public void BindGamePad(Yotsuba entity, ActionEntityInput action, Buttons button)
            => InputHelpers.BindGamePad(entity, action, button);

        /// <summary>
        /// Desvincula un botón del gamepad de una acción para la entidad actual.
        /// <para>Unbinds a gamepad button from an action for the current entity.</para>
        /// </summary>
        /// <param name="action">Acción a desvincular. <para>Action to unbind.</para></param>
        public void UnbindGamePad(ActionEntityInput action)
            => InputHelpers.UnbindGamePad(Entity, action);

        /// <summary>
        /// Establece el índice del jugador del gamepad para la entidad actual.
        /// <para>Sets the gamepad player index for the current entity.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador (0-3). <para>Player index (0-3).</para></param>
        public void SetGamePadIndex(PlayerIndex playerIndex)
            => InputHelpers.SetGamePadIndex(Entity, playerIndex);

        /// <summary>
        /// Vincula un botón del mouse a una acción para la entidad actual.
        /// <para>Binds a mouse button to an action for the current entity.</para>
        /// </summary>
        /// <param name="action">Acción a vincular. <para>Action to bind.</para></param>
        /// <param name="button">Botón del mouse. <para>Mouse button.</para></param>
        public void BindMouse(ActionEntityInput action, MouseButton button)
            => InputHelpers.BindMouse(Entity, action, button);

        /// <summary>
        /// Desvincula un botón del mouse de una acción para la entidad actual.
        /// <para>Unbinds a mouse button from an action for the current entity.</para>
        /// </summary>
        /// <param name="action">Acción a desvincular. <para>Action to unbind.</para></param>
        public void UnbindMouse(ActionEntityInput action)
            => InputHelpers.UnbindMouse(Entity, action);

        /// <summary>
        /// Configura los bindings WASD por defecto para la entidad actual.
        /// <para>Sets up default WASD keyboard bindings for the current entity.</para>
        /// </summary>
        public void SetupDefaultWASD()
            => InputHelpers.SetupDefaultWASD(Entity);

        /// <summary>
        /// Configura los bindings de gamepad por defecto para la entidad actual.
        /// <para>Sets up default gamepad bindings for the current entity.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador del gamepad. <para>Player index for the gamepad.</para></param>
        public void SetupDefaultGamePad(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.SetupDefaultGamePad(Entity, playerIndex);

        /// <summary>
        /// Configura los bindings de WASD y gamepad por defecto para la entidad actual.
        /// <para>Sets up both default WASD and gamepad bindings for the current entity.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador del gamepad. <para>Player index for the gamepad.</para></param>
        public void SetupDefaultControls(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.SetupDefaultControls(Entity, playerIndex);

        /// <summary>
        /// Obtiene la posición actual del thumbstick izquierdo para un jugador específico.
        /// <para>Gets the current position of the left thumbstick for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <returns>Posición del thumbstick como Vector2. <para>Thumbstick position as Vector2.</para></returns>
        public Vector2 GetLeftThumbstick(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.GetLeftThumbstick(playerIndex);

        /// <summary>
        /// Obtiene la posición actual del thumbstick derecho para un jugador específico.
        /// <para>Gets the current position of the right thumbstick for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <returns>Posición del thumbstick como Vector2. <para>Thumbstick position as Vector2.</para></returns>
        public Vector2 GetRightThumbstick(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.GetRightThumbstick(playerIndex);

        /// <summary>
        /// Comprueba si hay un gamepad conectado para un jugador específico.
        /// <para>Checks if a gamepad is connected for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <returns>True si el gamepad está conectado. <para>True if gamepad is connected.</para></returns>
        public bool IsGamePadConnected(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.IsGamePadConnected(playerIndex);

        /// <summary>
        /// Obtiene el valor del gatillo izquierdo para un jugador específico.
        /// <para>Gets the left trigger value for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <returns>Valor del gatillo (0.0 a 1.0). <para>Trigger value (0.0 to 1.0).</para></returns>
        public float GetLeftTrigger(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.GetLeftTrigger(playerIndex);

        /// <summary>
        /// Obtiene el valor del gatillo derecho para un jugador específico.
        /// <para>Gets the right trigger value for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <returns>Valor del gatillo (0.0 a 1.0). <para>Trigger value (0.0 to 1.0).</para></returns>
        public float GetRightTrigger(PlayerIndex playerIndex = PlayerIndex.One)
            => InputHelpers.GetRightTrigger(playerIndex);

        /// <summary>
        /// Establece la vibración del gamepad para un jugador específico.
        /// <para>Sets gamepad vibration for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        /// <param name="strength">Fuerza de vibración (0.0 a 1.0). <para>Vibration strength (0.0 to 1.0).</para></param>
        /// <param name="duration">Duración en segundos. <para>Duration in seconds.</para></param>
        public void SetVibration(PlayerIndex playerIndex, float strength, float duration)
            => InputHelpers.SetVibration(playerIndex, strength, duration);

        /// <summary>
        /// Detiene la vibración del gamepad para un jugador específico.
        /// <para>Stops gamepad vibration for a specific player.</para>
        /// </summary>
        /// <param name="playerIndex">Índice del jugador. <para>Player index.</para></param>
        public void StopVibration(PlayerIndex playerIndex)
            => InputHelpers.StopVibration(playerIndex);

        #endregion

        #region Resource Methods

        /// <summary>
        /// Método para cargar assets desde el ContentManager.
        /// <para>Loads assets from the ContentManager.</para>
        /// </summary>
        /// <typeparam name="T">Tipo del asset. <para>Asset type.</para></typeparam>
        /// <param name="path">Ruta del asset. <para>Asset path.</para></param>
        /// <returns>Asset cargado. <para>Loaded asset.</para></returns>
        public T Load<T>(string path)
        {
            return YTBGlobalState.ContentManager.Load<T>(path);
        }

        /// <summary>
        /// Intenta cargar un asset localizado según la cultura actual y usa el asset por defecto si no existe.
        /// <para>This method attempts to load the asset based on the current culture, falling back to the default asset if needed.</para>
        /// </summary>
        /// <typeparam name="T">Tipo del asset. <para>Asset type.</para></typeparam>
        /// <param name="path">Ruta base del asset. <para>Base asset path.</para></param>
        /// <returns>Asset cargado. <para>Loaded asset.</para></returns>
        public T LoadLocalized<T>(string path)
        {
            return YTBGlobalState.ContentManager.LoadLocalized<T>(path);
        }
        #endregion


        /// <summary>
        /// Envía un mensaje al sistema de UI del motor.
        /// <para>Sends a message to the engine UI system.</para>
        /// </summary>
        /// <param name="message">Mensaje a registrar. <para>Message to log.</para></param>
        /// <param name="color">Color del mensaje. <para>Message color.</para></param>
        public void SendLog(string message, Color color)
        {
            EngineUISystem.SendLog(message, color);
        }
    }
}
