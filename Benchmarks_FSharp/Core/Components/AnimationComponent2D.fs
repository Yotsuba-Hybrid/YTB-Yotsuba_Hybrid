namespace Benchmarks.Core.Components

open System.Collections.Generic
open Benchmarks.Core.Types

[<Struct>]
type AnimationData =
    val mutable CurrentFrame: int
    val mutable TotalFrames: int
    val mutable FrameTime: float32
    val mutable ElapsedTime: float32
    val mutable IsFinished: bool
    val mutable FinishedWasMarked: bool
    val mutable Loop: bool
    val mutable FrameWidth: int
    val mutable FrameHeight: int
    val mutable TextureId: int

    member this.Reset() =
        this.CurrentFrame <- 0
        this.ElapsedTime <- 0f
        this.IsFinished <- false
        this.FinishedWasMarked <- false

    member this.CurrentFrameRect(gameTime: GameTime) =
        this.ElapsedTime <- this.ElapsedTime + float32 gameTime.ElapsedGameTime.TotalSeconds
        if this.ElapsedTime >= this.FrameTime then
            this.ElapsedTime <- this.ElapsedTime - this.FrameTime
            if this.CurrentFrame < this.TotalFrames - 1 then
                this.CurrentFrame <- this.CurrentFrame + 1
            elif this.Loop then
                this.CurrentFrame <- 0
            else
                this.IsFinished <- true
        Rectangle(this.CurrentFrame * this.FrameWidth, 0, this.FrameWidth, this.FrameHeight)

[<Struct>]
type AnimationComponent2D =
    val mutable private Animations: Dictionary<AnimationType, AnimationData>
    val mutable CurrentAnimationType: struct (AnimationType * AnimationData)

    member this.AddAnimation(animType: AnimationType, animation: AnimationData) =
        if isNull this.Animations then
            this.Animations <- Dictionary<AnimationType, AnimationData>()
        this.Animations.[animType] <- animation

    member this.RemoveAnimation(animType: AnimationType) =
        if not (isNull this.Animations) then
            this.Animations.Remove(animType) |> ignore

    member this.GetAnimation(animType: AnimationType) =
        if not (isNull this.Animations) then
            match this.Animations.TryGetValue(animType) with
            | true, anim -> anim
            | _ -> Unchecked.defaultof<AnimationData>
        else
            Unchecked.defaultof<AnimationData>

    member this.ContainsAnimation(animType: AnimationType) =
        not (isNull this.Animations) && this.Animations.ContainsKey(animType)

    member this.ActivateAnimation(animType: AnimationType) =
        if not (isNull this.Animations) then
            match this.Animations.TryGetValue(animType) with
            | true, anim -> this.CurrentAnimationType <- struct (animType, anim)
            | _ -> ()
