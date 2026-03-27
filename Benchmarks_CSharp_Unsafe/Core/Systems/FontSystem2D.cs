using System.Runtime.CompilerServices;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    public unsafe struct FontSystem2D
    {
        private nint _emPtr;
        public int DrawCallCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitializeSystem(EntityManager* entities) => _emPtr = (nint)entities;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SharedEntityInitialize(ref Yotsuba entity)
        {
            if (!entity.HasComponent(YTBComponent.Font)) return;
            EntityManager* em = (EntityManager*)_emPtr;
            ref FontComponent2D font = ref Unsafe.Add(
                ref Unsafe.AsRef<FontComponent2D>(em->Font2DComponents.Ptr), entity.Id);
            _ = font.IsVisible;
        }

        public void DrawSystem(GameTime gameTime)
        {
            DrawCallCount = 0;
            EntityManager* em = (EntityManager*)_emPtr;
            if (em == null) return;

            FontComponent2D* fonts = em->Font2DComponents.Ptr;
            TransformComponent* transforms = em->TransformComponents.Ptr;
            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int count = em->YotsubaEntities.Count;

            for (int i = 0; i < count; i++)
            {
                ref Yotsuba e = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if (!e.HasComponent(YTBComponent.Font)) continue;
                ref FontComponent2D font = ref Unsafe.Add(ref Unsafe.AsRef<FontComponent2D>(fonts), e.Id);
                if (!font.IsVisible) continue;

                ref TransformComponent t = ref Unsafe.Add(ref Unsafe.AsRef<TransformComponent>(transforms), e.Id);
                _ = t.Position;
                DrawCallCount++;
            }
        }

        public void Dispose() { }
    }
}
