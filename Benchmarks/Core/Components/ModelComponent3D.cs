using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct ModelComponent3D
    {
        public int ModelId { get; set; }
        public bool IsVisible { get; set; }
        public float RadiusSphere { get; set; }
        public Vector3 SphereOffset { get; set; }
    }
}
