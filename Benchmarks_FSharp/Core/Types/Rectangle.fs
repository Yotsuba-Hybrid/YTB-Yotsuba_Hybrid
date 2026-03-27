namespace Benchmarks.Core.Types

open System.Runtime.CompilerServices

[<Struct>]
type Rectangle =
    val mutable X: int
    val mutable Y: int
    val mutable Width: int
    val mutable Height: int
    new(x, y, width, height) = { X = x; Y = y; Width = width; Height = height }

    member this.Left = this.X
    member this.Right = this.X + this.Width
    member this.Top = this.Y
    member this.Bottom = this.Y + this.Height

    static member Empty = Rectangle(0, 0, 0, 0)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member this.Intersects(other: Rectangle) =
        this.Left < other.Right && this.Right > other.Left &&
        this.Top < other.Bottom && this.Bottom > other.Top

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member this.Contains(px: int, py: int) =
        px >= this.Left && px < this.Right && py >= this.Top && py < this.Bottom
