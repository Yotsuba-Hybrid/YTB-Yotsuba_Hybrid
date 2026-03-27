namespace Benchmarks.Core.Systems

open Benchmarks.Core.Components
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Systems.Contract
open Benchmarks.Core.Types

type ScriptSystem() =
    inherit ISystem()

    override this.InitializeSystem(entities: EntityManager) =
        this.EntityManager <- entities

    override this.SharedEntityInitialize(entidad: byref<Yotsuba>) =
        if not (entidad.HasComponent(YTBComponent.Script)) then ()
        else
        let script = &this.GetScriptComponent(entidad.Id)
        script.IsActive |> ignore

    override this.SharedEntityForEachUpdate(entidad: byref<Yotsuba>, time: GameTime) =
        if not (entidad.HasComponent(YTBComponent.Script)) then ()
        else
        let script = &this.GetScriptComponent(entidad.Id)
        if not script.IsActive then ()
        else
        if entidad.HasComponent(YTBComponent.Transform) then
            let transform = &this.GetTransform2DComponent(entidad.Id)
            transform.Position |> ignore

    member this.DrawSystem2D(gameTime: GameTime) =
        if isNull this.EntityManager then ()
        else
        let entities = this.GetEntitiesAsSpan()
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            if entity.HasComponent(YTBComponent.Script) then
                let script = &this.GetScriptComponent(entity.Id)
                if script.IsActive then
                    script.ScriptCount |> ignore

    member _.DrawSystem3D(gameTime: GameTime) = ()

    member _.Clear() = ()
    override _.Dispose() = ()
