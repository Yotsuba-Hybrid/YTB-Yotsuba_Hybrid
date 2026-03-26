using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Systems.Contract;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    /// <summary>
    /// Sistema encargado de gestionar todas las animaciones de las entidades.
    /// Replica el patron de iteracion con ref mutation del engine.
    /// </summary>
    public class AnimationSystem2D : ISystem
    {
        private EventManager EventManager { get; set; }

        public override void InitializeSystem(EntityManager @entities)
        {
            EventManager = EventManager.Instance;
            EntityManager = @entities;
        }

        public override void UpdateSystem(GameTime gameTime)
        {
            Span<AnimationComponent2D> animationsComponents = GetAnimationComponentsAsSpan();
            Span<SpriteComponent2D> spriteComponents = GetSpriteComponentsAsSpan();
            if (EntityManager == null) return;
            foreach (ref Yotsuba entity in GetEntitiesAsSpan())
            {
                if (!entity.HasComponent(YTBComponent.Animation) || !entity.HasComponent(YTBComponent.Sprite)) continue;
                ref AnimationComponent2D animationComponent = ref animationsComponents[entity.Id];
                ref SpriteComponent2D spriteComponent = ref spriteComponents[entity.Id];

                if (animationComponent.CurrentAnimationType.Item2.IsFinished)
                {
                    if (!animationComponent.CurrentAnimationType.Item2.FinishedWasMarked)
                    {
                        animationComponent.CurrentAnimationType.Item2.FinishedWasMarked = true;
                    }
                }
                else
                {
                    Rectangle region = animationComponent.CurrentAnimationType.Item2.CurrentFrameRect(gameTime);
                    spriteComponent.TextureId = animationComponent.CurrentAnimationType.Item2.TextureId;
                    spriteComponent.SourceRectangle = region;
                }
            }
        }

        public override void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time) { }
        public override void SharedEntityInitialize(ref Yotsuba Entidad) { }
        public override void Dispose() { }
    }
}
