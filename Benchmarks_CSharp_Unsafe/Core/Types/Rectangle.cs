using System.Runtime.CompilerServices;

namespace Benchmarks.Core.Types
{
    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle(int x, int y, int width, int height)
        {
            Unsafe.SkipInit(out this);
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int Left
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => X;
        }
        public int Right
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => X + Width;
        }
        public int Top
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Y;
        }
        public int Bottom
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Y + Height;
        }

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
