namespace Benchmarks.Core.Components

open Benchmarks.Core.Types

[<Struct>]
type TransformComponent =
    val mutable Size: Vector3
    val mutable Scale: float32
    val mutable Rotation: float32
    val mutable Position: Vector3
    val mutable SpriteEffects: SpriteEffects
    val mutable Color: Color

    new(position: Vector3, size: Vector3, scale: float32, spriteEffects: SpriteEffects, color: Color) =
        { Position = position; Size = size; Scale = scale; Rotation = 0f; SpriteEffects = spriteEffects; Color = color }

    new(position: Vector3, size: Vector3) =
        { Position = position; Size = size; Scale = 1f; Rotation = 0f; SpriteEffects = SpriteEffects.None; Color = Color.White }

    new(position: Vector3, size: Vector3, scale: float32) =
        { Position = position; Size = size; Scale = scale; Rotation = 0f; SpriteEffects = SpriteEffects.None; Color = Color.White }

    member this.SetPosition(x: float32, y: float32, z: float32) =
        this.Position <- Vector3(x, y, z)
