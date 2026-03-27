namespace Benchmarks.Core.Systems.Contract

open Benchmarks.Core.Types

[<AbstractClass>]
type IRenderSystem() =
    inherit ISystem()

    abstract Render2D: GameTime -> unit
    default _.Render2D(_) = ()

    abstract Render3D: GameTime -> unit
    default _.Render3D(_) = ()
