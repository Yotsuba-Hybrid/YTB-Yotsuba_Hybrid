using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct SpriteComponent2D(int textureId, Rectangle sourceRectangle)
    {
        public int TextureId { get; set; } = textureId;
        public Rectangle SourceRectangle { get; set; } = sourceRectangle;
        public bool IsVisible { get; set; } = true;
        public bool Is2_5D { get; set; } = false;
    }
}
