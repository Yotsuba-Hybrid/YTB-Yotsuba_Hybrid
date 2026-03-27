namespace Benchmarks.Core.Systems

open Benchmarks.Core.Components
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Systems.Contract
open Benchmarks.Core.Types

type AnimationSystem2D() =
    inherit ISystem()

    let mutable eventManager: EventManager = Unchecked.defaultof<EventManager>

    override this.InitializeSystem(entities: EntityManager) =
        eventManager <- EventManager.Instance
        this.EntityManager <- entities

    override this.UpdateSystem(gameTime: GameTime) =
        let animationsComponents = this.GetAnimationComponentsAsSpan()
        let spriteComponents = this.GetSpriteComponentsAsSpan()
        if isNull this.EntityManager then ()
        else
        let entities = this.GetEntitiesAsSpan()
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            if entity.HasComponent(YTBComponent.Animation) && entity.HasComponent(YTBComponent.Sprite) then
                let animationComponent = &animationsComponents.[entity.Id]
                let spriteComponent = &spriteComponents.[entity.Id]
                let struct (_, animData) = animationComponent.CurrentAnimationType
                if animData.IsFinished then
                    if not animData.FinishedWasMarked then
                        let mutable current = animationComponent.CurrentAnimationType
                        let struct (at, ad) = current
                        let mutable ad2 = ad
                        ad2.FinishedWasMarked <- true
                        animationComponent.CurrentAnimationType <- struct (at, ad2)
                else
                    let mutable current = animationComponent.CurrentAnimationType
                    let struct (at, ad) = current
                    let mutable ad2 = ad
                    let region = ad2.CurrentFrameRect(gameTime)
                    spriteComponent.TextureId <- ad2.TextureId
                    spriteComponent.SourceRectangle <- region
                    animationComponent.CurrentAnimationType <- struct (at, ad2)

    override _.SharedEntityForEachUpdate(_, _) = ()
    override _.SharedEntityInitialize(_) = ()
    override _.Dispose() = ()
