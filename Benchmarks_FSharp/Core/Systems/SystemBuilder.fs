namespace Benchmarks.Core.Systems

open System
open Benchmarks.Core.Collections
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Systems.Contract
open Benchmarks.Core.Types

type SystemBuilder() =
    inherit IRenderSystem()

    static member val Systems = YTB<Func<ISystem>>() with get, set
    member val SystemsInstances = YTB<ISystem>() with get, set

    static member AddSystem<'T when 'T :> ISystem and 'T: (new: unit -> 'T)>() =
        SystemBuilder.Systems.Add(Func<ISystem>(fun () -> new 'T() :> ISystem)) |> ignore

    override this.InitializeSystem(entities: EntityManager) =
        this.SystemsInstances.Clear()
        let systems = SystemBuilder.Systems.AsSpan()
        for i = 0 to systems.Length - 1 do
            this.SystemsInstances.Add(systems.[i].Invoke()) |> ignore

        let instances = this.SystemsInstances.AsSpan()
        for i = 0 to instances.Length - 1 do
            instances.[i].InitializeSystem(entities)

    override this.SharedEntityInitialize(entidad: byref<Yotsuba>) =
        let instances = this.SystemsInstances.AsSpan()
        for i = 0 to instances.Length - 1 do
            instances.[i].SharedEntityInitialize(&entidad)

    override this.UpdateSystem(gameTime: GameTime) =
        let instances = this.SystemsInstances.AsSpan()
        for i = 0 to instances.Length - 1 do
            instances.[i].UpdateSystem(gameTime)

    override this.SharedEntityForEachUpdate(entidad: byref<Yotsuba>, time: GameTime) =
        let instances = this.SystemsInstances.AsSpan()
        for i = 0 to instances.Length - 1 do
            instances.[i].SharedEntityForEachUpdate(&entidad, time)

    override this.Render2D(gameTime: GameTime) =
        let instances = this.SystemsInstances.AsSpan()
        for i = 0 to instances.Length - 1 do
            match instances.[i] with
            | :? IRenderSystem as system -> system.Render2D(gameTime)
            | _ -> ()

    override this.Render3D(gameTime: GameTime) =
        let instances = this.SystemsInstances.AsSpan()
        for i = 0 to instances.Length - 1 do
            match instances.[i] with
            | :? IRenderSystem as system -> system.Render3D(gameTime)
            | _ -> ()

    override this.Dispose() =
        let instances = this.SystemsInstances.AsSpan()
        for i = 0 to instances.Length - 1 do
            instances.[i].Dispose()
        this.SystemsInstances.Clear()
