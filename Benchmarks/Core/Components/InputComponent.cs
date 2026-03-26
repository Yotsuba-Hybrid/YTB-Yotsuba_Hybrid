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

    public struct InputComponent()
    {
        private int InputsInUse { get; set; } = 0;

        public Dictionary<ActionEntityInput, int> KeyBoard { get; set; }
        public Dictionary<ActionEntityInput, int> GamePad { get; set; }
        public Dictionary<ActionEntityInput, int> Mouse { get; set; }
        public int GamePadIndex { get; set; }

        public bool HasInput(InputInUse input) => (InputsInUse & (int)input) != 0;
        public void AddInput(InputInUse input) => InputsInUse |= (int)input;
        public void RemoveInput(InputInUse input) => InputsInUse &= ~(int)input;
    }
}
