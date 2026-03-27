using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems.Contract
{
    /// <summary>
    /// Define una clase abstracta comun para los sistemas del motor.
    /// Replica exacta del pattern de ISystem del engine.
    /// </summary>
    public abstract class ISystem : IDisposable
    {
        public EntityManager EntityManager { get; set; }

        public virtual void InitializeSystem(EntityManager @entities) { }
        public virtual void SharedEntityInitialize(ref Yotsuba @Entidad) { }
        public virtual void UpdateSystem(GameTime @gameTime) { }
        public virtual void SharedEntityForEachUpdate(ref Yotsuba @Entidad, GameTime time) { }

        public virtual void Dispose() { }

        // --- Entities ---
        public Span<Yotsuba> GetEntitiesAsSpan() => EntityManager.YotsubaEntities.AsSpan();

        public ref Yotsuba GetEntity(int entityId) => ref EntityManager.YotsubaEntities[entityId];

        public ref Yotsuba GetEntityByNameOrDefault(string name) =>
            ref EntityManager.YotsubaEntities.Find(x => x.Name == name);

        // --- Transform ---
        public Span<TransformComponent> GetTransformComponentsAsSpan() => EntityManager.TransformComponents.AsSpan();
        public ref TransformComponent GetTransform2DComponent(int entityId) => ref EntityManager.TransformComponents[entityId];
        public ref TransformComponent GetTransform2DComponent(ref Yotsuba entity) => ref EntityManager.TransformComponents[entity.Id];
        public ref TransformComponent GetTransform2DComponent(Yotsuba entity) => ref EntityManager.TransformComponents[entity.Id];

        // --- Sprite 2D ---
        public Span<SpriteComponent2D> GetSpriteComponentsAsSpan() => EntityManager.Sprite2DComponents.AsSpan();
        public ref SpriteComponent2D GetSprite2DComponent(int entityId) => ref EntityManager.Sprite2DComponents[entityId];
        public ref SpriteComponent2D GetSprite2DComponent(ref Yotsuba entity) => ref EntityManager.Sprite2DComponents[entity.Id];
        public ref SpriteComponent2D GetSprite2DComponent(Yotsuba entity) => ref EntityManager.Sprite2DComponents[entity.Id];

        // --- TileMap 2D ---
        public Span<TileMapComponent2D> GetTilemapComponentsAsSpan() => EntityManager.TileMapComponent2Ds.AsSpan();
        public ref TileMapComponent2D GetTilemapComponent(int entityId) => ref EntityManager.TileMapComponent2Ds[entityId];
        public ref TileMapComponent2D GetTilemapComponent(ref Yotsuba entity) => ref EntityManager.TileMapComponent2Ds[entity.Id];

        // --- Shader ---
        public Span<ShaderComponent> GetShaderComponentsAsSpan() => EntityManager.ShaderComponents.AsSpan();
        public ref ShaderComponent GetShaderComponent(int entityId) => ref EntityManager.ShaderComponents[entityId];
        public ref ShaderComponent GetShaderComponent(ref Yotsuba entity) => ref EntityManager.ShaderComponents[entity.Id];

        // --- Script ---
        public Span<ScriptComponent> GetScriptComponentsAsSpan() => EntityManager.ScriptComponents.AsSpan();
        public ref ScriptComponent GetScriptComponent(int entityId) => ref EntityManager.ScriptComponents[entityId];

        // --- RigidBody 2D ---
        public Span<RigidBodyComponent2D> GetRigidBodyComponentsAsSpan() => EntityManager.Rigidbody2DComponents.AsSpan();
        public ref RigidBodyComponent2D GetRigidBodyComponent(int entityId) => ref EntityManager.Rigidbody2DComponents[entityId];
        public ref RigidBodyComponent2D GetRigidBodyComponent(ref Yotsuba entity) => ref EntityManager.Rigidbody2DComponents[entity.Id];

        // --- Button 2D ---
        public Span<ButtonComponent2D> GetButtonComponentsAsSpan() => EntityManager.Button2DComponents.AsSpan();
        public ref ButtonComponent2D GetButtonComponent(int entityId) => ref EntityManager.Button2DComponents[entityId];

        // --- Model 3D ---
        public Span<ModelComponent3D> GetModelsComponentsAsSpan() => EntityManager.ModelComponents3D.AsSpan();
        public ref ModelComponent3D GetModelComponent(int entityId) => ref EntityManager.ModelComponents3D[entityId];

        // --- YTB Model 3D ---
        public Span<YTBModelComponent3D> GetYTBModelComponentsAsSpan() => EntityManager.YtbModelComponents.AsSpan();
        public ref YTBModelComponent3D GetYTBModelComponent(int entityId) => ref EntityManager.YtbModelComponents[entityId];

        // --- Input ---
        public Span<InputComponent> GetInputComponentsAsSpan() => EntityManager.InputComponents.AsSpan();
        public ref InputComponent GetInputComponent(int entityId) => ref EntityManager.InputComponents[entityId];
        public ref InputComponent GetInputComponent(ref Yotsuba entity) => ref EntityManager.InputComponents[entity.Id];

        // --- Font 2D ---
        public Span<FontComponent2D> GetFontComponentsAsSpan() => EntityManager.Font2DComponents.AsSpan();
        public ref FontComponent2D GetFontComponent(int entityId) => ref EntityManager.Font2DComponents[entityId];

        // --- Animation 2D ---
        public Span<AnimationComponent2D> GetAnimationComponentsAsSpan() => EntityManager.Animation2DComponents.AsSpan();
        public ref AnimationComponent2D GetAnimationComponent(int entityId) => ref EntityManager.Animation2DComponents[entityId];
        public ref AnimationComponent2D GetAnimationComponent(ref Yotsuba entity) => ref EntityManager.Animation2DComponents[entity.Id];

        public static bool IsDefaultEntity(ref Yotsuba entity) => entity.Equals(default(Yotsuba));
    }
}
