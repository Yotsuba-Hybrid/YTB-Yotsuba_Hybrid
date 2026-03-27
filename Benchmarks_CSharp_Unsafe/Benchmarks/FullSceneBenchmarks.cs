using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Benchmarks
{
    /// <summary>
    /// Benchmarks del lifecycle completo de la escena usando punteros nativos.
    /// Scene y EntityManager viven en NativeMemory.
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public unsafe class FullSceneBenchmarks
    {
        [Params(10, 100, 500, 1000)]
        public int EntityCount;

        private nint _scenePtr;
        private GameTime _gameTime;

        [GlobalSetup]
        public void Setup()
        {
            _scenePtr = SceneFactory.CreateTestScene(EntityCount);
            _gameTime = new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(16.67));
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            if (_scenePtr != 0)
            {
                SceneStruct.Destroy(_scenePtr);
                _scenePtr = 0;
            }
        }

        [Benchmark(Description = "Scene.Initialize (create + init all systems)")]
        public nint Initialize()
        {
            nint scene = SceneFactory.CreateTestScene(EntityCount);
            return scene;
        }

        [Benchmark(Description = "Scene.Update (single frame)")]
        public void Update()
        {
            SceneStruct* scene = (SceneStruct*)_scenePtr;
            scene->Update(_gameTime);
        }

        [Benchmark(Description = "Scene.Draw (single frame)")]
        public void Draw()
        {
            SceneStruct* scene = (SceneStruct*)_scenePtr;
            scene->Draw(_gameTime);
        }

        [Benchmark(Description = "Full Frame (Update + Draw)")]
        public void FullFrame()
        {
            SceneStruct* scene = (SceneStruct*)_scenePtr;
            scene->Update(_gameTime);
            scene->Draw(_gameTime);
        }
    }
}
