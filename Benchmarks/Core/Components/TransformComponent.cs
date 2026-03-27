using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct TransformComponent(Vector3 position, Vector3 size, float scale, SpriteEffects spriteEffects, Color color)
    {
        public void SetPosition(float x, float y, float z) => Position = new Vector3(x, y, z);

        public Vector3 Size { get; set; } = size;
        public float Scale { get; set; } = scale;
        public float Rotation { get; set; } = 0f;
        public Vector3 Position { get; set; } = position;
        public SpriteEffects SpriteEffects { get; set; } = spriteEffects;
        public Color Color { get; set; } = color;

        public TransformComponent(Vector3 position, Vector3 size) : this(position, size, 1f, SpriteEffects.None, Color.White) { }
        public TransformComponent(Vector3 position, Vector3 size, float scale) : this(position, size, scale, SpriteEffects.None, Color.White) { }
    }
}
