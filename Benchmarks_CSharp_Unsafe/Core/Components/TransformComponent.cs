using System.Runtime.CompilerServices;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct TransformComponent
    {
        public Vector3 Position;
        public Vector3 Size;
        public float Scale;
        public float Rotation;
        public SpriteEffects SpriteEffects;
        public Color Color;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TransformComponent(Vector3 position, Vector3 size, float scale, SpriteEffects spriteEffects, Color color)
        {
            Unsafe.SkipInit(out this);
            Position = position;
            Size = size;
            Scale = scale;
            Rotation = 0f;
            SpriteEffects = spriteEffects;
            Color = color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TransformComponent(Vector3 position, Vector3 size)
            : this(position, size, 1f, SpriteEffects.None, Color.White) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TransformComponent(Vector3 position, Vector3 size, float scale)
            : this(position, size, scale, SpriteEffects.None, Color.White) { }
    }
}
