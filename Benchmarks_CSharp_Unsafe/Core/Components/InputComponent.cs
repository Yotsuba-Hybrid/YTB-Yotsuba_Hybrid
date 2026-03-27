using System.Runtime.CompilerServices;

namespace Benchmarks.Core.Components
{
    [Flags]
    public enum InputInUse
    {
        None = 0,
        HasMouse = 1 << 0,
        HasGamepad = 1 << 1,
        HasKeyboard = 1 << 2,
    }

    public enum ActionEntityInput
    {
        None,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Jump,
        Attack,
        Interact,
    }

    /// <summary>
    /// InlineArray replaces Dictionary&lt;ActionEntityInput, int&gt;.
    /// ActionEntityInput has 8 values (0-7) → fixed 8 slots, bitmask for presence.
    /// </summary>
    [InlineArray(8)]
    public struct InputActionSlots
    {
        private int _element0;
    }

    public struct InputComponent
    {
        private int InputsInUse;
        public InputActionSlots KeyBoard;
        public int KeyBoardMask;
        public InputActionSlots GamePad;
        public int GamePadMask;
        public InputActionSlots Mouse;
        public int MouseMask;
        public int GamePadIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasInput(InputInUse input) => (InputsInUse & (int)input) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddInput(InputInUse input) => InputsInUse |= (int)input;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveInput(InputInUse input) => InputsInUse &= ~(int)input;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetKeyBoardMapping(ActionEntityInput action, int key)
        {
            KeyBoard[(int)action] = key;
            KeyBoardMask |= (1 << (int)action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMouseMapping(ActionEntityInput action, int key)
        {
            Mouse[(int)action] = key;
            MouseMask |= (1 << (int)action);
        }
    }
}
