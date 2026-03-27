using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Benchmarks
{
    /// <summary>
    /// Benchmarks del lifecycle completo de la escena: Initialize, Update, Draw, FullFrame.
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public class FullSceneBenchmarks
    {
        [Params(10, 100, 500, 1000)]
        public int EntityCount;

        private Scene _scene;
        private GameTime _gameTime;

        [GlobalSetup]
        public void Setup()
        {
            _scene = SceneFactory.CreateTestScene(EntityCount);
            _gameTime = new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(16.67));
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _scene?.Dispose();
        }

        [Benchmark(Description = "Scene.Initialize (create + init all systems)")]
        public Scene Initialize()
        {
            var scene = SceneFactory.CreateTestScene(EntityCount);
            return scene;
        }

        [Benchmark(Description = "Scene.Update (single frame)")]
        public void Update()
        {
            _scene.Update(_gameTime);
        }

        [Benchmark(Description = "Scene.Draw (single frame)")]
        public void Draw()
        {
            _scene.Draw(_gameTime);
        }

        [Benchmark(Description = "Full Frame (Update + Draw)")]
        public void FullFrame()
        {
            _scene.Update(_gameTime);
            _scene.Draw(_gameTime);
        }
    }
}
