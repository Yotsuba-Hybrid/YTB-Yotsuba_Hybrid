namespace Benchmarks.Core.Components

open Benchmarks.Core.Types

[<Struct>]
type RigidBodyComponent2D =
    val mutable SPEED: float32
    val mutable TOP_SPEED: float32
    val mutable OffSetCollision: Vector2
    val mutable GameType: GameType
    val mutable Velocity: Vector3
    val mutable Mass: MassLevel
    val mutable Gravity: float32
    val mutable JumpForce: float32
    val mutable MaxFallSpeed: float32
    val mutable FastFallMultiplier: float32
    val mutable IsGrounded: bool
    val mutable IsJumping: bool
    val mutable IsFastFalling: bool
    val mutable FacingDirection: int

    new(gameType: GameType, mass: MassLevel) =
        { SPEED = 1.0f; TOP_SPEED = 3.0f; OffSetCollision = Vector2.Zero; GameType = gameType
          Velocity = Vector3.Zero; Mass = mass; Gravity = 0.5f; JumpForce = -12.0f
          MaxFallSpeed = 15.0f; FastFallMultiplier = 2.5f; IsGrounded = false
          IsJumping = false; IsFastFalling = false; FacingDirection = 1 }
