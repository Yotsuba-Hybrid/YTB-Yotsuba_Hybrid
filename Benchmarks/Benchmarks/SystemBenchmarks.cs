using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Systems;
using Benchmarks.Core.Types;

namespace Benchmarks.Benchmarks
{
    /// <summary>
    /// Benchmarks individuales para cada sistema del engine.
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public class SystemBenchmarks
    {
        [Params(10, 100, 500, 1000)]
        public int EntityCount;

        private EntityManager _entityManager;
        private GameTime _gameTime;

        private PhysicsSystem2D _physics;
        private RenderSystem2D _render;
        private AnimationSystem2D _animation;
        private InputSystem _input;
        private ButtonSystem2D _button;
        private CameraSystem _camera;
        private ScriptSystem _script;
        private SystemBuilder _systemBuilder;

        [GlobalSetup]
        public void Setup()
        {
            EventManager.Reset();
            _entityManager = SceneFactory.CreateTestEntityManager(EntityCount);
            _gameTime = new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(16.67));

            _physics = new PhysicsSystem2D();
            _physics.InitializeSystem(_entityManager);

            _render = new RenderSystem2D();
            _render.InitializeSystem(_entityManager);

            _animation = new AnimationSystem2D();
            _animation.InitializeSystem(_entityManager);

            _input = new InputSystem();
            _input.InitializeSystem(_entityManager);

            _button = new ButtonSystem2D();
            _button.InitializeSystem(_entityManager);

            _camera = new CameraSystem();
            _camera.InitializeSystem(_entityManager);

            _script = new ScriptSystem();
            _script.InitializeSystem(_entityManager);

            _systemBuilder = new SystemBuilder();
            _systemBuilder.InitializeSystem(_entityManager);
        }

        [Benchmark(Description = "PhysicsSystem2D.UpdateSystem")]
        public void PhysicsSystem()
        {
            _physics.UpdateSystem(_gameTime);
        }

        [Benchmark(Description = "RenderSystem2D.UpdateSystem (multi-pass + culling)")]
        public void RenderSystem()
        {
            _render.UpdateSystem(_gameTime);
        }

        [Benchmark(Description = "AnimationSystem2D.UpdateSystem")]
        public void AnimationSystem()
        {
            _animation.UpdateSystem(_gameTime);
        }

        [Benchmark(Description = "InputSystem.UpdateSystem")]
        public void InputSystemBench()
        {
            _input.UpdateSystem(_gameTime);
        }

        [Benchmark(Description = "ButtonSystem2D.UpdateSystem")]
        public void ButtonSystem()
        {
            _button.UpdateSystem(_gameTime);
        }

        [Benchmark(Description = "CameraSystem.UpdateSystem")]
        public void CameraSystemBench()
        {
            _camera.UpdateSystem(_gameTime);
        }

        [Benchmark(Description = "SharedEntityForEach loop (Script + SystemBuilder)")]
        public void SharedEntityLoop()
        {
            foreach (ref Yotsuba entity in _entityManager.YotsubaEntities.AsSpan())
            {
                _script.SharedEntityForEachUpdate(ref entity, _gameTime);
                _systemBuilder.SharedEntityForEachUpdate(ref entity, _gameTime);
            }
        }
    }
}
