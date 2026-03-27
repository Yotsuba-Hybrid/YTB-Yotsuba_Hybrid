namespace Benchmarks.Core.Types

open System.Runtime.CompilerServices

[<Struct>]
type Vector3 =
    val mutable X: float32
    val mutable Y: float32
    val mutable Z: float32
    new(x, y, z) = { X = x; Y = y; Z = z }

    static member Zero = Vector3(0f, 0f, 0f)
    static member One = Vector3(1f, 1f, 1f)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member (+)(a: Vector3, b: Vector3) = Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member (-)(a: Vector3, b: Vector3) = Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member (*)(a: Vector3, s: float32) = Vector3(a.X * s, a.Y * s, a.Z * s)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member op_Equality(a: Vector3, b: Vector3) = a.X = b.X && a.Y = b.Y && a.Z = b.Z

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member op_Inequality(a: Vector3, b: Vector3) = not (Vector3.op_Equality(a, b))

    override this.Equals(obj) =
        match obj with
        | :? Vector3 as v -> this.X = v.X && this.Y = v.Y && this.Z = v.Z
        | _ -> false

    override this.GetHashCode() = HashCode.Combine(this.X, this.Y, this.Z)
