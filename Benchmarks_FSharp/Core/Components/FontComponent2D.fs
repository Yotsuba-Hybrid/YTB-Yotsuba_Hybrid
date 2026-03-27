namespace Benchmarks.Core.Components

open Benchmarks.Core.Types

[<Struct>]
type FontComponent2D =
    val mutable Text: string
    val mutable FontId: int
    val mutable Color: Color
    val mutable FontScale: float32
    val mutable IsVisible: bool
