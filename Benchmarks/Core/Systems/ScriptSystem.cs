using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Systems.Contract;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    /// <summary>
    /// Sistema encargado de manejar los scripts de las entidades.
    /// Stub que replica el patron de iteracion del engine.
    /// </summary>
    public class ScriptSystem : ISystem
    {
        public override void InitializeSystem(EntityManager @entities)
        {
            EntityManager = @entities;
        }

        public override void SharedEntityInitialize(ref Yotsuba Entidad)
        {
            if (!Entidad.HasComponent(YTBComponent.Script)) return;
            ref ScriptComponent script = ref GetScriptComponent(Entidad.Id);
            _ = script.IsActive;
        }

        public override void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time)
        {
            if (!Entidad.HasComponent(YTBComponent.Script)) return;
            ref ScriptComponent script = ref GetScriptComponent(Entidad.Id);
            if (!script.IsActive) return;
            // Simulate script execution: read transform, modify it
            if (Entidad.HasComponent(YTBComponent.Transform))
            {
                ref TransformComponent transform = ref GetTransform2DComponent(Entidad.Id);
                _ = transform.Position;
            }
        }

        public void DrawSystem2D(GameTime gameTime)
        {
            if (EntityManager == null) return;
            foreach (ref Yotsuba entity in GetEntitiesAsSpan())
            {
                if (!entity.HasComponent(YTBComponent.Script)) continue;
                ref ScriptComponent script = ref GetScriptComponent(entity.Id);
                if (!script.IsActive) continue;
                _ = script.ScriptCount;
            }
        }

        public void DrawSystem3D(GameTime gameTime)
        {
            // Stub for 3D script rendering
        }

        public void Clear() { }
        public override void Dispose() { }
    }
}
