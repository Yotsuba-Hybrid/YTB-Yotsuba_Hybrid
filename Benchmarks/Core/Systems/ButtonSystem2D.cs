using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Systems.Contract;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    /// <summary>
    /// Sistema encargado de gestionar los botones.
    /// Replica el patron de Rectangle.Contains check del engine.
    /// </summary>
    public class ButtonSystem2D : ISystem
    {
        // Simulated mouse position
        private Vector2 _mousePosition = new(500, 400);
        private bool _mousePressed = false;

        public override void InitializeSystem(EntityManager @entities)
        {
            EntityManager = @entities;
        }

        public override void UpdateSystem(GameTime gameTime)
        {
            if (EntityManager == null) return;

            Span<ButtonComponent2D> buttonComponents = GetButtonComponentsAsSpan();

            foreach (ref Yotsuba entity in GetEntitiesAsSpan())
            {
                if (!entity.HasComponent(YTBComponent.Button2D)) continue;

                ref ButtonComponent2D button = ref buttonComponents[entity.Id];
                if (!button.IsActive) continue;

                if (button.EffectiveArea.Contains((int)_mousePosition.X, (int)_mousePosition.Y))
                {
                    if (_mousePressed)
                    {
                        button.Action?.Invoke();
                    }
                }
            }
        }

        public override void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time) { }
        public override void SharedEntityInitialize(ref Yotsuba Entidad) { }
        public override void Dispose() { }
    }
}
