using System.Runtime.CompilerServices;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    public unsafe struct AnimationSystem2D
    {
        private nint _emPtr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitializeSystem(EntityManager* entities) => _emPtr = (nint)entities;

        public void UpdateSystem(GameTime gameTime)
        {
            EntityManager* em = (EntityManager*)_emPtr;
            if (em == null) return;

            AnimationComponent2D* anims = em->Animation2DComponents.Ptr;
            SpriteComponent2D* sprites = em->Sprite2DComponents.Ptr;
            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int count = em->YotsubaEntities.Count;

            for (int i = 0; i < count; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if (!entity.HasComponent(YTBComponent.Animation) || !entity.HasComponent(YTBComponent.Sprite)) continue;

                ref AnimationComponent2D anim = ref Unsafe.Add(ref Unsafe.AsRef<AnimationComponent2D>(anims), entity.Id);
                ref SpriteComponent2D sprite = ref Unsafe.Add(ref Unsafe.AsRef<SpriteComponent2D>(sprites), entity.Id);

                if (anim.CurrentData.IsFinished)
                {
                    if (!anim.CurrentData.FinishedWasMarked)
                        anim.CurrentData.FinishedWasMarked = true;
                }
                else
                {
                    Rectangle region = anim.CurrentData.CurrentFrameRect(gameTime);
                    sprite.TextureId = anim.CurrentData.TextureId;
                    sprite.SourceRectangle = region;
                }
            }
        }

        public void SharedEntityForEachUpdate(ref Yotsuba e, GameTime t) { }
        public void SharedEntityInitialize(ref Yotsuba e) { }
        public void Dispose() { }
    }
}
