namespace Benchmarks.Core.Systems

open System.Collections.Generic
open Benchmarks.Core.Components
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Systems.Contract
open Benchmarks.Core.Types

type InputSystem() =
    inherit ISystem()

    let mutable eventManager: EventManager = Unchecked.defaultof<EventManager>
    let _pressedKeys = HashSet<int>([ 0; 1; 2; 3 ])
    let _mousePosition = Vector2(500f, 400f)

    override this.InitializeSystem(entities: EntityManager) =
        eventManager <- EventManager.Instance
        this.EntityManager <- entities

    override this.UpdateSystem(gameTime: GameTime) =
        if isNull this.EntityManager then ()
        else
        let inputComponents = this.GetInputComponentsAsSpan()
        let entities = this.EntityManager.YotsubaEntities.AsSpan()
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            if entity.HasComponent(YTBComponent.Input) then
                let inputComponent = &inputComponents.[entity.Id]
                InputSystem.ProcessKeyboard(_pressedKeys, entity.Id, &inputComponent, gameTime)
                InputSystem.ProcessMouse(_mousePosition, entity.Id, &inputComponent, gameTime)

    static member private ProcessKeyboard(pressedKeys: HashSet<int>, entityId: int, inputComponent: byref<InputComponent>, gameTime: GameTime) =
        if not (inputComponent.HasInput(InputInUse.HasKeyboard)) then ()
        elif isNull inputComponent.KeyBoard then ()
        else
            for mapping in inputComponent.KeyBoard do
                if pressedKeys.Contains(mapping.Value) then
                    mapping.Key |> ignore

    static member private ProcessMouse(mousePosition: Vector2, entityId: int, inputComponent: byref<InputComponent>, gameTime: GameTime) =
        if not (inputComponent.HasInput(InputInUse.HasMouse)) then ()
        elif isNull inputComponent.Mouse then ()
        else
            for _mapping in inputComponent.Mouse do
                mousePosition |> ignore

    override _.SharedEntityForEachUpdate(_, _) = ()
    override _.SharedEntityInitialize(_) = ()
    override _.Dispose() = ()
