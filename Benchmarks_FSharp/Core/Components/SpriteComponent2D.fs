namespace Benchmarks.Core.Components

open Benchmarks.Core.Types

[<Struct>]
type SpriteComponent2D =
    val mutable TextureId: int
    val mutable SourceRectangle: Rectangle
    val mutable IsVisible: bool
    val mutable Is2_5D: bool

    new(textureId: int, sourceRectangle: Rectangle) =
        { TextureId = textureId; SourceRectangle = sourceRectangle; IsVisible = true; Is2_5D = false }
