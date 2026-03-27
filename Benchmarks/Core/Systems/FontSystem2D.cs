using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Systems.Contract;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    /// <summary>
    /// Sistema encargado de dibujar texto en pantalla.
    /// Stub que replica el patron de iteracion del engine.
    /// </summary>
    public class FontSystem2D : ISystem
    {
        public int DrawCallCount { get; private set; }

        public override void InitializeSystem(EntityManager @entities)
        {
            EntityManager = @entities;
        }

        public override void SharedEntityInitialize(ref Yotsuba Entidad)
        {
            if (!Entidad.HasComponent(YTBComponent.Font)) return;
            ref FontComponent2D font = ref GetFontComponent(Entidad.Id);
            _ = font.IsVisible;
        }

        public void DrawSystem(GameTime gameTime)
        {
            DrawCallCount = 0;
            if (EntityManager == null) return;

            Span<FontComponent2D> fontComponents = GetFontComponentsAsSpan();
            Span<TransformComponent> transforms = GetTransformComponentsAsSpan();

            foreach (ref Yotsuba entity in GetEntitiesAsSpan())
            {
                if (!entity.HasComponent(YTBComponent.Font)) continue;

                ref readonly FontComponent2D font = ref fontComponents[entity.Id];
                if (!font.IsVisible) continue;

                ref readonly TransformComponent transform = ref transforms[entity.Id];

                // Simulate text draw
                _ = transform.Position;
                _ = font.Text;
                DrawCallCount++;
            }
        }

        public override void Dispose() { }
    }
}
