using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Systems;
using Benchmarks.Core.Types;

namespace Benchmarks.Benchmarks
{
    /// <summary>
    /// Benchmarks individuales para cada sistema.
    /// EntityManager vive en NativeMemory, sistemas son structs con puntero.
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public unsafe class SystemBenchmarks
    {
        [Params(10, 100, 500, 1000)]
        public int EntityCount;

        private nint _emNint; // EntityManager*
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
            EntityManager* em = SceneFactory.CreateTestEntityManager(EntityCount);
            _emNint = (nint)em;
            _gameTime = new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(16.67));

            _physics = default;
            _physics.InitializeSystem(em);

            _render = default;
            _render.InitializeSystem(em);

            _animation = default;
            _animation.InitializeSystem(em);

            _input = default;
            _input.InitializeSystem(em);

            _button = default;
            _button.InitializeSystem(em);

            _camera = default;
            _camera.InitializeSystem(em);

            _script = default;
            _script.InitializeSystem(em);

            _systemBuilder = default;
            _systemBuilder.InitializeSystem(em);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            if (_emNint != 0)
            {
                EntityManager.Destroy((EntityManager*)_emNint);
                _emNint = 0;
            }
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
            EntityManager* em = (EntityManager*)_emNint;
            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int count = em->YotsubaEntities.Count;
            for (int i = 0; i < count; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                _script.SharedEntityForEachUpdate(ref entity, _gameTime);
            }
        }
    }
}
