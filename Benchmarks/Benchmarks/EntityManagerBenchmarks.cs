using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Core.Collections;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Benchmarks
{
    /// <summary>
    /// Benchmarks para EntityManager: AddEntity, component access, bitmask checks.
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public class EntityManagerBenchmarks
    {
        [Params(100, 500, 1000)]
        public int EntityCount;

        private EntityManager _entityManager;

        [GlobalSetup]
        public void Setup()
        {
            _entityManager = SceneFactory.CreateTestEntityManager(EntityCount);
        }

        [Benchmark(Description = "AddEntity (N entities with all component slots)")]
        public EntityManager AddEntities()
        {
            var em = new EntityManager();
            for (int i = 0; i < EntityCount; i++)
            {
                var entity = new Yotsuba(i) { Name = $"E_{i}" };
                em.AddEntity(ref entity);
            }
            return em;
        }

        [Benchmark(Description = "AddEntity + AddTransformComponent")]
        public EntityManager AddEntitiesWithComponents()
        {
            var em = new EntityManager();
            for (int i = 0; i < EntityCount; i++)
            {
                var entity = new Yotsuba(i) { Name = $"E_{i}" };
                em.AddEntity(ref entity);
                em.AddTransformComponent(entity, new TransformComponent(
                    new Vector3(i * 10f, i * 5f, 0),
                    new Vector3(32, 32, 0)
                ));
            }
            return em;
        }

        [Benchmark(Description = "Component access by entity.Id (Transform)")]
        public float ComponentAccessById()
        {
            float sum = 0;
            Span<Yotsuba> entities = _entityManager.YotsubaEntities.AsSpan();
            for (int i = 0; i < entities.Length; i++)
            {
                ref Yotsuba entity = ref entities[i];
                if (entity.HasComponent(YTBComponent.Transform))
                {
                    ref TransformComponent t = ref _entityManager.TransformComponents[entity.Id];
                    sum += t.Position.X + t.Position.Y;
                }
            }
            return sum;
        }

        [Benchmark(Description = "HasComponent bitmask check (all entities)")]
        public int BitmaskCheck()
        {
            int count = 0;
            foreach (ref Yotsuba entity in _entityManager.YotsubaEntities.AsSpan())
            {
                if (entity.HasComponent(YTBComponent.Transform)) count++;
                if (entity.HasComponent(YTBComponent.Sprite)) count++;
                if (entity.HasComponent(YTBComponent.Rigibody)) count++;
                if (entity.HasComponent(YTBComponent.Animation)) count++;
                if (entity.HasComponent(YTBComponent.Input)) count++;
                if (entity.HasComponent(YTBComponent.Button2D)) count++;
                if (entity.HasComponent(YTBComponent.Shader)) count++;
            }
            return count;
        }

        [Benchmark(Description = "Entity iteration with Span<T> + multi-component ref access")]
        public float FullEntityIteration()
        {
            float sum = 0;
            Span<Yotsuba> entities = _entityManager.YotsubaEntities.AsSpan();
            Span<TransformComponent> transforms = _entityManager.TransformComponents.AsSpan();
            Span<RigidBodyComponent2D> rigidbodies = _entityManager.Rigidbody2DComponents.AsSpan();

            foreach (ref Yotsuba entity in entities)
            {
                if (!entity.HasComponent(YTBComponent.Transform) || !entity.HasComponent(YTBComponent.Rigibody)) continue;

                ref TransformComponent transform = ref transforms[entity.Id];
                ref RigidBodyComponent2D rb = ref rigidbodies[entity.Id];

                sum += transform.Position.X + transform.Position.Y + rb.Velocity.X + rb.Velocity.Y;
            }
            return sum;
        }
    }
}
