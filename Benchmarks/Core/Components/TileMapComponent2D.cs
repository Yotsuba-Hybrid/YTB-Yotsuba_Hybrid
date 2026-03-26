using Benchmarks.Core.Collections;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct TileLayer
    {
        public string Name { get; set; }
        public int[] Data { get; set; }
    }

    public struct TileMapComponent2D
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public YTB<TileLayer> TileLayers { get; set; }
        public Dictionary<int, List<Rectangle>> Collisions { get; set; }
    }
}
