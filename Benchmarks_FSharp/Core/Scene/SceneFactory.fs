namespace Benchmarks.Core.Scene

open System
open System.Collections.Generic
open Benchmarks.Core.Collections
open Benchmarks.Core.Components
open Benchmarks.Core.Entity
open Benchmarks.Core.Types

module SceneFactory =

    let private populateEntities (em: EntityManager) (entityCount: int) =
        let rng = Random(42)

        let worldEntities = int (float entityCount * 0.70)
        let npcEntities = int (float entityCount * 0.10)
        let uiEntities = int (float entityCount * 0.10)
        let shaderEntities = int (float entityCount * 0.05)
        let playerEntities = entityCount - worldEntities - npcEntities - uiEntities - shaderEntities

        let mutable id = 0

        // 70% - World entities: Transform + Sprite + RigidBody
        for _ = 0 to worldEntities - 1 do
            let mutable entity = Yotsuba(id)
            entity.Name <- $"World_{id}"
            em.AddEntity(&entity)

            em.AddTransformComponent(entity, TransformComponent(
                Vector3(float32 (rng.Next(-1000, 1000)), float32 (rng.Next(-1000, 1000)), rng.NextSingle()),
                Vector3(float32 (rng.Next(1, 4) * 32), float32 (rng.Next(1, 4) * 32), 0f),
                rng.NextSingle() * 2f + 0.5f,
                SpriteEffects.None,
                Color.White))

            em.AddSpriteComponent(entity, SpriteComponent2D(
                rng.Next(1, 100),
                Rectangle(0, 0, 32, 32)))

            let gameType = if rng.Next(2) = 0 then GameType.TopDown else GameType.Platform
            let mutable rb = RigidBodyComponent2D(gameType, MassLevel.Collision)
            rb.Velocity <- Vector3(rng.NextSingle() * 2f - 1f, rng.NextSingle() * 2f - 1f, 0f)
            em.AddRigidbodyComponent(entity, rb)

            id <- id + 1

        // 10% - NPC entities: Transform + Sprite + Animation + RigidBody
        for _ = 0 to npcEntities - 1 do
            let mutable entity = Yotsuba(id)
            entity.Name <- $"NPC_{id}"
            em.AddEntity(&entity)

            em.AddTransformComponent(entity, TransformComponent(
                Vector3(float32 (rng.Next(-1000, 1000)), float32 (rng.Next(-1000, 1000)), rng.NextSingle()),
                Vector3(64f, 64f, 0f),
                1f, SpriteEffects.None, Color.White))

            em.AddSpriteComponent(entity, SpriteComponent2D(
                rng.Next(1, 100),
                Rectangle(0, 0, 64, 64)))

            let mutable animComp = AnimationComponent2D()
            let mutable idleAnim = Unchecked.defaultof<AnimationData>
            idleAnim.CurrentFrame <- 0
            idleAnim.TotalFrames <- 8
            idleAnim.FrameTime <- 0.1f
            idleAnim.FrameWidth <- 64
            idleAnim.FrameHeight <- 64
            idleAnim.TextureId <- rng.Next(1, 100)
            idleAnim.Loop <- true
            animComp.AddAnimation(AnimationType.Idle, idleAnim)

            let mutable walkAnim = Unchecked.defaultof<AnimationData>
            walkAnim.CurrentFrame <- 0
            walkAnim.TotalFrames <- 6
            walkAnim.FrameTime <- 0.12f
            walkAnim.FrameWidth <- 64
            walkAnim.FrameHeight <- 64
            walkAnim.TextureId <- rng.Next(1, 100)
            walkAnim.Loop <- true
            animComp.AddAnimation(AnimationType.Walk, walkAnim)
            animComp.ActivateAnimation(AnimationType.Idle)

            em.AddAnimationComponent(entity, animComp)

            let mutable rb = RigidBodyComponent2D(GameType.TopDown, MassLevel.Collision)
            rb.Velocity <- Vector3(rng.NextSingle() - 0.5f, rng.NextSingle() - 0.5f, 0f)
            em.AddRigidbodyComponent(entity, rb)

            id <- id + 1

        // 10% - UI entities: Transform + Sprite + Button2D
        for _ = 0 to uiEntities - 1 do
            let mutable entity = Yotsuba(id)
            entity.Name <- $"UI_{id}"
            em.AddEntity(&entity)

            let bx = rng.Next(0, 1800)
            let by = rng.Next(0, 1000)
            let bw = rng.Next(50, 200)
            let bh = rng.Next(30, 80)

            em.AddTransformComponent(entity, TransformComponent(
                Vector3(float32 bx, float32 by, 0.9f),
                Vector3(float32 bw, float32 bh, 0f),
                1f, SpriteEffects.None, Color.White))

            em.AddSpriteComponent(entity, SpriteComponent2D(
                rng.Next(1, 100),
                Rectangle(0, 0, bw, bh)))

            let mutable button = Unchecked.defaultof<ButtonComponent2D>
            button.IsActive <- true
            button.EffectiveArea <- Rectangle(bx, by, bw, bh)
            button.Action <- Action(fun () -> ignore 0)
            em.AddButtonComponent2D(entity, button)

            id <- id + 1

        // 5% - Shader entities: Transform + Sprite + Shader
        for _ = 0 to shaderEntities - 1 do
            let mutable entity = Yotsuba(id)
            entity.Name <- $"Shader_{id}"
            em.AddEntity(&entity)

            em.AddTransformComponent(entity, TransformComponent(
                Vector3(float32 (rng.Next(-1000, 1000)), float32 (rng.Next(-1000, 1000)), rng.NextSingle()),
                Vector3(128f, 128f, 0f),
                1f, SpriteEffects.None, Color.White))

            em.AddSpriteComponent(entity, SpriteComponent2D(
                rng.Next(1, 100),
                Rectangle(0, 0, 128, 128)))

            em.AddShaderComponent2D(entity, ShaderComponent(rng.Next(1, 10), true))

            id <- id + 1

        // 5% - Player entities: Transform + Sprite + Input + RigidBody
        for i = 0 to playerEntities - 1 do
            let mutable entity = Yotsuba(id)
            entity.Name <- $"Player_{id}"
            em.AddEntity(&entity)

            em.AddTransformComponent(entity, TransformComponent(
                Vector3(float32 (rng.Next(-500, 500)), float32 (rng.Next(-500, 500)), 0.5f),
                Vector3(48f, 48f, 0f),
                1f, SpriteEffects.None, Color.White))

            em.AddSpriteComponent(entity, SpriteComponent2D(
                rng.Next(1, 100),
                Rectangle(0, 0, 48, 48)))

            let mutable inputComp = Unchecked.defaultof<InputComponent>
            inputComp.KeyBoard <- Dictionary<ActionEntityInput, int>()
            inputComp.KeyBoard.Add(ActionEntityInput.MoveUp, 0)
            inputComp.KeyBoard.Add(ActionEntityInput.MoveDown, 1)
            inputComp.KeyBoard.Add(ActionEntityInput.MoveLeft, 2)
            inputComp.KeyBoard.Add(ActionEntityInput.MoveRight, 3)
            inputComp.KeyBoard.Add(ActionEntityInput.Jump, 4)
            inputComp.AddInput(InputInUse.HasKeyboard)
            em.AddInputComponent(entity, inputComp)

            let mutable rb = RigidBodyComponent2D(GameType.Platform, MassLevel.Collision)
            rb.Velocity <- Vector3(0f, 0f, 0f)
            rb.SPEED <- 2.0f
            rb.TOP_SPEED <- 5.0f
            em.AddRigidbodyComponent(entity, rb)

            if i = 0 then
                em.CameraEntityId <- entity.Id
                em.CameraZoom <- 1.0f

            id <- id + 1

    let CreateTestScene(entityCount: int) =
        EventManager.Reset()
        let scene = Scene()
        populateEntities scene.EntityManager entityCount
        scene.Initialize()
        scene

    let CreateTestEntityManager(entityCount: int) =
        let entityManager = EntityManager()
        populateEntities entityManager entityCount
        entityManager
