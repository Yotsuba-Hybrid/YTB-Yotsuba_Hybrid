namespace Benchmarks.Core.Types

type SpriteEffects =
    | None = 0
    | FlipHorizontally = 1
    | FlipVertically = 2

type GameType =
    | TopDown = 0
    | Platform = 1

type MassLevel =
    | Collision = 1
    | NoCollision = 2
    | Slow = 3

type AnimationType =
    | None = 0
    | Idle = 1
    | Walk = 2
    | Run = 3
    | Jump = 4
    | Crouch = 5
    | Attack = 6
    | Hurt = 7
    | Die = 8
