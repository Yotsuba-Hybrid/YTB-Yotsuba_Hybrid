using System.Runtime.CompilerServices;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    public unsafe struct ButtonSystem2D
    {
        private nint _emPtr;
        private Vector2 _mousePosition;
        private bool _mousePressed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitializeSystem(EntityManager* entities)
        {
            _emPtr = (nint)entities;
            _mousePosition = new Vector2(500, 400);
            _mousePressed = false;
        }

        public void UpdateSystem(GameTime gameTime)
        {
            EntityManager* em = (EntityManager*)_emPtr;
            if (em == null) return;

            ButtonComponent2D* buttons = em->Button2DComponents.Ptr;
            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int count = em->YotsubaEntities.Count;

            for (int i = 0; i < count; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if (!entity.HasComponent(YTBComponent.Button2D)) continue;

                ref ButtonComponent2D button = ref Unsafe.Add(ref Unsafe.AsRef<ButtonComponent2D>(buttons), entity.Id);
                if (!button.IsActive) continue;

                if (button.EffectiveArea.Contains((int)_mousePosition.X, (int)_mousePosition.Y))
                {
                    if (_mousePressed && button.ActionPtr != 0)
                    {
                        ((delegate* managed<void>)button.ActionPtr)();
                    }
                }
            }
        }

        public void SharedEntityForEachUpdate(ref Yotsuba e, GameTime t) { }
        public void SharedEntityInitialize(ref Yotsuba e) { }
        public void Dispose() { }
    }
}
