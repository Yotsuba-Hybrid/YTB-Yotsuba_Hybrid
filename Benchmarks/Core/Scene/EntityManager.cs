using Benchmarks.Core.Collections;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;

namespace Benchmarks.Core.Scene
{
    /// <summary>
    /// Clase que administra las entidades de una escena.
    /// Replica exacta del patron de parallel arrays del engine.
    /// </summary>
    public class EntityManager(YTB<Yotsuba> entities)
    {
        public YTB<Yotsuba> YotsubaEntities = entities;

        public YTB<TransformComponent> TransformComponents = new YTB<TransformComponent>();
        public YTB<SpriteComponent2D> Sprite2DComponents = new YTB<SpriteComponent2D>();
        public YTB<AnimationComponent2D> Animation2DComponents = new YTB<AnimationComponent2D>();
        public YTB<RigidBodyComponent2D> Rigidbody2DComponents = new YTB<RigidBodyComponent2D>();
        public YTB<InputComponent> InputComponents = new YTB<InputComponent>();
        public YTB<ModelComponent3D> ModelComponents3D = new YTB<ModelComponent3D>();
        public YTB<TileMapComponent2D> TileMapComponent2Ds = new YTB<TileMapComponent2D>();
        public YTB<ButtonComponent2D> Button2DComponents = new YTB<ButtonComponent2D>();
        public YTB<ScriptComponent> ScriptComponents = new YTB<ScriptComponent>();
        public YTB<FontComponent2D> Font2DComponents = new YTB<FontComponent2D>();
        public YTB<ShaderComponent> ShaderComponents = new YTB<ShaderComponent>();
        public YTB<YTBModelComponent3D> YtbModelComponents = new YTB<YTBModelComponent3D>();

        public int CameraEntityId { get; set; } = -1;
        public float CameraZoom { get; set; } = 1.0f;

        public EntityManager() : this(new YTB<Yotsuba>())
        {
        }

        public void AddEntity(ref Yotsuba entity)
        {
            int Index = YotsubaEntities.Add(entity);
            YotsubaEntities[Index].Id = Index;
            entity.Id = Index;
            int ComponentIndex = 0;

            if (TransformComponents.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(TransformComponent)}. Index {Index} != {ComponentIndex}");

            if (Sprite2DComponents.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(SpriteComponent2D)}. Index {Index} != {ComponentIndex}");

            if (Animation2DComponents.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(AnimationComponent2D)}. Index {Index} != {ComponentIndex}");

            if (Rigidbody2DComponents.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(RigidBodyComponent2D)}. Index {Index} != {ComponentIndex}");

            if (InputComponents.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(InputComponent)}. Index {Index} != {ComponentIndex}");

            if (ModelComponents3D.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(ModelComponent3D)}. Index {Index} != {ComponentIndex}");

            if (Button2DComponents.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(ButtonComponent2D)}. Index {Index} != {ComponentIndex}");

            if (ScriptComponents.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(ScriptComponent)}. Index {Index} != {ComponentIndex}");

            if (TileMapComponent2Ds.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(TileMapComponent2D)}. Index {Index} != {ComponentIndex}");

            if (Font2DComponents.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(FontComponent2D)}. Index {Index} != {ComponentIndex}");

            if (ShaderComponents.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(ShaderComponent)}. Index {Index} != {ComponentIndex}");

            if (YtbModelComponents.Add(default, out ComponentIndex) != Index)
                throw new InvalidOperationException(
                    $"Component entered at different index than entity. Component: {typeof(YTBModelComponent3D)}. Index {Index} != {ComponentIndex}");
        }

        public void AddTransformComponent(Yotsuba entity, TransformComponent component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Transform);
            TransformComponents[(uint)entity.Id] = component;
        }

        public void AddSpriteComponent(Yotsuba entity, SpriteComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Sprite);
            Sprite2DComponents[(uint)entity.Id] = component;
        }

        public void AddAnimationComponent(Yotsuba entity, AnimationComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Animation);
            Animation2DComponents[(uint)entity.Id] = component;
        }

        public void AddRigidbodyComponent(Yotsuba entity, RigidBodyComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Rigibody);
            Rigidbody2DComponents[(uint)entity.Id] = component;
        }

        public void AddInputComponent(Yotsuba entity, InputComponent component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Input);
            InputComponents[(uint)entity.Id] = component;
        }

        public void AddButtonComponent2D(Yotsuba entity, ButtonComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Button2D);
            Button2DComponents[(uint)entity.Id] = component;
        }

        public void AddShaderComponent2D(Yotsuba entity, ShaderComponent component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Shader);
            ShaderComponents[(uint)entity.Id] = component;
        }

        public void AddScriptComponent(Yotsuba entity, ScriptComponent script)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Script);
            ScriptComponents[(uint)entity.Id] = script;
        }

        public void AddFontComponent2D(Yotsuba entity, FontComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Font);
            Font2DComponents[(uint)entity.Id] = component;
        }

        public void AddTileMapComponent(Yotsuba entity, TileMapComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.TileMap);
            TileMapComponent2Ds[(uint)entity.Id] = component;
        }

        public void AddModelComponent3D(Yotsuba entity, ModelComponent3D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Model3D);
            ModelComponents3D[(uint)entity.Id] = component;
        }

        public void AddYTBObject3D(Yotsuba entity, YTBModelComponent3D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.YTBModel3D);
            YtbModelComponents[(uint)entity.Id] = component;
        }
    }
}
