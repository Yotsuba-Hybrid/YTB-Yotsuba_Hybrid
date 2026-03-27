namespace Benchmarks.Core.Systems

open Benchmarks.Core.Components
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Systems.Contract
open Benchmarks.Core.Types

type RenderSystem2D() =
    inherit IRenderSystem()

    member val DrawCallCount = 0 with get, set
    member val CulledCount = 0 with get, set
    let mutable eventManager: EventManager = Unchecked.defaultof<EventManager>

    override this.InitializeSystem(entities: EntityManager) =
        eventManager <- EventManager.Instance
        this.EntityManager <- entities

    override this.UpdateSystem(gameTime: GameTime) =
        this.DrawCallCount <- 0
        this.CulledCount <- 0

        let entityManager = this.EntityManager
        if isNull entityManager then ()
        else

        let cameraEntityId = entityManager.CameraEntityId
        let mutable viewMatrix = Matrix.Identity
        let mutable cameraWorldBounds = Rectangle.Empty

        let transformComponents = this.GetTransformComponentsAsSpan()

        if cameraEntityId >= 0 then
            let camTransform = &transformComponents.[cameraEntityId]
            let mutable currentZoom = entityManager.CameraZoom
            if currentZoom <= 0.001f then currentZoom <- 0.001f

            let offset = Vector2.Zero
            let viewportWidth = 1920f
            let viewportHeight = 1080f
            let screenCenter = Vector2(viewportWidth / 2f, viewportHeight / 2f)

            let camPos = Vector2(camTransform.Position.X, camTransform.Position.Y) + offset
            viewMatrix <- Matrix.CreateTranslation(-camPos.X, -camPos.Y, 0f)
                          * Matrix.CreateScale(currentZoom)
                          * Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0f)

            let visibleWorldWidth = viewportWidth / currentZoom
            let visibleWorldHeight = viewportHeight / currentZoom
            let cameraCenter = Vector2(camTransform.Position.X, camTransform.Position.Y) + offset
            let camLeft = cameraCenter.X - (visibleWorldWidth / 2f)
            let camTop = cameraCenter.Y - (visibleWorldHeight / 2f)
            let padding = 100
            cameraWorldBounds <- Rectangle(
                int (camLeft - float32 padding),
                int (camTop - float32 padding),
                int (visibleWorldWidth + float32 (padding * 2)),
                int (visibleWorldHeight + float32 (padding * 2)))
        else
            cameraWorldBounds <- Rectangle(0, 0, 1920, 1080)

        let spriteComponent2Ds = entityManager.Sprite2DComponents.AsSpan()
        let spanEntities = entityManager.YotsubaEntities.AsSpan()
        let shaderComponents = entityManager.ShaderComponents.AsSpan()

        // --- PASS 1: UI Elements ---
        for i = 0 to spanEntities.Length - 1 do
            let entity = &spanEntities.[i]
            if entity.HasComponent(YTBComponent.YTBUIElement) then
                let sprite = &spriteComponent2Ds.[entity.Id]
                if sprite.IsVisible && not sprite.Is2_5D then
                    let transform = &transformComponents.[entity.Id]
                    let _destinationRect = Rectangle(
                        int transform.Position.X, int transform.Position.Y,
                        int transform.Size.X, int transform.Size.Y)
                    this.DrawCallCount <- this.DrawCallCount + 1

        // --- PASS 2: World Sprites ---
        for i = 0 to spanEntities.Length - 1 do
            let entity = &spanEntities.[i]
            if (entity.HasComponent(YTBComponent.Sprite) && entity.HasComponent(YTBComponent.Transform))
               && not (entity.HasComponent(YTBComponent.Button2D)) then

                if entity.HasComponent(YTBComponent.Shader) then
                    let shaderComp = shaderComponents.[entity.Id]
                    if shaderComp.IsActive && shaderComp.ShaderId <> 0 then
                        () // continue - skip shader entities in this pass
                    else
                        let sprite = &spriteComponent2Ds.[entity.Id]
                        if sprite.IsVisible && not sprite.Is2_5D then
                            let transform = &transformComponents.[entity.Id]
                            let entWidth = transform.Size.X * transform.Scale
                            let entHeight = transform.Size.Y * transform.Scale
                            let entityBounds = Rectangle(
                                int (transform.Position.X - entWidth / 2f),
                                int (transform.Position.Y - entHeight / 2f),
                                int entWidth, int entHeight)
                            if not (cameraWorldBounds.Intersects(entityBounds)) then
                                this.CulledCount <- this.CulledCount + 1
                            else
                                this.DrawCallCount <- this.DrawCallCount + 1
                else
                    let sprite = &spriteComponent2Ds.[entity.Id]
                    if sprite.IsVisible && not sprite.Is2_5D then
                        let transform = &transformComponents.[entity.Id]
                        let entWidth = transform.Size.X * transform.Scale
                        let entHeight = transform.Size.Y * transform.Scale
                        let entityBounds = Rectangle(
                            int (transform.Position.X - entWidth / 2f),
                            int (transform.Position.Y - entHeight / 2f),
                            int entWidth, int entHeight)
                        if not (cameraWorldBounds.Intersects(entityBounds)) then
                            this.CulledCount <- this.CulledCount + 1
                        else
                            this.DrawCallCount <- this.DrawCallCount + 1

        // --- PASS 3: Shader Entities ---
        for i = 0 to spanEntities.Length - 1 do
            let entity = &spanEntities.[i]
            if (entity.HasComponent(YTBComponent.Sprite) && entity.HasComponent(YTBComponent.Transform))
               && not (entity.HasComponent(YTBComponent.Button2D))
               && entity.HasComponent(YTBComponent.Shader) then
                let shaderComp = &shaderComponents.[entity.Id]
                if shaderComp.IsActive && shaderComp.ShaderId <> 0 then
                    let sprite = &spriteComponent2Ds.[entity.Id]
                    if sprite.IsVisible && not sprite.Is2_5D then
                        let transform = &transformComponents.[entity.Id]
                        let entWidth = transform.Size.X * transform.Scale
                        let entHeight = transform.Size.Y * transform.Scale
                        let entityBounds = Rectangle(
                            int (transform.Position.X - entWidth / 2f),
                            int (transform.Position.Y - entHeight / 2f),
                            int entWidth, int entHeight)
                        if not (cameraWorldBounds.Intersects(entityBounds)) then
                            this.CulledCount <- this.CulledCount + 1
                        else
                            this.DrawCallCount <- this.DrawCallCount + 1

        // --- PASS 4: Buttons/HUD ---
        for i = 0 to spanEntities.Length - 1 do
            let entity = &spanEntities.[i]
            if (entity.HasComponent(YTBComponent.Sprite) && entity.HasComponent(YTBComponent.Transform))
               && entity.HasComponent(YTBComponent.Button2D)
               && not (entity.HasComponent(YTBComponent.YTBUIElement)) then
                let sprite = &spriteComponent2Ds.[entity.Id]
                if sprite.IsVisible && not sprite.Is2_5D then
                    let _transform = &transformComponents.[entity.Id]
                    this.DrawCallCount <- this.DrawCallCount + 1

    override _.Dispose() = ()
