namespace Benchmarks.Core.Components

open Benchmarks.Core.Types

[<Struct>]
type ModelComponent3D =
    val mutable ModelId: int
    val mutable IsVisible: bool
    val mutable RadiusSphere: float32
    val mutable SphereOffset: Vector3
