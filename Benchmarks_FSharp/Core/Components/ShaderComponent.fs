namespace Benchmarks.Core.Components

[<Struct>]
type ShaderComponent =
    val mutable ShaderId: int
    val mutable IsActive: bool
    new(shaderId: int, isActive: bool) = { ShaderId = shaderId; IsActive = isActive }
