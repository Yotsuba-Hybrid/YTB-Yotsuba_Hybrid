using System.Runtime.CompilerServices;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Entity
{
    [Flags]
    public enum YTBComponent : ushort
    {
        Sprite = 1 << 0,
        Transform = 1 << 1,
        Animation = 1 << 2,
        Rigibody = 1 << 3,
        Input = 1 << 4,
        Model3D = 1 << 5,
        Button2D = 1 << 6,
        Camera = 1 << 7,
        Script = 1 << 8,
        TileMap = 1 << 9,
        Font = 1 << 10,
        Shader = 1 << 11,
        YTBUIElement = 1 << 12,
        YTBModel3D = 1 << 14,
    }

    /// <summary>
    /// Fully unmanaged entity struct. Name is FixedString32 (no heap string).
    /// </summary>
    public struct Yotsuba
    {
        public int Id;
        public FixedString32 Name;
        public int Components;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Yotsuba(int id)
        {
            Unsafe.SkipInit(out this);
            Id = id;
            Name = default;
            Components = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasComponent(YTBComponent component) => (Components & (int)component) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasNotComponent(YTBComponent component) => !HasComponent(component);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent(YTBComponent component) => Components |= (int)component;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent(YTBComponent component) => Components &= ~(int)component;
    }
}
