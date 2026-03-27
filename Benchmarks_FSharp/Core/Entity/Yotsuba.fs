namespace Benchmarks.Core.Entity

open System

[<Flags>]
type YTBComponent =
    | Sprite = 1us
    | Transform = 2us
    | Animation = 4us
    | Rigibody = 8us
    | Input = 16us
    | Model3D = 32us
    | Button2D = 64us
    | Camera = 128us
    | Script = 256us
    | TileMap = 512us
    | Font = 1024us
    | Shader = 2048us
    | YTBUIElement = 4096us
    | YTBModel3D = 16384us

[<Struct>]
type Yotsuba =
    val mutable Id: int
    val mutable Name: string
    val mutable Components: int

    new(id: int) = { Id = id; Name = null; Components = 0 }

    member this.HasComponent(component: YTBComponent) =
        (this.Components &&& int component) <> 0

    member this.HasNotComponent(component: YTBComponent) =
        not (this.HasComponent(component))

    member this.AddComponent(component: YTBComponent) =
        this.Components <- this.Components ||| int component

    member this.RemoveComponent(component: YTBComponent) =
        this.Components <- this.Components &&& ~~~(int component)
