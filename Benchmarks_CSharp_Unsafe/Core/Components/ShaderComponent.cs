namespace Benchmarks.Core.Components
{
    public struct ShaderComponent
    {
        public int ShaderId;
        public bool IsActive;

        public ShaderComponent(int shaderId, bool isActive)
        {
            ShaderId = shaderId;
            IsActive = isActive;
        }
    }
}
