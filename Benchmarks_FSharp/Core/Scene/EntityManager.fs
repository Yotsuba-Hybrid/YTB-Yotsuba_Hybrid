namespace Benchmarks.Core.Scene

open System
open Benchmarks.Core.Collections
open Benchmarks.Core.Components
open Benchmarks.Core.Entity

type EntityManager(entities: YTB<Yotsuba>) =
    member val YotsubaEntities = entities with get, set

    member val TransformComponents = YTB<TransformComponent>() with get, set
    member val Sprite2DComponents = YTB<SpriteComponent2D>() with get, set
    member val Animation2DComponents = YTB<AnimationComponent2D>() with get, set
    member val Rigidbody2DComponents = YTB<RigidBodyComponent2D>() with get, set
    member val InputComponents = YTB<InputComponent>() with get, set
    member val ModelComponents3D = YTB<ModelComponent3D>() with get, set
    member val TileMapComponent2Ds = YTB<TileMapComponent2D>() with get, set
    member val Button2DComponents = YTB<ButtonComponent2D>() with get, set
    member val ScriptComponents = YTB<ScriptComponent>() with get, set
    member val Font2DComponents = YTB<FontComponent2D>() with get, set
    member val ShaderComponents = YTB<ShaderComponent>() with get, set
    member val YtbModelComponents = YTB<YTBModelComponent3D>() with get, set

    member val CameraEntityId = -1 with get, set
    member val CameraZoom = 1.0f with get, set

    new() = EntityManager(YTB<Yotsuba>())

    member this.AddEntity(entity: byref<Yotsuba>) =
        let index = this.YotsubaEntities.Add(entity)
        this.YotsubaEntities.[index].Id <- index
        entity.Id <- index
        let mutable componentIndex = 0

        if this.TransformComponents.Add(Unchecked.defaultof<TransformComponent>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<TransformComponent>}. Index {index} != {componentIndex}"))
        if this.Sprite2DComponents.Add(Unchecked.defaultof<SpriteComponent2D>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<SpriteComponent2D>}. Index {index} != {componentIndex}"))
        if this.Animation2DComponents.Add(Unchecked.defaultof<AnimationComponent2D>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<AnimationComponent2D>}. Index {index} != {componentIndex}"))
        if this.Rigidbody2DComponents.Add(Unchecked.defaultof<RigidBodyComponent2D>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<RigidBodyComponent2D>}. Index {index} != {componentIndex}"))
        if this.InputComponents.Add(Unchecked.defaultof<InputComponent>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<InputComponent>}. Index {index} != {componentIndex}"))
        if this.ModelComponents3D.Add(Unchecked.defaultof<ModelComponent3D>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<ModelComponent3D>}. Index {index} != {componentIndex}"))
        if this.Button2DComponents.Add(Unchecked.defaultof<ButtonComponent2D>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<ButtonComponent2D>}. Index {index} != {componentIndex}"))
        if this.ScriptComponents.Add(Unchecked.defaultof<ScriptComponent>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<ScriptComponent>}. Index {index} != {componentIndex}"))
        if this.TileMapComponent2Ds.Add(Unchecked.defaultof<TileMapComponent2D>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<TileMapComponent2D>}. Index {index} != {componentIndex}"))
        if this.Font2DComponents.Add(Unchecked.defaultof<FontComponent2D>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<FontComponent2D>}. Index {index} != {componentIndex}"))
        if this.ShaderComponents.Add(Unchecked.defaultof<ShaderComponent>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<ShaderComponent>}. Index {index} != {componentIndex}"))
        if this.YtbModelComponents.Add(Unchecked.defaultof<YTBModelComponent3D>, &componentIndex) <> index then
            raise (InvalidOperationException($"Component entered at different index than entity. Component: {typeof<YTBModelComponent3D>}. Index {index} != {componentIndex}"))

    member this.AddTransformComponent(entity: Yotsuba, component': TransformComponent) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.Transform)
        this.TransformComponents.SetByUint(uint entity.Id, component')

    member this.AddSpriteComponent(entity: Yotsuba, component': SpriteComponent2D) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.Sprite)
        this.Sprite2DComponents.SetByUint(uint entity.Id, component')

    member this.AddAnimationComponent(entity: Yotsuba, component': AnimationComponent2D) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.Animation)
        this.Animation2DComponents.SetByUint(uint entity.Id, component')

    member this.AddRigidbodyComponent(entity: Yotsuba, component': RigidBodyComponent2D) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.Rigibody)
        this.Rigidbody2DComponents.SetByUint(uint entity.Id, component')

    member this.AddInputComponent(entity: Yotsuba, component': InputComponent) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.Input)
        this.InputComponents.SetByUint(uint entity.Id, component')

    member this.AddButtonComponent2D(entity: Yotsuba, component': ButtonComponent2D) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.Button2D)
        this.Button2DComponents.SetByUint(uint entity.Id, component')

    member this.AddShaderComponent2D(entity: Yotsuba, component': ShaderComponent) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.Shader)
        this.ShaderComponents.SetByUint(uint entity.Id, component')

    member this.AddScriptComponent(entity: Yotsuba, script: ScriptComponent) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.Script)
        this.ScriptComponents.SetByUint(uint entity.Id, script)

    member this.AddFontComponent2D(entity: Yotsuba, component': FontComponent2D) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.Font)
        this.Font2DComponents.SetByUint(uint entity.Id, component')

    member this.AddTileMapComponent(entity: Yotsuba, component': TileMapComponent2D) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.TileMap)
        this.TileMapComponent2Ds.SetByUint(uint entity.Id, component')

    member this.AddModelComponent3D(entity: Yotsuba, component': ModelComponent3D) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.Model3D)
        this.ModelComponents3D.SetByUint(uint entity.Id, component')

    member this.AddYTBObject3D(entity: Yotsuba, component': YTBModelComponent3D) =
        this.YotsubaEntities.[entity.Id].AddComponent(YTBComponent.YTBModel3D)
        this.YtbModelComponents.SetByUint(uint entity.Id, component')
