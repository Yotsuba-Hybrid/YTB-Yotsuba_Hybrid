namespace Benchmarks.Core.Types

[<Struct>]
type Color =
    val mutable R: byte
    val mutable G: byte
    val mutable B: byte
    val mutable A: byte
    new(r, g, b, a) = { R = r; G = g; B = b; A = a }

    static member White = Color(255uy, 255uy, 255uy, 255uy)
    static member Black = Color(0uy, 0uy, 0uy, 255uy)
    static member Red = Color(255uy, 0uy, 0uy, 255uy)
    static member Green = Color(0uy, 255uy, 0uy, 255uy)
    static member Blue = Color(0uy, 0uy, 255uy, 255uy)
