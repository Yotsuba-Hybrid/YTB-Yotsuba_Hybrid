using System.Numerics;
using System.Runtime.CompilerServices;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    public unsafe struct InputSystem
    {
        private nint _emPtr;

        // Simulated input state as bitmask (keys 0-3 pressed = 0b1111)
        private int _pressedKeysMask;
        private Types.Vector2 _mousePosition;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitializeSystem(EntityManager* entities)
        {
            _emPtr = (nint)entities;
            _pressedKeysMask = 0b1111; // keys 0,1,2,3
            _mousePosition = new Types.Vector2(500, 400);
        }

        public void UpdateSystem(GameTime gameTime)
        {
            EntityManager* em = (EntityManager*)_emPtr;
            if (em == null) return;

            InputComponent* inputs = em->InputComponents.Ptr;
            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int count = em->YotsubaEntities.Count;

            for (int i = 0; i < count; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if (!entity.HasComponent(YTBComponent.Input)) continue;

                ref InputComponent input = ref Unsafe.Add(ref Unsafe.AsRef<InputComponent>(inputs), entity.Id);
                ProcessKeyboard(ref input);
                ProcessMouse(ref input);
            }
        }

        /// <summary>
        /// Iterates keyboard mappings via bitmask (BitOperations.TrailingZeroCount).
        /// Replaces Dictionary foreach with zero-allocation bit iteration.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ProcessKeyboard(ref InputComponent input)
        {
            if (!input.HasInput(InputInUse.HasKeyboard)) return;
            int mask = input.KeyBoardMask;
            while (mask != 0)
            {
                int actionIdx = BitOperations.TrailingZeroCount(mask);
                int keyValue = input.KeyBoard[actionIdx];
                if ((_pressedKeysMask & (1 << keyValue)) != 0)
                {
                    _ = (ActionEntityInput)actionIdx;
                }
                mask &= mask - 1; // Clear lowest set bit
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ProcessMouse(ref InputComponent input)
        {
            if (!input.HasInput(InputInUse.HasMouse)) return;
            int mask = input.MouseMask;
            while (mask != 0)
            {
                _ = _mousePosition;
                mask &= mask - 1;
            }
        }

        public void SharedEntityForEachUpdate(ref Yotsuba e, GameTime t) { }
        public void SharedEntityInitialize(ref Yotsuba e) { }
        public void Dispose() { }
    }
}
