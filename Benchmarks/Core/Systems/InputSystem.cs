using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Systems.Contract;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    /// <summary>
    /// Sistema encargado de manejar la entrada del usuario.
    /// Simula el procesamiento de input con dictionary lookups.
    /// </summary>
    public class InputSystem : ISystem
    {
        private EventManager EventManager { get; set; }

        // Simulated input state
        private readonly HashSet<int> _pressedKeys = new() { 0, 1, 2, 3 };
        private readonly Vector2 _mousePosition = new(500, 400);

        public override void InitializeSystem(EntityManager @entities)
        {
            EventManager = EventManager.Instance;
            EntityManager = @entities;
        }

        public override void UpdateSystem(GameTime gameTime)
        {
            if (EntityManager == null) return;

            Span<InputComponent> inputComponents = GetInputComponentsAsSpan();

            foreach (ref Yotsuba entity in EntityManager.YotsubaEntities.AsSpan())
            {
                if (entity.HasComponent(YTBComponent.Input))
                {
                    ref InputComponent inputComponent = ref inputComponents[entity.Id];
                    ProcessKeyboard(entity.Id, ref inputComponent, gameTime);
                    ProcessMouse(entity.Id, ref inputComponent, gameTime);
                }
            }
        }

        private void ProcessKeyboard(int entityId, ref InputComponent inputComponent, GameTime gameTime)
        {
            if (!inputComponent.HasInput(InputInUse.HasKeyboard)) return;
            if (inputComponent.KeyBoard == null) return;

            foreach (var mapping in inputComponent.KeyBoard)
            {
                if (_pressedKeys.Contains(mapping.Value))
                {
                    // Simulate key processing (equivalent to publishing events)
                    _ = mapping.Key;
                }
            }
        }

        private void ProcessMouse(int entityId, ref InputComponent inputComponent, GameTime gameTime)
        {
            if (!inputComponent.HasInput(InputInUse.HasMouse)) return;
            if (inputComponent.Mouse == null) return;

            foreach (var mapping in inputComponent.Mouse)
            {
                // Simulate mouse processing
                _ = _mousePosition;
            }
        }

        public override void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time) { }
        public override void SharedEntityInitialize(ref Yotsuba Entidad) { }
        public override void Dispose() { }
    }
}
