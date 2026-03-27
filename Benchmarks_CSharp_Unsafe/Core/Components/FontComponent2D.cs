using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    /// <summary>
    /// Text field is FixedString32 instead of heap-allocated string.
    /// </summary>
    public struct FontComponent2D
    {
        public FixedString32 Text;
        public int FontId;
        public Color Color;
        public float FontScale;
        public bool IsVisible;
    }
}
