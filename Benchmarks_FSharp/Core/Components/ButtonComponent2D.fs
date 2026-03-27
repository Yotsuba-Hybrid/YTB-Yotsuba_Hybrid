namespace Benchmarks.Core.Components

open System
open Benchmarks.Core.Types

[<Struct>]
type ButtonComponent2D =
    val mutable IsActive: bool
    val mutable EffectiveArea: Rectangle
    val mutable Action: Action
