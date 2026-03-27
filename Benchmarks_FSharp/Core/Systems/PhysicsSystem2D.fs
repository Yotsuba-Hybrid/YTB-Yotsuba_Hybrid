namespace Benchmarks.Core.Systems

open System
open Benchmarks.Core.Components
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Systems.Contract
open Benchmarks.Core.Types

type PhysicsSystem2D() =
    inherit ISystem()

    let mutable eventManager: EventManager = Unchecked.defaultof<EventManager>

    override this.InitializeSystem(entities: EntityManager) =
        eventManager <- EventManager.Instance
        this.EntityManager <- entities

    override this.UpdateSystem(gameTime: GameTime) =
        if isNull this.EntityManager then ()
        else
            let entities = this.GetEntitiesAsSpan()
            let transformComponents = this.GetTransformComponentsAsSpan()
            let rigidbodyComponents = this.GetRigidBodyComponentsAsSpan()
            PhysicsSystem2D.ApplyPlatformPhysics(entities, rigidbodyComponents, gameTime)
            PhysicsSystem2D.MoveEntities(this.EntityManager, entities, transformComponents, rigidbodyComponents, gameTime)

    static member private ApplyPlatformPhysics(entities: Span<Yotsuba>, rigidbodyComponents: Span<RigidBodyComponent2D>, gameTime: GameTime) =
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            if entity.HasComponent(YTBComponent.Rigibody) then
                let rigidBody = &rigidbodyComponents.[entity.Id]
                if rigidBody.GameType = GameType.Platform then
                    let mutable newVelocityY = rigidBody.Velocity.Y + rigidBody.Gravity
                    if rigidBody.IsFastFalling && rigidBody.Velocity.Y >= 0f then
                        newVelocityY <- rigidBody.Velocity.Y + (rigidBody.Gravity * rigidBody.FastFallMultiplier)
                    newVelocityY <- Math.Min(newVelocityY, rigidBody.MaxFallSpeed)
                    rigidBody.Velocity <- Vector3(rigidBody.Velocity.X, newVelocityY, rigidBody.Velocity.Z)

    static member private MoveEntities(entityManager: EntityManager, entities: Span<Yotsuba>, transformComponents: Span<TransformComponent>, rigibodyComponents: Span<RigidBodyComponent2D>, gameTime: GameTime) =
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            if entity.HasComponent(YTBComponent.Transform) && entity.HasComponent(YTBComponent.Rigibody) then
                let mutable collisionBottom = false
                let mutable collisionTop = false
                let mutable collisionLeft = false
                let mutable collisionRight = false

                let rigidBody = &rigibodyComponents.[entity.Id]
                let transform = &transformComponents.[entity.Id]
                let wasGrounded = rigidBody.IsGrounded

                let nextPosition = Vector2(transform.Position.X, transform.Position.Y)
                                   + Vector2(rigidBody.Velocity.X, rigidBody.Velocity.Y)

                let entityRect = Rectangle(
                    int (nextPosition.X + rigidBody.OffSetCollision.X),
                    int (nextPosition.Y + rigidBody.OffSetCollision.Y),
                    int (transform.Size.X * transform.Scale),
                    int (transform.Size.Y * transform.Scale))

                let sizeZero = Vector3.op_Equality(transform.Size, Vector3.Zero)

                for j = 0 to entities.Length - 1 do
                    let otherEntity = &entities.[j]
                    if otherEntity.Id <> entity.Id &&
                       otherEntity.HasComponent(YTBComponent.Transform) &&
                       otherEntity.HasComponent(YTBComponent.Rigibody) then
                        let otherRigidBody = &rigibodyComponents.[otherEntity.Id]
                        let otherTransform = &transformComponents.[otherEntity.Id]

                        if otherEntity.HasComponent(YTBComponent.TileMap) then
                            PhysicsSystem2D.CheckTileMapCollision(
                                entityManager, &entity, &otherEntity, &rigidBody, &otherRigidBody,
                                &transform, &otherTransform, entityRect, sizeZero, gameTime,
                                &collisionBottom, &collisionTop, &collisionLeft, &collisionRight)
                        else
                            PhysicsSystem2D.CheckEntityCollision(
                                &entity, &otherEntity, &rigidBody, &otherRigidBody,
                                &transform, &otherTransform, entityRect, sizeZero, gameTime,
                                &collisionBottom, &collisionTop, &collisionLeft, &collisionRight)

                PhysicsSystem2D.ApplyMovement(
                    entity.Id, &rigidBody, &transform, collisionBottom, collisionTop,
                    collisionLeft, collisionRight, wasGrounded, gameTime)

    static member private CheckTileMapCollision(
        entityManager: EntityManager,
        entity: byref<Yotsuba>, tilemapEntity: byref<Yotsuba>,
        rigidBody: byref<RigidBodyComponent2D>, otherRigidBody: byref<RigidBodyComponent2D>,
        transform: byref<TransformComponent>, otherTransform: byref<TransformComponent>,
        entityRect: Rectangle, sizeZero: bool, gameTime: GameTime,
        collisionBottom: byref<bool>, collisionTop: byref<bool>,
        collisionLeft: byref<bool>, collisionRight: byref<bool>) =

        if rigidBody.Mass = MassLevel.NoCollision || otherRigidBody.Mass = MassLevel.NoCollision then ()
        else
            let tilemap = &entityManager.TileMapComponent2Ds.[tilemapEntity.Id]
            let originOffsetX = otherTransform.Size.X * 0.5f * otherTransform.Scale
            let originOffsetY = otherTransform.Size.Y * 0.5f * otherTransform.Scale

            let layers = tilemap.TileLayers.AsSpan()
            for li = 0 to layers.Length - 1 do
                let layer = &layers.[li]
                let isCollisionLayer =
                    layer.Name.Contains("Collision", StringComparison.OrdinalIgnoreCase) ||
                    layer.Name.Contains("Collider", StringComparison.OrdinalIgnoreCase)

                for i = 0 to layer.Data.Length - 1 do
                    let gid = layer.Data.[i]
                    if gid <> 0 then
                        let tileX = i % tilemap.Width
                        let tileY = i / tilemap.Width
                        let worldX = otherTransform.Scale * float32 (tileX * tilemap.TileWidth) + otherTransform.Position.X - originOffsetX
                        let worldY = otherTransform.Scale * float32 (tileY * tilemap.TileHeight) + otherTransform.Position.Y - originOffsetY

                        let mutable nativeRects: System.Collections.Generic.List<Rectangle> = null
                        let hasNativeCollision = tilemap.Collisions.TryGetValue(gid, &nativeRects)

                        if isCollisionLayer && not hasNativeCollision then
                            let tileRect = Rectangle(
                                int worldX, int worldY,
                                int (float32 tilemap.TileWidth * otherTransform.Scale),
                                int (float32 tilemap.TileHeight * otherTransform.Scale))
                            if entityRect.Intersects(tileRect) && not sizeZero then
                                PhysicsSystem2D.DetermineCollisionDirection(
                                    entityRect, tileRect, &rigidBody,
                                    &collisionBottom, &collisionTop, &collisionLeft, &collisionRight)

                        if hasNativeCollision then
                            for nativeRect in nativeRects do
                                let collisionRect = Rectangle(
                                    int (worldX + float32 nativeRect.X * otherTransform.Scale),
                                    int (worldY + float32 nativeRect.Y * otherTransform.Scale),
                                    int (float32 nativeRect.Width * otherTransform.Scale),
                                    int (float32 nativeRect.Height * otherTransform.Scale))
                                if entityRect.Intersects(collisionRect) && not sizeZero then
                                    PhysicsSystem2D.DetermineCollisionDirection(
                                        entityRect, collisionRect, &rigidBody,
                                        &collisionBottom, &collisionTop, &collisionLeft, &collisionRight)

    static member private CheckEntityCollision(
        entity: byref<Yotsuba>, otherEntity: byref<Yotsuba>,
        rigidBody: byref<RigidBodyComponent2D>, otherRigidBody: byref<RigidBodyComponent2D>,
        transform: byref<TransformComponent>, otherTransform: byref<TransformComponent>,
        entityRect: Rectangle, sizeZero: bool, gameTime: GameTime,
        collisionBottom: byref<bool>, collisionTop: byref<bool>,
        collisionLeft: byref<bool>, collisionRight: byref<bool>) =

        if rigidBody.Mass = MassLevel.NoCollision || otherRigidBody.Mass = MassLevel.NoCollision then ()
        elif Vector3.op_Equality(otherTransform.Size, Vector3.Zero) then ()
        else
            let otherRect = Rectangle(
                int (otherTransform.Position.X + otherRigidBody.OffSetCollision.X),
                int (otherTransform.Position.Y + otherRigidBody.OffSetCollision.Y),
                int (otherTransform.Size.X * otherTransform.Scale),
                int (otherTransform.Size.Y * otherTransform.Scale))
            if entityRect.Intersects(otherRect) && not sizeZero then
                PhysicsSystem2D.DetermineCollisionDirection(
                    entityRect, otherRect, &rigidBody,
                    &collisionBottom, &collisionTop, &collisionLeft, &collisionRight)

    static member private DetermineCollisionDirection(
        entityRect: Rectangle, obstacleRect: Rectangle,
        rigidBody: byref<RigidBodyComponent2D>,
        collisionBottom: byref<bool>, collisionTop: byref<bool>,
        collisionLeft: byref<bool>, collisionRight: byref<bool>) =

        let overlapLeft = entityRect.Right - obstacleRect.Left
        let overlapRight = obstacleRect.Right - entityRect.Left
        let overlapTop = entityRect.Bottom - obstacleRect.Top
        let overlapBottom = obstacleRect.Bottom - entityRect.Top

        let minOverlapX = Math.Min(overlapLeft, overlapRight)
        let minOverlapY = Math.Min(overlapTop, overlapBottom)

        if minOverlapY < minOverlapX then
            if overlapTop < overlapBottom && rigidBody.Velocity.Y > 0f then
                collisionBottom <- true
            elif overlapBottom < overlapTop && rigidBody.Velocity.Y < 0f then
                collisionTop <- true
        else
            if overlapLeft < overlapRight && rigidBody.Velocity.X > 0f then
                collisionRight <- true
            elif overlapRight < overlapLeft && rigidBody.Velocity.X < 0f then
                collisionLeft <- true

    static member private ApplyMovement(
        entityId: int,
        rigidBody: byref<RigidBodyComponent2D>,
        transform: byref<TransformComponent>,
        collisionBottom: bool, collisionTop: bool,
        collisionLeft: bool, collisionRight: bool,
        wasGrounded: bool, gameTime: GameTime) =

        let mutable finalVelocity = Vector2(rigidBody.Velocity.X, rigidBody.Velocity.Y)

        if collisionBottom then
            finalVelocity.Y <- 0f
            rigidBody.IsGrounded <- true
            rigidBody.IsJumping <- false
            rigidBody.IsFastFalling <- false
        elif collisionTop then
            finalVelocity.Y <- 0f
        elif rigidBody.GameType = GameType.Platform then
            if wasGrounded then
                rigidBody.IsGrounded <- false

        if collisionLeft || collisionRight then
            finalVelocity.X <- 0f

        if not (collisionBottom || collisionTop) || not (collisionLeft || collisionRight) then
            let moveX = if collisionLeft || collisionRight then 0f else finalVelocity.X
            let moveY = if collisionBottom || collisionTop then 0f else finalVelocity.Y
            transform.Position <- transform.Position + Vector3(moveX, moveY, 0f)

        rigidBody.Velocity <- Vector3(finalVelocity.X, finalVelocity.Y, rigidBody.Velocity.Z)

    override _.SharedEntityForEachUpdate(_, _) = ()
    override _.SharedEntityInitialize(_) = ()
    override _.Dispose() = ()
