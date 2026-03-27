namespace Benchmarks.Benchmarks

open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Jobs
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Systems
open Benchmarks.Core.Types

[<MemoryDiagnoser>]
[<SimpleJob(RuntimeMoniker.HostProcess)>]
type SystemBenchmarks() =

    [<Params(10, 100, 500, 1000)>]
    member val EntityCount = 0 with get, set

    member val private _entityManager: EntityManager = Unchecked.defaultof<EntityManager> with get, set
    member val private _gameTime: GameTime = Unchecked.defaultof<GameTime> with get, set

    member val private _physics: PhysicsSystem2D = Unchecked.defaultof<PhysicsSystem2D> with get, set
    member val private _render: RenderSystem2D = Unchecked.defaultof<RenderSystem2D> with get, set
    member val private _animation: AnimationSystem2D = Unchecked.defaultof<AnimationSystem2D> with get, set
    member val private _input: InputSystem = Unchecked.defaultof<InputSystem> with get, set
    member val private _button: ButtonSystem2D = Unchecked.defaultof<ButtonSystem2D> with get, set
    member val private _camera: CameraSystem = Unchecked.defaultof<CameraSystem> with get, set
    member val private _script: ScriptSystem = Unchecked.defaultof<ScriptSystem> with get, set
    member val private _systemBuilder: SystemBuilder = Unchecked.defaultof<SystemBuilder> with get, set

    [<GlobalSetup>]
    member this.Setup() =
        EventManager.Reset()
        this._entityManager <- SceneFactory.CreateTestEntityManager(this.EntityCount)
        this._gameTime <- GameTime(TimeSpan.FromSeconds(1.0), TimeSpan.FromMilliseconds(16.67))

        this._physics <- PhysicsSystem2D()
        this._physics.InitializeSystem(this._entityManager)

        this._render <- RenderSystem2D()
        this._render.InitializeSystem(this._entityManager)

        this._animation <- AnimationSystem2D()
        this._animation.InitializeSystem(this._entityManager)

        this._input <- InputSystem()
        this._input.InitializeSystem(this._entityManager)

        this._button <- ButtonSystem2D()
        this._button.InitializeSystem(this._entityManager)

        this._camera <- CameraSystem()
        this._camera.InitializeSystem(this._entityManager)

        this._script <- ScriptSystem()
        this._script.InitializeSystem(this._entityManager)

        this._systemBuilder <- SystemBuilder()
        this._systemBuilder.InitializeSystem(this._entityManager)

    [<Benchmark(Description = "PhysicsSystem2D.UpdateSystem")>]
    member this.PhysicsSystem() =
        this._physics.UpdateSystem(this._gameTime)

    [<Benchmark(Description = "RenderSystem2D.UpdateSystem (multi-pass + culling)")>]
    member this.RenderSystem() =
        this._render.UpdateSystem(this._gameTime)

    [<Benchmark(Description = "AnimationSystem2D.UpdateSystem")>]
    member this.AnimationSystem() =
        this._animation.UpdateSystem(this._gameTime)

    [<Benchmark(Description = "InputSystem.UpdateSystem")>]
    member this.InputSystemBench() =
        this._input.UpdateSystem(this._gameTime)

    [<Benchmark(Description = "ButtonSystem2D.UpdateSystem")>]
    member this.ButtonSystem() =
        this._button.UpdateSystem(this._gameTime)

    [<Benchmark(Description = "CameraSystem.UpdateSystem")>]
    member this.CameraSystemBench() =
        this._camera.UpdateSystem(this._gameTime)

    [<Benchmark(Description = "SharedEntityForEach loop (Script + SystemBuilder)")>]
    member this.SharedEntityLoop() =
        let entities = this._entityManager.YotsubaEntities.AsSpan()
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            this._script.SharedEntityForEachUpdate(&entity, this._gameTime)
            this._systemBuilder.SharedEntityForEachUpdate(&entity, this._gameTime)
