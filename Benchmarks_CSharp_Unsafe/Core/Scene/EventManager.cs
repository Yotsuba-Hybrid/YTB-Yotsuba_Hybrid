namespace Benchmarks.Core.Scene
{
    /// <summary>
    /// Minimal struct EventManager. No heap allocations.
    /// Events are not used in benchmark hot paths - provides API compatibility only.
    /// </summary>
    public struct EventManager
    {
        private static EventManager _instance;
        public static ref EventManager Instance => ref _instance;
        public static void Reset() { _instance = default; }
        public void Clear() { }
    }
}
