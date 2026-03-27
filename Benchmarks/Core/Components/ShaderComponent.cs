namespace Benchmarks.Core.Components
{
    public struct ShaderComponent(int shaderId, bool isActive)
    {
        public int ShaderId { get; set; } = shaderId;
        public bool IsActive { get; set; } = isActive;
    }
}
