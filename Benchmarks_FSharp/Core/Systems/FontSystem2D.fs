namespace Benchmarks.Core.Systems

open Benchmarks.Core.Components
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Systems.Contract
open Benchmarks.Core.Types

type FontSystem2D() =
    inherit ISystem()

    member val DrawCallCount = 0 with get, set

    override this.InitializeSystem(entities: EntityManager) =
        this.EntityManager <- entities

    override this.SharedEntityInitialize(entidad: byref<Yotsuba>) =
        if not (entidad.HasComponent(YTBComponent.Font)) then ()
        else
        let font = &this.GetFontComponent(entidad.Id)
        font.IsVisible |> ignore

    member this.DrawSystem(gameTime: GameTime) =
        this.DrawCallCount <- 0
        if isNull this.EntityManager then ()
        else
        let fontComponents = this.GetFontComponentsAsSpan()
        let transforms = this.GetTransformComponentsAsSpan()
        let entities = this.GetEntitiesAsSpan()
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            if entity.HasComponent(YTBComponent.Font) then
                let font = &fontComponents.[entity.Id]
                if font.IsVisible then
                    let transform = &transforms.[entity.Id]
                    transform.Position |> ignore
                    font.Text |> ignore
                    this.DrawCallCount <- this.DrawCallCount + 1

    override _.Dispose() = ()
