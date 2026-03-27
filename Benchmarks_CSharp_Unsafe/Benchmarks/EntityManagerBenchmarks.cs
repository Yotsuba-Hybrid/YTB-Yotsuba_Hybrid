using System.Runtime.CompilerServices;
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
    /// Benchmarks for NativeMemory-backed EntityManager.
    /// All iteration via Unsafe.Add pointer traversal.
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public unsafe class EntityManagerBenchmarks
    {
        [Params(100, 500, 1000)]
        public int EntityCount;

        private nint _emNint; // EntityManager*

        [GlobalSetup]
        public void Setup()
        {
            EntityManager* em = SceneFactory.CreateTestEntityManager(EntityCount);
            _emNint = (nint)em;
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

        [Benchmark(Description = "AddEntity (N entities with all component slots)")]
        public nint AddEntities()
        {
            EntityManager* em = EntityManager.Create();
            for (int i = 0; i < EntityCount; i++)
            {
                Unsafe.SkipInit(out Yotsuba entity);
                entity.Id = i;
                entity.Name = FixedString32.FromInt("E_", i);
                entity.Components = 0;
                em->AddEntity(ref entity);
            }
            nint result = (nint)em;
            return result;
        }

        [Benchmark(Description = "AddEntity + AddTransformComponent")]
        public nint AddEntitiesWithComponents()
        {
            EntityManager* em = EntityManager.Create();
            for (int i = 0; i < EntityCount; i++)
            {
                Unsafe.SkipInit(out Yotsuba entity);
                entity.Id = i;
                entity.Name = FixedString32.FromInt("E_", i);
                entity.Components = 0;
                em->AddEntity(ref entity);
                em->AddTransformComponent(entity, new TransformComponent(
                    new Vector3(i * 10f, i * 5f, 0),
                    new Vector3(32, 32, 0)
                ));
            }
            nint result = (nint)em;
            return result;
        }

        [Benchmark(Description = "Component access by entity.Id (Transform via Unsafe.Add)")]
        public float ComponentAccessById()
        {
            EntityManager* em = (EntityManager*)_emNint;
            float sum = 0;
            Yotsuba* entities = em->YotsubaEntities.Ptr;
            TransformComponent* transforms = em->TransformComponents.Ptr;
            int count = em->YotsubaEntities.Count;

            for (int i = 0; i < count; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if (entity.HasComponent(YTBComponent.Transform))
                {
                    ref TransformComponent t = ref Unsafe.Add(ref Unsafe.AsRef<TransformComponent>(transforms), entity.Id);
                    sum += t.Position.X + t.Position.Y;
                }
            }
            return sum;
        }

        [Benchmark(Description = "HasComponent bitmask check (all entities)")]
        public int BitmaskCheck()
        {
            EntityManager* em = (EntityManager*)_emNint;
            int count = 0;
            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int entityCount = em->YotsubaEntities.Count;

            for (int i = 0; i < entityCount; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
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

        [Benchmark(Description = "Entity iteration with Unsafe.Add + multi-component ref access")]
        public float FullEntityIteration()
        {
            EntityManager* em = (EntityManager*)_emNint;
            float sum = 0;
            Yotsuba* entities = em->YotsubaEntities.Ptr;
            TransformComponent* transforms = em->TransformComponents.Ptr;
            RigidBodyComponent2D* rigidbodies = em->Rigidbody2DComponents.Ptr;
            int count = em->YotsubaEntities.Count;

            for (int i = 0; i < count; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if (!entity.HasComponent(YTBComponent.Transform) || !entity.HasComponent(YTBComponent.Rigibody)) continue;

                ref TransformComponent transform = ref Unsafe.Add(ref Unsafe.AsRef<TransformComponent>(transforms), entity.Id);
                ref RigidBodyComponent2D rb = ref Unsafe.Add(ref Unsafe.AsRef<RigidBodyComponent2D>(rigidbodies), entity.Id);

                sum += transform.Position.X + transform.Position.Y + rb.Velocity.X + rb.Velocity.Y;
            }
            return sum;
        }
    }
}
