using System.ComponentModel;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Exceptions;
using YotsubaEngine.Graphics;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.YotsubaGame
{
    /// <summary>
    /// Clase que administra las entidades de una escena.
    /// <para>Class that manages a scene's entities.</para>
    /// </summary>
    /// <param name="entities">Arreglo de entidades de la escena. <para>Scene entity array.</para></param>
    public class EntityManager(YTB<Yotsuba> entities)
    {
        /// <summary>
        /// Listado de entidades en la escena.
        /// <para>List of entities in the scene.</para>
        /// </summary>
        public YTB<Yotsuba> YotsubaEntities = entities;

        /// <summary>
        /// Listado de los componentes de transformación de las entidades.
        /// <para>List of transform components for entities.</para>
        /// </summary>
        public YTB<TransformComponent> TransformComponents = new YTB<TransformComponent>();

        /// <summary>
        /// Listado de los componentes de sprite de las entidades 2D.
        /// <para>List of sprite components for 2D entities.</para>
        /// </summary>
        public YTB<SpriteComponent2D> Sprite2DComponents = new YTB<SpriteComponent2D>();

        /// <summary>
        /// Listado de los componentes de animación de las entidades 2D.
        /// <para>List of animation components for 2D entities.</para>
        /// </summary>
        public YTB<AnimationComponent2D> Animation2DComponents = new YTB<AnimationComponent2D>();

        /// <summary>
        /// Listado de los componentes de cuerpo rígido 2D.
        /// <para>List of 2D rigid body components.</para>
        /// </summary>

        public YTB<RigidBodyComponent2D> Rigidbody2DComponents = new YTB<RigidBodyComponent2D>();

        /// <summary>
        /// Listado de los componentes de entrada de las entidades.
        /// <para>List of input components for entities.</para>
        /// </summary>
        public YTB<InputComponent> InputComponents = new YTB<InputComponent>();

        /// <summary>
        /// Listado de los componentes de modelos 3D de las entidades.
        /// <para>List of 3D model components for entities.</para>
        /// </summary>
        public YTB<ModelComponent3D> ModelComponents3D = new YTB<ModelComponent3D>();

        /// <summary>
        /// Listado de los componentes de mapa de tiles de las entidades.
        /// <para>List of tile map components for entities.</para>
        /// </summary>
        public YTB<TileMapComponent2D> TileMapComponent2Ds = new YTB<TileMapComponent2D>();

        /// <summary>
        /// Componente de cámara en tercera persona que calcula y expone las matrices de vista y proyección.
        /// <para>Third-person camera component that exposes view and projection matrices.</para>
        /// </summary>
        public CameraComponent3D Camera { get; set; }

        /// <summary>
        /// Listado de los botones 2D de la escena.
        /// <para>List of 2D button components in the scene.</para>
        /// </summary>
        public YTB<ButtonComponent2D> Button2DComponents = new YTB<ButtonComponent2D>();

        /// <summary>
        /// Listado de scripts que alteran el comportamiento de las entidades.
        /// <para>List of script components that alter entity behavior.</para>
        /// </summary>
        public YTB<ScriptComponent> ScriptComponents = new YTB<ScriptComponent>();

        /// <summary>
        /// Listado de fuentes de texto en 2D.
        /// <para>List of 2D font components.</para>
        /// </summary>
        public YTB<FontComponent2D> Font2DComponents = new YTB<FontComponent2D>();

        /// <summary>
        /// Listado de componentes de shader para entidades 2D.
        /// <para>List of shader components for 2D entities.</para>
        /// </summary>
        public YTB<ShaderComponent> ShaderComponents = new YTB<ShaderComponent>();

        /// <summary>
        /// Objetos 3D de la escena.
        /// <para>3D objects in the scene.</para>
        /// </summary>
        public YTB<Object3D> Object3Ds = new YTB<Object3D>();

        /// <summary>
        /// Colección de colecciones de objetos 3D.
        /// <para>Collection of 3D object collections.</para>
        /// </summary>
        public YTB<ListObject3D> StorageObjectS3D = new YTB<ListObject3D>();

        /// <summary>
        /// Crea un administrador de entidades vacío.
        /// <para>Creates an empty entity manager.</para>
        /// </summary>
        public EntityManager() : this(new YTB<Yotsuba>())
        {

        }

        /// <summary>
        /// Método para agregar una nueva entidad al administrador.
        /// <para>Adds a new entity to the manager.</para>
        /// </summary>
        /// <exception cref="AddComponentInDiferentEntityIndexException">
        /// Excepción producida cuando un índice de un componente no es igual al índice al que entro una entidad
        /// </exception>
        /// <param name="entity">Entidad a agregar. <para>Entity to add.</para></param>
        public void AddEntity(ref Yotsuba entity)
        {
            int Index = YotsubaEntities.Add(entity);
            YotsubaEntities[Index].Id = Index;
            entity.Id = Index;
            int ComponentIndex = 0;

            if (TransformComponents.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un indice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(TransformComponent)}. " +
                    $"Index {Index} != {ComponentIndex}"
                    );

            if (Sprite2DComponents.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(SpriteComponent2D)}. " +
                    $"Index {Index} != {ComponentIndex}"
                    );

            if (Animation2DComponents.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(AnimationComponent2D)}. " +
                    $"Index {Index} != {ComponentIndex}"
                    );

            if (Rigidbody2DComponents.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(RigidBodyComponent2D)}. " +
                    $"Index {Index} != {ComponentIndex}"
                    );

            if (InputComponents.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(InputComponent)}. " +
                    $"Index {Index} != {ComponentIndex}"
                    );

            if (ModelComponents3D.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(ModelComponent3D)}. " +
                    $"Index {Index} != {ComponentIndex}"
                    );

            if (Button2DComponents.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(ButtonComponent2D)}. " +
                    $"Index {Index} != {ComponentIndex}");

            if (ScriptComponents.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(ScriptComponent)}. " +
                    $"Index {Index} != {ComponentIndex}");

            if (TileMapComponent2Ds.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(TileMapComponent2D)}. " +
                    $"Index {Index} != {ComponentIndex}");

            if (Font2DComponents.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(FontComponent2D)}. " +
                    $"Index {Index} != {ComponentIndex}");

            if (ShaderComponents.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(ShaderComponent)}. " +
                    $"Index {Index} != {ComponentIndex}");

            if (StorageObjectS3D.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(ListObject3D)}. " +
                    $"Index {Index} != {ComponentIndex}");

            if (Object3Ds.Add(default, out ComponentIndex) != Index)
                throw new AddComponentInDiferentEntityIndexException(
                    $"Componente ingreso a un índice diferente al de su entidad asociada. " +
                    $"Componente: {typeof(Object3D)}. " +
                    $"Index {Index} != {ComponentIndex}");
        }
        /// <summary>
        /// Agrega un componente de tilemap a una entidad.
        /// <para>Adds a tile map component to an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de tilemap. <para>Tile map component.</para></param>
        public void AddTileMapComponent(Yotsuba entity, TileMapComponent2D component)
        {

            YotsubaEntities[entity.Id].AddComponent(YTBComponent.TileMap);
            TileMapComponent2Ds[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Agrega un componente de transformación a una entidad.
        /// <para>Adds a transform component to an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de transformación. <para>Transform component.</para></param>
        public void AddTransformComponent(Yotsuba entity, TransformComponent component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Transform);
            TransformComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Agrega un componente de sprite a una entidad.
        /// <para>Adds a sprite component to an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de sprite. <para>Sprite component.</para></param>
        public void AddSpriteComponent(Yotsuba entity, SpriteComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Sprite);
            Sprite2DComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Agrega un componente de animación a una entidad.
        /// <para>Adds an animation component to an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de animación. <para>Animation component.</para></param>
        public void AddAnimationComponent(Yotsuba entity, AnimationComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Animation);
            Animation2DComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Agrega una animación a una entidad que ya tenga un componente de animación.
        /// <para>Adds an animation to an entity that already has an animation component.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="animationType">Tipo de animación. <para>Animation type.</para></param>
        /// <param name="animation">Animación a agregar. <para>Animation to add.</para></param>
        public void AddAnimationInAnimationComponent(Yotsuba entity, AnimationType animationType, Animation animation)
        {
            if (!YotsubaEntities[entity.Id].HasComponent(YTBComponent.Animation))
                return;
            Animation2DComponents[entity.Id].AddAnimation(animationType, animation);
        }


        /// <summary>
        /// Agrega un cuerpo rígido a una entidad.
        /// <para>Adds a rigid body to an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de cuerpo rígido. <para>Rigid body component.</para></param>
        public void AddRigidbodyComponent(Yotsuba entity, RigidBodyComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Rigibody);
            Rigidbody2DComponents[(uint)entity.Id] = component;
        }


        /// <summary>
        /// Agrega un componente de entrada a una entidad.
        /// <para>Adds an input component to an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de entrada. <para>Input component.</para></param>
        public void AddInputComponent(Yotsuba entity, InputComponent component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Input);
            InputComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Agrega un componente de modelo 3D a una entidad.
        /// <para>Adds a 3D model component to an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de modelo 3D. <para>3D model component.</para></param>
        public void AddModelComponent3D(Yotsuba entity, ModelComponent3D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Model3D);
            ModelComponents3D[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Agrega un componente de cámara a la escena; solo puede haber una cámara activa.
        /// <para>Adds a camera component to the scene; only one camera can be active.</para>
        /// </summary>
        /// <param name="component">Componente de cámara. <para>Camera component.</para></param>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        public void AddCameraComponent(CameraComponent3D component, Yotsuba entity)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Camera);
            Camera = component;
        }

        /// <summary>
        /// Agrega un componente de botón 2D a una entidad.
        /// <para>Adds a 2D button component to an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de botón 2D. <para>2D button component.</para></param>
        public void AddButtonComponent2D(Yotsuba entity, ButtonComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Button2D);
            Button2DComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Agrega un componente de script a una entidad.
        /// <para>Adds a script component to an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="script">Componente de script. <para>Script component.</para></param>
        public void AddScriptComponent(Yotsuba entity, ScriptComponent script)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Script);
            ScriptComponents[(uint)entity.Id] = script;
        }

        /// <summary>
        /// Agrega un componente de fuente de texto 2D a una entidad.
        /// <para>Adds a 2D font component to an entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de fuente 2D. <para>2D font component.</para></param>
        public void AddFontComponent2D(Yotsuba entity, FontComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Font);
            Font2DComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Agrega un componente de shader a una entidad 2D.
        /// <para>Adds a shader component to a 2D entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de shader. <para>Shader component.</para></param>
        public void AddShaderComponent2D(Yotsuba entity, ShaderComponent component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Shader);
            ShaderComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Agrega un objeto de almacenamiento 3D a la colección o escena.
        /// <para>Adds a new 3D storage object to the collection or scene.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de almacenamiento 3D. <para>3D storage component.</para></param>
        public void AddStorageObject3D(Yotsuba entity, ListObject3D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.StorageObjects3D);
            StorageObjectS3D[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Agrega un objeto 3D a la colección o escena.
        /// <para>Adds a new 3D object to the collection or scene.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="component">Componente de objeto 3D. <para>3D object component.</para></param>
        public void AddObject3D(Yotsuba entity, Object3D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Object3D);
            Object3Ds[(uint)entity.Id] = component;
        }
    }
}

