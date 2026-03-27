namespace Benchmarks.Benchmarks

open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Jobs
open Benchmarks.Core.Collections
open Benchmarks.Core.Components
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Types

[<MemoryDiagnoser>]
[<SimpleJob(RuntimeMoniker.HostProcess)>]
type EntityManagerBenchmarks() =

    [<Params(100, 500, 1000)>]
    member val EntityCount = 0 with get, set

    member val private _entityManager: EntityManager = Unchecked.defaultof<EntityManager> with get, set

    [<GlobalSetup>]
    member this.Setup() =
        this._entityManager <- SceneFactory.CreateTestEntityManager(this.EntityCount)

    [<Benchmark(Description = "AddEntity (N entities with all component slots)")>]
    member this.AddEntities() =
        let em = EntityManager()
        for i = 0 to this.EntityCount - 1 do
            let mutable entity = Yotsuba(i)
            entity.Name <- $"E_{i}"
            em.AddEntity(&entity)
        em

    [<Benchmark(Description = "AddEntity + AddTransformComponent")>]
    member this.AddEntitiesWithComponents() =
        let em = EntityManager()
        for i = 0 to this.EntityCount - 1 do
            let mutable entity = Yotsuba(i)
            entity.Name <- $"E_{i}"
            em.AddEntity(&entity)
            em.AddTransformComponent(entity, TransformComponent(
                Vector3(float32 i * 10f, float32 i * 5f, 0f),
                Vector3(32f, 32f, 0f)))
        em

    [<Benchmark(Description = "Component access by entity.Id (Transform)")>]
    member this.ComponentAccessById() =
        let mutable sum = 0f
        let entities = this._entityManager.YotsubaEntities.AsSpan()
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            if entity.HasComponent(YTBComponent.Transform) then
                let t = &this._entityManager.TransformComponents.[entity.Id]
                sum <- sum + t.Position.X + t.Position.Y
        sum

    [<Benchmark(Description = "HasComponent bitmask check (all entities)")>]
    member this.BitmaskCheck() =
        let mutable count = 0
        let entities = this._entityManager.YotsubaEntities.AsSpan()
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            if entity.HasComponent(YTBComponent.Transform) then count <- count + 1
            if entity.HasComponent(YTBComponent.Sprite) then count <- count + 1
            if entity.HasComponent(YTBComponent.Rigibody) then count <- count + 1
            if entity.HasComponent(YTBComponent.Animation) then count <- count + 1
            if entity.HasComponent(YTBComponent.Input) then count <- count + 1
            if entity.HasComponent(YTBComponent.Button2D) then count <- count + 1
            if entity.HasComponent(YTBComponent.Shader) then count <- count + 1
        count

    [<Benchmark(Description = "Entity iteration with Span<T> + multi-component ref access")>]
    member this.FullEntityIteration() =
        let mutable sum = 0f
        let entities = this._entityManager.YotsubaEntities.AsSpan()
        let transforms = this._entityManager.TransformComponents.AsSpan()
        let rigidbodies = this._entityManager.Rigidbody2DComponents.AsSpan()

        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            if entity.HasComponent(YTBComponent.Transform) && entity.HasComponent(YTBComponent.Rigibody) then
                let transform = &transforms.[entity.Id]
                let rb = &rigidbodies.[entity.Id]
                sum <- sum + transform.Position.X + transform.Position.Y + rb.Velocity.X + rb.Velocity.Y
        sum
