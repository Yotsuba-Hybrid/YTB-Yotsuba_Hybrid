namespace Benchmarks.Core.Systems.Contract

open System
open Benchmarks.Core.Components
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Types

[<AbstractClass>]
type ISystem() =
    member val EntityManager: EntityManager = Unchecked.defaultof<EntityManager> with get, set

    abstract InitializeSystem: EntityManager -> unit
    default _.InitializeSystem(_) = ()

    abstract SharedEntityInitialize: entity: byref<Yotsuba> -> unit
    default _.SharedEntityInitialize(_) = ()

    abstract UpdateSystem: GameTime -> unit
    default _.UpdateSystem(_) = ()

    abstract SharedEntityForEachUpdate: entity: byref<Yotsuba> * time: GameTime -> unit
    default _.SharedEntityForEachUpdate(_, _) = ()

    abstract Dispose: unit -> unit
    default _.Dispose() = ()

    interface IDisposable with
        member this.Dispose() = this.Dispose()

    // --- Entities ---
    member this.GetEntitiesAsSpan() = this.EntityManager.YotsubaEntities.AsSpan()
    member this.GetEntity(entityId: int) = &this.EntityManager.YotsubaEntities.[entityId]
    member this.GetEntityByNameOrDefault(name: string) =
        &this.EntityManager.YotsubaEntities.Find(fun x -> x.Name = name)

    // --- Transform ---
    member this.GetTransformComponentsAsSpan() = this.EntityManager.TransformComponents.AsSpan()
    member this.GetTransform2DComponent(entityId: int) = &this.EntityManager.TransformComponents.[entityId]

    // --- Sprite 2D ---
    member this.GetSpriteComponentsAsSpan() = this.EntityManager.Sprite2DComponents.AsSpan()
    member this.GetSprite2DComponent(entityId: int) = &this.EntityManager.Sprite2DComponents.[entityId]

    // --- TileMap 2D ---
    member this.GetTilemapComponentsAsSpan() = this.EntityManager.TileMapComponent2Ds.AsSpan()
    member this.GetTilemapComponent(entityId: int) = &this.EntityManager.TileMapComponent2Ds.[entityId]

    // --- Shader ---
    member this.GetShaderComponentsAsSpan() = this.EntityManager.ShaderComponents.AsSpan()
    member this.GetShaderComponent(entityId: int) = &this.EntityManager.ShaderComponents.[entityId]

    // --- Script ---
    member this.GetScriptComponentsAsSpan() = this.EntityManager.ScriptComponents.AsSpan()
    member this.GetScriptComponent(entityId: int) = &this.EntityManager.ScriptComponents.[entityId]

    // --- RigidBody 2D ---
    member this.GetRigidBodyComponentsAsSpan() = this.EntityManager.Rigidbody2DComponents.AsSpan()
    member this.GetRigidBodyComponent(entityId: int) = &this.EntityManager.Rigidbody2DComponents.[entityId]

    // --- Button 2D ---
    member this.GetButtonComponentsAsSpan() = this.EntityManager.Button2DComponents.AsSpan()
    member this.GetButtonComponent(entityId: int) = &this.EntityManager.Button2DComponents.[entityId]

    // --- Model 3D ---
    member this.GetModelsComponentsAsSpan() = this.EntityManager.ModelComponents3D.AsSpan()
    member this.GetModelComponent(entityId: int) = &this.EntityManager.ModelComponents3D.[entityId]

    // --- YTB Model 3D ---
    member this.GetYTBModelComponentsAsSpan() = this.EntityManager.YtbModelComponents.AsSpan()
    member this.GetYTBModelComponent(entityId: int) = &this.EntityManager.YtbModelComponents.[entityId]

    // --- Input ---
    member this.GetInputComponentsAsSpan() = this.EntityManager.InputComponents.AsSpan()
    member this.GetInputComponent(entityId: int) = &this.EntityManager.InputComponents.[entityId]

    // --- Font 2D ---
    member this.GetFontComponentsAsSpan() = this.EntityManager.Font2DComponents.AsSpan()
    member this.GetFontComponent(entityId: int) = &this.EntityManager.Font2DComponents.[entityId]

    // --- Animation 2D ---
    member this.GetAnimationComponentsAsSpan() = this.EntityManager.Animation2DComponents.AsSpan()
    member this.GetAnimationComponent(entityId: int) = &this.EntityManager.Animation2DComponents.[entityId]

    static member IsDefaultEntity(entity: byref<Yotsuba>) = entity.Equals(Unchecked.defaultof<Yotsuba>)
