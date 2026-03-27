using System.Runtime.CompilerServices;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    public unsafe struct ScriptSystem
    {
        private nint _emPtr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitializeSystem(EntityManager* entities) => _emPtr = (nint)entities;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SharedEntityInitialize(ref Yotsuba entity)
        {
            if (!entity.HasComponent(YTBComponent.Script)) return;
            EntityManager* em = (EntityManager*)_emPtr;
            ref ScriptComponent script = ref Unsafe.Add(
                ref Unsafe.AsRef<ScriptComponent>(em->ScriptComponents.Ptr), entity.Id);
            _ = script.IsActive;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SharedEntityForEachUpdate(ref Yotsuba entity, GameTime time)
        {
            if (!entity.HasComponent(YTBComponent.Script)) return;
            EntityManager* em = (EntityManager*)_emPtr;
            ref ScriptComponent script = ref Unsafe.Add(
                ref Unsafe.AsRef<ScriptComponent>(em->ScriptComponents.Ptr), entity.Id);
            if (!script.IsActive) return;

            if (entity.HasComponent(YTBComponent.Transform))
            {
                ref TransformComponent t = ref Unsafe.Add(
                    ref Unsafe.AsRef<TransformComponent>(em->TransformComponents.Ptr), entity.Id);
                _ = t.Position;
            }
        }

        public void DrawSystem2D(GameTime gameTime)
        {
            EntityManager* em = (EntityManager*)_emPtr;
            if (em == null) return;
            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int count = em->YotsubaEntities.Count;
            ScriptComponent* scripts = em->ScriptComponents.Ptr;

            for (int i = 0; i < count; i++)
            {
                ref Yotsuba e = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if (!e.HasComponent(YTBComponent.Script)) continue;
                ref ScriptComponent s = ref Unsafe.Add(ref Unsafe.AsRef<ScriptComponent>(scripts), e.Id);
                if (!s.IsActive) continue;
                _ = s.ScriptCount;
            }
        }

        public void DrawSystem3D(GameTime gameTime) { }
        public void Clear() { }
        public void Dispose() { }
    }
}
