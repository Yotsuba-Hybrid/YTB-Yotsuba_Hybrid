using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct ButtonComponent2D
    {
        public bool IsActive { get; set; }
        public Rectangle EffectiveArea { get; set; }
        public Action Action { get; set; }
    }
}
