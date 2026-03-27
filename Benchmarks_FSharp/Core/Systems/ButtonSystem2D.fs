namespace Benchmarks.Core.Systems

open Benchmarks.Core.Components
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Systems.Contract
open Benchmarks.Core.Types

type ButtonSystem2D() =
    inherit ISystem()

    let _mousePosition = Vector2(500f, 400f)
    let mutable _mousePressed = false

    override this.InitializeSystem(entities: EntityManager) =
        this.EntityManager <- entities

    override this.UpdateSystem(gameTime: GameTime) =
        if isNull this.EntityManager then ()
        else
        let buttonComponents = this.GetButtonComponentsAsSpan()
        let entities = this.GetEntitiesAsSpan()
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            if entity.HasComponent(YTBComponent.Button2D) then
                let button = &buttonComponents.[entity.Id]
                if button.IsActive then
                    if button.EffectiveArea.Contains(int _mousePosition.X, int _mousePosition.Y) then
                        if _mousePressed then
                            if not (isNull button.Action) then
                                button.Action.Invoke()

    override _.SharedEntityForEachUpdate(_, _) = ()
    override _.SharedEntityInitialize(_) = ()
    override _.Dispose() = ()
