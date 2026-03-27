namespace Benchmarks.Core.Components

open System
open System.Collections.Generic

[<Flags>]
type InputInUse =
    | None = 0
    | HasMouse = 1
    | HasGamepad = 2
    | HasKeyboard = 4

type ActionEntityInput =
    | None = 0
    | MoveUp = 1
    | MoveDown = 2
    | MoveLeft = 3
    | MoveRight = 4
    | Jump = 5
    | Attack = 6
    | Interact = 7

[<Struct>]
type InputComponent =
    val mutable private InputsInUse: int
    val mutable KeyBoard: Dictionary<ActionEntityInput, int>
    val mutable GamePad: Dictionary<ActionEntityInput, int>
    val mutable Mouse: Dictionary<ActionEntityInput, int>
    val mutable GamePadIndex: int

    member this.HasInput(input: InputInUse) = (this.InputsInUse &&& int input) <> 0
    member this.AddInput(input: InputInUse) = this.InputsInUse <- this.InputsInUse ||| int input
    member this.RemoveInput(input: InputInUse) = this.InputsInUse <- this.InputsInUse &&& ~~~(int input)
