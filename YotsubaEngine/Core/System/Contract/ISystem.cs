using Microsoft.Xna.Framework;
using System;
using System.Linq;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Core.System.Contract
{
    /// <summary>
    /// Define una clase abstracta común para los sistemas del motor.
    /// <para>Defines a common abstract class for engine systems.</para>
    /// </summary>
    public abstract class ISystem : IDisposable
    {

        /// <summary>
        /// Referencia al administrador de entidades de la escena actual.
        /// <para>Reference to the entity manager of the current scene.</para>
        /// </summary>
        public EntityManager EntityManager { get; set; }

        /// <summary>
        /// Inicializa el sistema con las referencias de entidades necesarias.
        /// <para>Initializes the system with required entity references.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public virtual void InitializeSystem(EntityManager @entities) { }

        /// <summary>
        /// Se ejecuta una vez por entidad durante la inicialización.
        /// <para>Runs once per entity during initialization.</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        public virtual void SharedEntityInitialize(ref Yotsuba @Entidad) { }

        /// <summary>
        /// Actualiza el sistema en cada frame.
        /// <para>Updates the system each frame.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public virtual void UpdateSystem(GameTime @gameTime) { }

        /// <summary>
        /// Se ejecuta en un bucle compartido que recorre todas las entidades para mejorar el rendimiento.
        /// <para>Runs during a shared loop across all entities for performance.</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        public virtual void SharedEntityForEachUpdate(ref Yotsuba @Entidad, GameTime time)
        {

        }

        /// <summary>
        /// Libera los recursos utilizados por el sistema.
        /// <para>Releases the resources used by the system.</para>
        /// </summary>
        public virtual void Dispose()
        {

        }

        // --- Entities ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todas las entidades registradas en el administrador.
        /// <para>Returns a <see cref="Span{T}"/> of all entities registered in the manager.</para>
        /// </summary>
        public Span<Yotsuba> GetEntitiesAsSpan()
        {
            return EntityManager.YotsubaEntities.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia a la entidad con el identificador especificado.
        /// <para>Returns a reference to the entity with the specified identifier.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref Yotsuba GetEntity(int entityId)
        {
            return ref EntityManager.YotsubaEntities[entityId];
        }

        /// <summary>
        /// Retorna una referencia a la primera entidad cuyo nombre coincida, o <c>default</c> si no se encuentra ninguna.
        /// <para>Returns a reference to the first entity whose name matches, or <c>default</c> if none is found.</para>
        /// </summary>
        /// <param name="name">Nombre de la entidad a buscar. <para>Name of the entity to look up.</para></param>
        /// <returns>
        /// Referencia a la entidad encontrada, o <c>default</c> si no existe ninguna con ese nombre.
        /// <para>Reference to the found entity, or <c>default</c> if no entity with that name exists.</para>
        /// </returns>
        public ref Yotsuba GetEntityByNameOrDefault(string name)
        {
            return ref EntityManager.YotsubaEntities.Find(x => x.Name == name);
        }

        // --- Transform ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de transformación.
        /// <para>Returns a <see cref="Span{T}"/> of all transform components.</para>
        /// </summary>
        public Span<TransformComponent> GetTransformComponentsAsSpan()
        {
            return EntityManager.TransformComponents.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de transformación de la entidad con el id especificado.
        /// <para>Returns a reference to the transform component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref TransformComponent GetTransform2DComponent(int entityId)
        {
            return ref EntityManager.TransformComponents[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de transformación de la entidad especificada.
        /// <para>Returns a reference to the transform component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref TransformComponent GetTransform2DComponent(ref Yotsuba entity)
        {
            return ref EntityManager.TransformComponents[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de transformación de la entidad especificada.
        /// <para>Returns a reference to the transform component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref TransformComponent GetTransform2DComponent(Yotsuba entity)
        {
            return ref EntityManager.TransformComponents[entity.Id];
        }

        // --- Sprite 2D ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de sprite 2D.
        /// <para>Returns a <see cref="Span{T}"/> of all 2D sprite components.</para>
        /// </summary>
        public Span<SpriteComponent2D> GetSpriteComponentsAsSpan()
        {
            return EntityManager.Sprite2DComponents.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de sprite 2D de la entidad con el id especificado.
        /// <para>Returns a reference to the 2D sprite component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref SpriteComponent2D GetSprite2DComponent(int entityId)
        {
            return ref EntityManager.Sprite2DComponents[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de sprite 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D sprite component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref SpriteComponent2D GetSprite2DComponent(ref Yotsuba entity)
        {
            return ref EntityManager.Sprite2DComponents[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de sprite 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D sprite component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref SpriteComponent2D GetSprite2DComponent(Yotsuba entity)
        {
            return ref EntityManager.Sprite2DComponents[entity.Id];
        }

        // --- TileMap 2D ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de mapa de tiles 2D.
        /// <para>Returns a <see cref="Span{T}"/> of all 2D tilemap components.</para>
        /// </summary>
        public Span<TileMapComponent2D> GetTilemapComponentsAsSpan()
        {
            return EntityManager.TileMapComponent2Ds.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de mapa de tiles 2D de la entidad con el id especificado.
        /// <para>Returns a reference to the 2D tilemap component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref TileMapComponent2D GetTilemapComponent(int entityId)
        {
            return ref EntityManager.TileMapComponent2Ds[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de mapa de tiles 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D tilemap component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref TileMapComponent2D GetTilemapComponent(ref Yotsuba entity)
        {
            return ref EntityManager.TileMapComponent2Ds[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de mapa de tiles 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D tilemap component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref TileMapComponent2D GetTilemapComponent(Yotsuba entity)
        {
            return ref EntityManager.TileMapComponent2Ds[entity.Id];
        }

        // --- Shader ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de shader.
        /// <para>Returns a <see cref="Span{T}"/> of all shader components.</para>
        /// </summary>
        public Span<ShaderComponent> GetShaderComponentsAsSpan()
        {
            return EntityManager.ShaderComponents.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de shader de la entidad con el id especificado.
        /// <para>Returns a reference to the shader component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref ShaderComponent GetShaderComponent(int entityId)
        {
            return ref EntityManager.ShaderComponents[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de shader de la entidad especificada.
        /// <para>Returns a reference to the shader component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref ShaderComponent GetShaderComponent(ref Yotsuba entity)
        {
            return ref EntityManager.ShaderComponents[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de shader de la entidad especificada.
        /// <para>Returns a reference to the shader component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref ShaderComponent GetShaderComponent(Yotsuba entity)
        {
            return ref EntityManager.ShaderComponents[entity.Id];
        }

        // --- Script ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de script.
        /// <para>Returns a <see cref="Span{T}"/> of all script components.</para>
        /// </summary>
        public Span<ScriptComponent> GetScriptComponentsAsSpan()
        {
            return EntityManager.ScriptComponents.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de script de la entidad con el id especificado.
        /// <para>Returns a reference to the script component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref ScriptComponent GetScriptComponent(int entityId)
        {
            return ref EntityManager.ScriptComponents[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de script de la entidad especificada.
        /// <para>Returns a reference to the script component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref ScriptComponent GetScriptComponent(ref Yotsuba entity)
        {
            return ref EntityManager.ScriptComponents[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de script de la entidad especificada.
        /// <para>Returns a reference to the script component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref ScriptComponent GetScriptComponent(Yotsuba entity)
        {
            return ref EntityManager.ScriptComponents[entity.Id];
        }

        // --- RigidBody 2D ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de cuerpo rígido 2D.
        /// <para>Returns a <see cref="Span{T}"/> of all 2D rigid body components.</para>
        /// </summary>
        public Span<RigidBodyComponent2D> GetRigidBodyComponentsAsSpan()
        {
            return EntityManager.Rigidbody2DComponents.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de cuerpo rígido 2D de la entidad con el id especificado.
        /// <para>Returns a reference to the 2D rigid body component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref RigidBodyComponent2D GetRigidBodyComponent(int entityId)
        {
            return ref EntityManager.Rigidbody2DComponents[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de cuerpo rígido 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D rigid body component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref RigidBodyComponent2D GetRigidBodyComponent(ref Yotsuba entity)
        {
            return ref EntityManager.Rigidbody2DComponents[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de cuerpo rígido 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D rigid body component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref RigidBodyComponent2D GetRigidBodyComponent(Yotsuba entity)
        {
            return ref EntityManager.Rigidbody2DComponents[entity.Id];
        }

        // --- Button 2D ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de botón 2D.
        /// <para>Returns a <see cref="Span{T}"/> of all 2D button components.</para>
        /// </summary>
        public Span<ButtonComponent2D> GetButtonComponentsAsSpan()
        {
            return EntityManager.Button2DComponents.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de botón 2D de la entidad con el id especificado.
        /// <para>Returns a reference to the 2D button component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref ButtonComponent2D GetButtonComponent(int entityId)
        {
            return ref EntityManager.Button2DComponents[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de botón 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D button component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref ButtonComponent2D GetButtonComponent(ref Yotsuba entity)
        {
            return ref EntityManager.Button2DComponents[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de botón 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D button component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref ButtonComponent2D GetButtonComponent(Yotsuba entity)
        {
            return ref EntityManager.Button2DComponents[entity.Id];
        }

        // --- Model 3D ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de modelo 3D.
        /// <para>Returns a <see cref="Span{T}"/> of all 3D model components.</para>
        /// </summary>
        public Span<ModelComponent3D> GetModelsComponentsAsSpan()
        {
            return EntityManager.ModelComponents3D.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de modelo 3D de la entidad con el id especificado.
        /// <para>Returns a reference to the 3D model component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref ModelComponent3D GetModelComponent(int entityId)
        {
            return ref EntityManager.ModelComponents3D[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de modelo 3D de la entidad especificada.
        /// <para>Returns a reference to the 3D model component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref ModelComponent3D GetModelComponent(ref Yotsuba entity)
        {
            return ref EntityManager.ModelComponents3D[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de modelo 3D de la entidad especificada.
        /// <para>Returns a reference to the 3D model component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref ModelComponent3D GetModelComponent(Yotsuba entity)
        {
            return ref EntityManager.ModelComponents3D[entity.Id];
        }

        // --- YTB Model 3D ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de modelo 3D en formato YTB.
        /// <para>Returns a <see cref="Span{T}"/> of all YTB-format 3D model components.</para>
        /// </summary>
        public Span<YTBModelComponent3D> GetYTBModelComponentsAsSpan()
        {
            return EntityManager.YtbModelComponents.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de modelo 3D YTB de la entidad con el id especificado.
        /// <para>Returns a reference to the YTB 3D model component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref YTBModelComponent3D GetYTBModelComponent(int entityId)
        {
            return ref EntityManager.YtbModelComponents[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de modelo 3D YTB de la entidad especificada.
        /// <para>Returns a reference to the YTB 3D model component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref YTBModelComponent3D GetYTBModelComponent(ref Yotsuba entity)
        {
            return ref EntityManager.YtbModelComponents[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de modelo 3D YTB de la entidad especificada.
        /// <para>Returns a reference to the YTB 3D model component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref YTBModelComponent3D GetYTBModelComponent(Yotsuba entity)
        {
            return ref EntityManager.YtbModelComponents[entity.Id];
        }

        // --- Input ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de entrada (input).
        /// <para>Returns a <see cref="Span{T}"/> of all input components.</para>
        /// </summary>
        public Span<InputComponent> GetInputComponentsAsSpan()
        {
            return EntityManager.InputComponents.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de entrada de la entidad con el id especificado.
        /// <para>Returns a reference to the input component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref InputComponent GetInputComponent(int entityId)
        {
            return ref EntityManager.InputComponents[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de entrada de la entidad especificada.
        /// <para>Returns a reference to the input component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref InputComponent GetInputComponent(ref Yotsuba entity)
        {
            return ref EntityManager.InputComponents[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de entrada de la entidad especificada.
        /// <para>Returns a reference to the input component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref InputComponent GetInputComponent(Yotsuba entity)
        {
            return ref EntityManager.InputComponents[entity.Id];
        }

        // --- Font 2D ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de fuente 2D.
        /// <para>Returns a <see cref="Span{T}"/> of all 2D font components.</para>
        /// </summary>
        public Span<FontComponent2D> GetFontComponentsAsSpan()
        {
            return EntityManager.Font2DComponents.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de fuente 2D de la entidad con el id especificado.
        /// <para>Returns a reference to the 2D font component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref FontComponent2D GetFontComponent(int entityId)
        {
            return ref EntityManager.Font2DComponents[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de fuente 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D font component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref FontComponent2D GetFontComponent(ref Yotsuba entity)
        {
            return ref EntityManager.Font2DComponents[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de fuente 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D font component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref FontComponent2D GetFontComponent(Yotsuba entity)
        {
            return ref EntityManager.Font2DComponents[entity.Id];
        }

        // --- Camera ---

        /// <summary>
        /// Retorna el componente de cámara activo de la escena.
        /// <para>Returns the active camera component of the scene.</para>
        /// </summary>
        public CameraComponent3D GetCamera() => EntityManager.Camera;

        // --- Animation 2D ---

        /// <summary>
        /// Retorna un <see cref="Span{T}"/> de todos los componentes de animación 2D.
        /// <para>Returns a <see cref="Span{T}"/> of all 2D animation components.</para>
        /// </summary>
        public Span<AnimationComponent2D> GetAnimationComponentsAsSpan()
        {
            return EntityManager.Animation2DComponents.AsSpan();
        }

        /// <summary>
        /// Retorna una referencia al componente de animación 2D de la entidad con el id especificado.
        /// <para>Returns a reference to the 2D animation component of the entity with the specified id.</para>
        /// </summary>
        /// <param name="entityId">Identificador único de la entidad. <para>Unique entity identifier.</para></param>
        public ref AnimationComponent2D GetAnimationComponent(int entityId)
        {
            return ref EntityManager.Animation2DComponents[entityId];
        }

        /// <summary>
        /// Retorna una referencia al componente de animación 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D animation component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad. <para>Entity instance.</para></param>
        public ref AnimationComponent2D GetAnimationComponent(ref Yotsuba entity)
        {
            return ref EntityManager.Animation2DComponents[entity.Id];
        }

        /// <summary>
        /// Retorna una referencia al componente de animación 2D de la entidad especificada.
        /// <para>Returns a reference to the 2D animation component of the specified entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de la entidad (por valor). <para>Entity instance (by value).</para></param>
        public ref AnimationComponent2D GetAnimationComponent(Yotsuba entity)
        {
            return ref EntityManager.Animation2DComponents[entity.Id];
        }

        /// <summary>
        /// Verifica si la entidad especificada es el valor <c>default</c>, es decir, si no fue encontrada o no fue inicializada.
        /// Útil para validar el resultado de <see cref="GetEntityByNameOrDefault"/>.
        /// <para>Checks whether the specified entity is the <c>default</c> value, meaning it was not found or not initialized.
        /// Useful for validating the result of <see cref="GetEntityByNameOrDefault"/>.</para>
        /// </summary>
        /// <param name="entity">Entidad a verificar. <para>Entity to check.</para></param>
        /// <returns>
        /// <c>true</c> si la entidad es el valor por defecto; <c>false</c> en caso contrario.
        /// <para><c>true</c> if the entity is the default value; <c>false</c> otherwise.</para>
        /// </returns>
        public static bool IsDefaultEntity(ref Yotsuba entity)
        {
            return entity.Equals(default(Yotsuba));
        }
    }
}
