using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct FontComponent2D
    {
        public string Text { get; set; }
        public int FontId { get; set; }
        public Color Color { get; set; }
        public float FontScale { get; set; }
        public bool IsVisible { get; set; }
    }
}
