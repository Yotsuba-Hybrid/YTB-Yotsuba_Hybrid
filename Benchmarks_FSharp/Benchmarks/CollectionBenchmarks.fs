namespace Benchmarks.Benchmarks

open System.Collections.Generic
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Jobs
open Benchmarks.Core.Collections
open Benchmarks.Core.Components
open Benchmarks.Core.Types

[<MemoryDiagnoser>]
[<SimpleJob(RuntimeMoniker.HostProcess)>]
type CollectionBenchmarks() =

    [<Params(100, 1000, 5000, 10000)>]
    member val Count = 0 with get, set

    member val private _ytb: YTB<TransformComponent> = Unchecked.defaultof<YTB<TransformComponent>> with get, set
    member val private _list: List<TransformComponent> = Unchecked.defaultof<List<TransformComponent>> with get, set
    member val private _sample: TransformComponent = Unchecked.defaultof<TransformComponent> with get, set

    [<GlobalSetup>]
    member this.Setup() =
        this._sample <- TransformComponent(Vector3(100f, 200f, 0f), Vector3(32f, 32f, 0f))

        this._ytb <- YTB<TransformComponent>()
        this._list <- List<TransformComponent>(this.Count)

        for i = 0 to this.Count - 1 do
            this._ytb.Add(TransformComponent(
                Vector3(float32 i * 10f, float32 i * 5f, 0f),
                Vector3(32f, 32f, 0f),
                1f, SpriteEffects.None, Color.White)) |> ignore
            this._list.Add(TransformComponent(
                Vector3(float32 i * 10f, float32 i * 5f, 0f),
                Vector3(32f, 32f, 0f),
                1f, SpriteEffects.None, Color.White))

    [<Benchmark(Description = "YTB<T>.Add N elements")>]
    member this.YTB_Add() =
        let ytb = YTB<TransformComponent>()
        for _ = 0 to this.Count - 1 do
            ytb.Add(this._sample) |> ignore
        ytb

    [<Benchmark(Description = "List<T>.Add N elements")>]
    member this.List_Add() =
        let list = List<TransformComponent>(500)
        for _ = 0 to this.Count - 1 do
            list.Add(this._sample)
        list

    [<Benchmark(Description = "YTB<T>.AsSpan() iterate")>]
    member this.YTB_IterateSpan() =
        let mutable sum = 0f
        for t in this._ytb.AsSpan() do
            sum <- sum + t.Position.X + t.Position.Y
        sum

    [<Benchmark(Description = "List<T> foreach iterate")>]
    member this.List_Iterate() =
        let mutable sum = 0f
        for t in this._list do
            sum <- sum + t.Position.X + t.Position.Y
        sum

    [<Benchmark(Description = "YTB<T> ref index access")>]
    member this.YTB_IndexAccess() =
        let mutable sum = 0f
        for i = 0 to this._ytb.Count - 1 do
            let t = &this._ytb.[i]
            sum <- sum + t.Position.X
        sum

    [<Benchmark(Description = "List<T> index access")>]
    member this.List_IndexAccess() =
        let mutable sum = 0f
        for i = 0 to this._list.Count - 1 do
            let t = this._list.[i]
            sum <- sum + t.Position.X
        sum
