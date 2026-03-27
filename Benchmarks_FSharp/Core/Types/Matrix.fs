namespace Benchmarks.Core.Types

open System
open System.Runtime.CompilerServices

[<Struct>]
type Matrix =
    val mutable M11: float32
    val mutable M12: float32
    val mutable M13: float32
    val mutable M14: float32
    val mutable M21: float32
    val mutable M22: float32
    val mutable M23: float32
    val mutable M24: float32
    val mutable M31: float32
    val mutable M32: float32
    val mutable M33: float32
    val mutable M34: float32
    val mutable M41: float32
    val mutable M42: float32
    val mutable M43: float32
    val mutable M44: float32

    static member Identity =
        let mutable m = Unchecked.defaultof<Matrix>
        m.M11 <- 1f
        m.M22 <- 1f
        m.M33 <- 1f
        m.M44 <- 1f
        m

    static member CreateTranslation(x: float32, y: float32, z: float32) =
        let mutable m = Matrix.Identity
        m.M41 <- x
        m.M42 <- y
        m.M43 <- z
        m

    static member CreateScale(scale: float32) =
        let mutable m = Matrix.Identity
        m.M11 <- scale
        m.M22 <- scale
        m.M33 <- scale
        m

    static member CreateRotationZ(radians: float32) =
        let cos = MathF.Cos(radians)
        let sin = MathF.Sin(radians)
        let mutable m = Matrix.Identity
        m.M11 <- cos
        m.M12 <- sin
        m.M21 <- -sin
        m.M22 <- cos
        m

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member (*)(a: Matrix, b: Matrix) =
        let mutable r = Unchecked.defaultof<Matrix>
        r.M11 <- a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31 + a.M14 * b.M41
        r.M12 <- a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32 + a.M14 * b.M42
        r.M13 <- a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33 + a.M14 * b.M43
        r.M14 <- a.M11 * b.M14 + a.M12 * b.M24 + a.M13 * b.M34 + a.M14 * b.M44
        r.M21 <- a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31 + a.M24 * b.M41
        r.M22 <- a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32 + a.M24 * b.M42
        r.M23 <- a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33 + a.M24 * b.M43
        r.M24 <- a.M21 * b.M14 + a.M22 * b.M24 + a.M23 * b.M34 + a.M24 * b.M44
        r.M31 <- a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31 + a.M34 * b.M41
        r.M32 <- a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32 + a.M34 * b.M42
        r.M33 <- a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33 + a.M34 * b.M43
        r.M34 <- a.M31 * b.M14 + a.M32 * b.M24 + a.M33 * b.M34 + a.M34 * b.M44
        r.M41 <- a.M41 * b.M11 + a.M42 * b.M21 + a.M43 * b.M31 + a.M44 * b.M41
        r.M42 <- a.M41 * b.M12 + a.M42 * b.M22 + a.M43 * b.M32 + a.M44 * b.M42
        r.M43 <- a.M41 * b.M13 + a.M42 * b.M23 + a.M43 * b.M33 + a.M44 * b.M43
        r.M44 <- a.M41 * b.M14 + a.M42 * b.M24 + a.M43 * b.M34 + a.M44 * b.M44
        r
