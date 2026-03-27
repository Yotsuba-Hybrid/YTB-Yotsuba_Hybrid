using System.Runtime.CompilerServices;

namespace Benchmarks.Core.Types
{
    public struct Rectangle(int x, int y, int width, int height)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public int Width { get; set; } = width;
        public int Height { get; set; } = height;

        public int Left => X;
        public int Right => X + Width;
        public int Top => Y;
        public int Bottom => Y + Height;

        public static readonly Rectangle Empty = new(0, 0, 0, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(Rectangle other)
        {
            return Left < other.Right && Right > other.Left &&
                   Top < other.Bottom && Bottom > other.Top;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(int px, int py)
        {
            return px >= Left && px < Right && py >= Top && py < Bottom;
        }
    }
}
