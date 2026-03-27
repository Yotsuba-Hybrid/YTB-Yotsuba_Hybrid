namespace Benchmarks.Core.Components

open System.Collections.Generic
open Benchmarks.Core.Collections
open Benchmarks.Core.Types

[<Struct>]
type TileLayer =
    val mutable Name: string
    val mutable Data: int array

[<Struct>]
type TileMapComponent2D =
    val mutable Width: int
    val mutable Height: int
    val mutable TileWidth: int
    val mutable TileHeight: int
    val mutable TileLayers: YTB<TileLayer>
    val mutable Collisions: Dictionary<int, List<Rectangle>>
