namespace Benchmarks.Core.Types
{
    /// <summary>
    /// Struct replacement for class GameTime. Zero heap allocation.
    /// </summary>
    public struct GameTime
    {
        public TimeSpan TotalGameTime;
        public TimeSpan ElapsedGameTime;

        public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime)
        {
            TotalGameTime = totalGameTime;
            ElapsedGameTime = elapsedGameTime;
        }
    }
}
