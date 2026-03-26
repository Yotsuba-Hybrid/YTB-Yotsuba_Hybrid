namespace Benchmarks.Core.Types
{
    public struct Color(byte r, byte g, byte b, byte a)
    {
        public byte R { get; set; } = r;
        public byte G { get; set; } = g;
        public byte B { get; set; } = b;
        public byte A { get; set; } = a;

        public static readonly Color White = new(255, 255, 255, 255);
        public static readonly Color Black = new(0, 0, 0, 255);
        public static readonly Color Red = new(255, 0, 0, 255);
        public static readonly Color Green = new(0, 255, 0, 255);
        public static readonly Color Blue = new(0, 0, 255, 255);
    }
}
