namespace Benchmarks.Core.Systems

open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Systems.Contract
open Benchmarks.Core.Types

type CameraSystem() =
    inherit ISystem()

    member val ViewMatrix = Matrix.Identity with get, set
    member val ProjectionMatrix = Matrix.Identity with get, set

    override this.InitializeSystem(entities: EntityManager) =
        this.EntityManager <- entities

    override this.UpdateSystem(gameTime: GameTime) =
        if isNull this.EntityManager then ()
        else
        let cameraEntityId = this.EntityManager.CameraEntityId
        if cameraEntityId < 0 then ()
        else
        let transforms = this.GetTransformComponentsAsSpan()
        let camTransform = &transforms.[cameraEntityId]
        let mutable zoom = this.EntityManager.CameraZoom
        if zoom <= 0.001f then zoom <- 0.001f

        let viewportWidth = 1920f
        let viewportHeight = 1080f
        let screenCenter = Vector2(viewportWidth / 2f, viewportHeight / 2f)
        let camPos = Vector2(camTransform.Position.X, camTransform.Position.Y)

        this.ViewMatrix <- Matrix.CreateTranslation(-camPos.X, -camPos.Y, 0f)
                           * Matrix.CreateRotationZ(0f)
                           * Matrix.CreateScale(zoom)
                           * Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0f)

    override _.SharedEntityForEachUpdate(_, _) = ()
    override _.SharedEntityInitialize(_) = ()
    override _.Dispose() = ()
