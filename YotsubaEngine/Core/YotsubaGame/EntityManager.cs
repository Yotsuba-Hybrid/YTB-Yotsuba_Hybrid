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
    /// Clase que administra las entidades de una escena`
    /// </summary>
    /// <param name="entities">Recibe el arreglo de entidades de la escena</param>
    public class EntityManager(YTB<Yotsuba> entities)
    {
        /// <summary>
        /// Listado de entidades en la escena.
        /// </summary>
        public YTB<Yotsuba> YotsubaEntities = entities;

        /// <summary>
        /// Listado de los componentes de transformación de las entidades.
        /// </summary>
        public YTB<TransformComponent> TransformComponents = new YTB<TransformComponent>();

        /// <summary>
        /// Listado de los componentes de sprite de las entidades 2D.
        /// </summary>
        public YTB<SpriteComponent2D> Sprite2DComponents = new YTB<SpriteComponent2D>();

        /// <summary>
        /// Listado de los componentes de animación de las entidades 2D.
        /// </summary>
        public YTB<AnimationComponent2D> Animation2DComponents = new YTB<AnimationComponent2D>();

        /// <summary>
        /// Listado de los componentes de Cuerpo rigido2D
        /// </summary>

        public YTB<RigidBodyComponent2D> Rigidbody2DComponents = new YTB<RigidBodyComponent2D>();

        /// <summary>
        /// Listado de los componentes de Input de las entidades
        /// </summary>
        public YTB<InputComponent> InputComponents = new YTB<InputComponent>();

        /// <summary>
        /// Listado de los componentes de modelos 3d de las entidades
        /// </summary>
        public YTB<ModelComponent3D> ModelComponents3D = new YTB<ModelComponent3D>();

        /// <summary>
        /// Listado de los componentes de mapa de tiles de las entidades
        /// </summary>
        public YTB<TileMapComponent2D> TileMapComponent2Ds = new YTB<TileMapComponent2D>();

        /// <summary>
        /// Componente de cámara en tercera persona que calcula y expone las
        /// matrices de vista y proyección en función de la posición y orientación
        /// del avatar.
        /// </summary>
        public CameraComponent3D Camera { get; set; }

        /// <summary>
        /// Listado de los botones 2d de la escena
        /// </summary>
        public YTB<ButtonComponent2D> Button2DComponents = new YTB<ButtonComponent2D>();

        /// <summary>
        /// Listado de scripts que alteran el comportamiento de las entidades
        /// </summary>
        public YTB<ScriptComponent> ScriptComponents = new YTB<ScriptComponent>();

        /// <summary>
        /// Listado de fuentes de texto en 2D
        /// </summary>
        public YTB<FontComponent2D> Font2DComponents = new YTB<FontComponent2D>();

        /// <summary>
        /// Listado de componentes de shader para entidades 2D
        /// </summary>
        public YTB<ShaderComponent> ShaderComponents = new YTB<ShaderComponent>();

        /// <summary>
        /// Objeto 3D
        /// </summary>
        public YTB<Object3D> Object3Ds = new YTB<Object3D>();

        /// <summary>
        /// Colección de colecciones de objetos 3D
        /// </summary>
        public YTB<ListObject3D> StorageObjectS3D = new YTB<ListObject3D>();

        public EntityManager() : this([])
        {

        }

        public void AddEntity(Yotsuba entity)
        {
            AddEntity(ref entity);
        }

        /// <summary>
        /// Método para agregar una nueva entidad al EntityManager.
        /// </summary>
        /// <exception cref="AddComponentInDiferentEntityIndexException">
        /// Excepción producida cuando un índice de un componente no es igual al índice al que entro una entidad
        /// </exception>
        /// <param name="entity"></param>
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
        /// Metodo para agregar un componente de Tilemap a una entidad.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void AddTileMapComponent(Yotsuba entity, TileMapComponent2D component)
        {
            entity.AddComponent(YTBComponent.TileMap);
            TileMapComponent2Ds[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Metodo para agregar un componente de transformación a una entidad.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void AddTransformComponent(Yotsuba entity, TransformComponent component)
        {
            entity.AddComponent(YTBComponent.Transform);
            TransformComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Metodo para agregar un componente de Sprite a una entidad.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void AddSpriteComponent(Yotsuba entity, SpriteComponent2D component)
        {
            entity.AddComponent(YTBComponent.Sprite);
            Sprite2DComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Metodo para agregar un componente de Animacion a una entidad.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void AddAnimationComponent(Yotsuba entity, AnimationComponent2D component)
        {
            entity.AddComponent(YTBComponent.Animation);
            Animation2DComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Agrega una animación a una entidad que ya tenga un componente de animación.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="animationType"></param>
        /// <param name="animation"></param>
        public void AddAnimationInAnimationComponent(Yotsuba entity, AnimationType animationType, Animation animation)
        {
            if (!entity.HasComponent(YTBComponent.Animation))
                return;
            Animation2DComponents[entity.Id].AddAnimation(animationType, animation);
        }


        /// <summary>
        /// Método para agregar un cuerpo rígido a una entidad
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void AddRigidbodyComponent(Yotsuba entity, RigidBodyComponent2D component)
        {
            entity.AddComponent(YTBComponent.Rigibody);
            Rigidbody2DComponents[(uint)entity.Id] = component;
        }


        /// <summary>
        /// Método para agregar un componente de Input a una entidad.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void AddInputComponent(Yotsuba entity, InputComponent component)
        {
            entity.AddComponent(YTBComponent.Input);
            InputComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Método para agregar un componente de modelo 3D a una entidad.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void AddModelComponent3D(Yotsuba entity, ModelComponent3D component)
        {
            entity.AddComponent(YTBComponent.Model3D);
            ModelComponents3D[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Método para agregar un componente de cámara a la escena.
        /// Solo puede haber una cámara activa.
        /// </summary>
        /// <param name="component"></param>
        public void AddCameraComponent(CameraComponent3D component, Yotsuba entity)
        {
            entity.AddComponent(YTBComponent.Camera);
            Camera = component;
        }

        /// <summary>
        /// Método para agregar un componente de botón 2D a una entidad.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void AddButtonComponent2D(Yotsuba entity, ButtonComponent2D component)
        {
            entity.AddComponent(YTBComponent.Button2D);
            Button2DComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Metodo para agregar un componente de script a una entidad
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="script"></param>
        public void AddScriptComponent(Yotsuba entity, ScriptComponent script)
        {
            entity.AddComponent(YTBComponent.Script);
            ScriptComponents[(uint)entity.Id] = script;
        }

        /// <summary>
        /// Metodo para agregar un componente de fuente de texto 2D a una entidad
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void AddFontComponent2D(Yotsuba entity, FontComponent2D component)
        {
            entity.AddComponent(YTBComponent.Font);
            Font2DComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Método para agregar un componente de shader a una entidad 2D.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void AddShaderComponent2D(Yotsuba entity, ShaderComponent component)
        {
            entity.AddComponent(YTBComponent.Shader);
            ShaderComponents[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Adds a new 3D storage object to the collection or scene.
        /// </summary>
        public void AddStorageObject3D(Yotsuba entity, ListObject3D component)
        {
            entity.AddComponent(YTBComponent.StorageObjects3D);
            StorageObjectS3D[(uint)entity.Id] = component;
        }

        /// <summary>
        /// Adds a new 3D storage object to the collection or scene.
        /// </summary>
        public void AddObject3D(Yotsuba entity, Object3D component)
        {
            entity.AddComponent(YTBComponent.Object3D);
            Object3Ds[(uint)entity.Id] = component;
        }
    }
}

