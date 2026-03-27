namespace Benchmarks.Benchmarks

open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Jobs
open Benchmarks.Core.Scene
open Benchmarks.Core.Types

[<MemoryDiagnoser>]
[<SimpleJob(RuntimeMoniker.HostProcess)>]
type FullSceneBenchmarks() =

    [<Params(10, 100, 500, 1000)>]
    member val EntityCount = 0 with get, set

    member val private _scene: Scene = Unchecked.defaultof<Scene> with get, set
    member val private _gameTime: GameTime = Unchecked.defaultof<GameTime> with get, set

    [<GlobalSetup>]
    member this.Setup() =
        this._scene <- SceneFactory.CreateTestScene(this.EntityCount)
        this._gameTime <- GameTime(TimeSpan.FromSeconds(1.0), TimeSpan.FromMilliseconds(16.67))

    [<GlobalCleanup>]
    member this.Cleanup() =
        if not (isNull (box this._scene)) then
            (this._scene :> IDisposable).Dispose()

    [<Benchmark(Description = "Scene.Initialize (create + init all systems)")>]
    member this.Initialize() =
        SceneFactory.CreateTestScene(this.EntityCount)

    [<Benchmark(Description = "Scene.Update (single frame)")>]
    member this.Update() =
        this._scene.Update(this._gameTime)

    [<Benchmark(Description = "Scene.Draw (single frame)")>]
    member this.Draw() =
        this._scene.Draw(this._gameTime)

    [<Benchmark(Description = "Full Frame (Update + Draw)")>]
    member this.FullFrame() =
        this._scene.Update(this._gameTime)
        this._scene.Draw(this._gameTime)
