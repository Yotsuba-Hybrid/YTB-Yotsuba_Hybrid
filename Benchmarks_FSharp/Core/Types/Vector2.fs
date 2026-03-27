namespace Benchmarks.Core.Types

open System.Runtime.CompilerServices

[<Struct>]
type Vector2 =
    val mutable X: float32
    val mutable Y: float32
    new(x, y) = { X = x; Y = y }

    static member Zero = Vector2(0f, 0f)
    static member One = Vector2(1f, 1f)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member (+)(a: Vector2, b: Vector2) = Vector2(a.X + b.X, a.Y + b.Y)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member (-)(a: Vector2, b: Vector2) = Vector2(a.X - b.X, a.Y - b.Y)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member (*)(a: Vector2, s: float32) = Vector2(a.X * s, a.Y * s)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member op_Equality(a: Vector2, b: Vector2) = a.X = b.X && a.Y = b.Y

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member op_Inequality(a: Vector2, b: Vector2) = not (Vector2.op_Equality(a, b))

    override this.Equals(obj) =
        match obj with
        | :? Vector2 as v -> this.X = v.X && this.Y = v.Y
        | _ -> false

    override this.GetHashCode() = HashCode.Combine(this.X, this.Y)
