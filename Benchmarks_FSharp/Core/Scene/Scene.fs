namespace Benchmarks.Core.Scene

open System
open Benchmarks.Core.Entity
open Benchmarks.Core.Systems
open Benchmarks.Core.Types

type Scene() =
    member val SceneName: string = null with get, set
    member val EntityManager = EntityManager() with get, set
    member val EventManager = EventManager.Instance with get, set
    member val SystemBuilder = SystemBuilder() with get, set

    member val internal FontSystem2D = FontSystem2D() with get, set
    member val private AnimationSystem2D = AnimationSystem2D() with get, set
    member val private ButtonSystem2D = ButtonSystem2D() with get, set
    member val private PhysicsSystem2D = PhysicsSystem2D() with get, set
    member val private RenderSystem2D = RenderSystem2D() with get, set
    member val private CameraSystem = CameraSystem() with get, set
    member val private InputSystem = InputSystem() with get, set
    member val ScriptSystem = ScriptSystem() with get, set
    member val private TilemapSystem = TileMapSystem2D() with get, set

    member this.Initialize() =
        this.AnimationSystem2D.InitializeSystem(this.EntityManager)
        this.ButtonSystem2D.InitializeSystem(this.EntityManager)
        this.PhysicsSystem2D.InitializeSystem(this.EntityManager)
        this.RenderSystem2D.InitializeSystem(this.EntityManager)
        this.CameraSystem.InitializeSystem(this.EntityManager)
        this.InputSystem.InitializeSystem(this.EntityManager)
        this.ScriptSystem.InitializeSystem(this.EntityManager)
        this.TilemapSystem.InitializeSystem(this.EntityManager)
        this.FontSystem2D.InitializeSystem(this.EntityManager)
        this.SystemBuilder.InitializeSystem(this.EntityManager)

        let entities = this.EntityManager.YotsubaEntities.AsSpan()
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            this.SystemBuilder.SharedEntityInitialize(&entity)
            this.ScriptSystem.SharedEntityInitialize(&entity)
            this.FontSystem2D.SharedEntityInitialize(&entity)

    member this.Update(gameTime: GameTime) =
        this.InputSystem.UpdateSystem(gameTime)

        if isNull this.EntityManager.YotsubaEntities || this.EntityManager.YotsubaEntities.Count = 0 then ()
        else
        this.PhysicsSystem2D.UpdateSystem(gameTime)
        this.ButtonSystem2D.UpdateSystem(gameTime)
        this.AnimationSystem2D.UpdateSystem(gameTime)
        this.CameraSystem.UpdateSystem(gameTime)
        this.SystemBuilder.UpdateSystem(gameTime)

        let entities = this.EntityManager.YotsubaEntities.AsSpan()
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            this.ScriptSystem.SharedEntityForEachUpdate(&entity, gameTime)
            this.SystemBuilder.SharedEntityForEachUpdate(&entity, gameTime)

    member this.Draw(gameTime: GameTime) =
        if not (isNull this.EntityManager.YotsubaEntities) && this.EntityManager.YotsubaEntities.Count > 0 then
            let cameraBounds = Rectangle(0, 0, 1920, 1080)
            this.TilemapSystem.UpdateSystem(gameTime, cameraBounds)
            this.RenderSystem2D.UpdateSystem(gameTime)
            this.FontSystem2D.DrawSystem(gameTime)
            this.ScriptSystem.DrawSystem3D(gameTime)
            this.SystemBuilder.Render3D(gameTime)
            this.ScriptSystem.DrawSystem2D(gameTime)
            this.SystemBuilder.Render2D(gameTime)

    member this.Clear() =
        this.ScriptSystem.Clear()
        this.SystemBuilder.Dispose()
        this.AnimationSystem2D.Dispose()
        this.ButtonSystem2D.Dispose()
        this.PhysicsSystem2D.Dispose()
        this.CameraSystem.Dispose()
        this.InputSystem.Dispose()
        this.FontSystem2D.Dispose()

    interface IDisposable with
        member this.Dispose() = this.Clear()
