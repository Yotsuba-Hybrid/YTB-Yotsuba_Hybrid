namespace Benchmarks.Core.Types

open System

type GameTime(totalGameTime: TimeSpan, elapsedGameTime: TimeSpan) =
    member val TotalGameTime = totalGameTime with get, set
    member val ElapsedGameTime = elapsedGameTime with get, set
    new() = GameTime(TimeSpan.Zero, TimeSpan.Zero)
