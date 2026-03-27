using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct SpriteComponent2D
    {
        public int TextureId;
        public Rectangle SourceRectangle;
        public bool IsVisible;
        public bool Is2_5D;

        public SpriteComponent2D(int textureId, Rectangle sourceRectangle)
        {
            TextureId = textureId;
            SourceRectangle = sourceRectangle;
            IsVisible = true;
            Is2_5D = false;
        }
    }
}
