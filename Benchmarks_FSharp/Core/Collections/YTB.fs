namespace Benchmarks.Core.Collections

open System
open System.Collections.Generic
open System.Linq
open System.Runtime.CompilerServices
open Benchmarks.Core.Entity

type YTB<'T>() =
    let _predetSizesOfYTB = [| 500; 1000; 5000; 10_000; 50_000; 100_000; 500_000; 1_000_000; 5_000_000; 10_000_000; 20_000_000; 30_000_000; 40_000_000; 50_000_000 |]
    let mutable predetCurrentSizeIndex = 0
    let mutable _arr: 'T array = Array.zeroCreate _predetSizesOfYTB.[0]
    let mutable _count = 0
    let mutable _defaultValue: 'T = Unchecked.defaultof<'T>

    member _.Capacity
        with get() = _predetSizesOfYTB.[predetCurrentSizeIndex]

    member internal _.InternalArray
        with get() = _arr

    member _._ytb = _arr.Take(_count)

    member _.Count
        with get() = _count

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.Add(item: 'T) =
        if _count >= _predetSizesOfYTB.[predetCurrentSizeIndex] then
            predetCurrentSizeIndex <- predetCurrentSizeIndex + 1
            if predetCurrentSizeIndex >= _predetSizesOfYTB.Length then
                raise (IndexOutOfRangeException("YTB has reached its maximum capacity."))
            let newArray = Array.zeroCreate _predetSizesOfYTB.[predetCurrentSizeIndex]
            Array.Copy(_arr, newArray, _count)
            _arr <- newArray
        _arr.[_count] <- item
        let index = _count
        _count <- _count + 1
        index

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member _.Add(item: 'T, index: outref<int>) =
        if _count >= _predetSizesOfYTB.[predetCurrentSizeIndex] then
            predetCurrentSizeIndex <- predetCurrentSizeIndex + 1
            if predetCurrentSizeIndex >= _predetSizesOfYTB.Length then
                raise (IndexOutOfRangeException("YTB has reached its maximum capacity."))
            let newArray = Array.zeroCreate _predetSizesOfYTB.[predetCurrentSizeIndex]
            Array.Copy(_arr, newArray, _count)
            _arr <- newArray
        _arr.[_count] <- item
        let idx = _count
        _count <- _count + 1
        index <- idx
        idx

    member _.RemoveAt(index: int) =
        if index < 0 || index >= _count then
            false
        else
            let shiftCount = _count - index - 1
            if shiftCount > 0 then
                Array.Copy(_arr, index + 1, _arr, index, shiftCount)
            _count <- _count - 1
            _arr.[_count] <- Unchecked.defaultof<'T>
            true

    member this.Remove(item: 'T) =
        let index = Array.IndexOf(_arr, item, 0, _count)
        if index < 0 then false
        else this.RemoveAt(index)

    member _.Item
        with get(index: int) : 'T byref =
            if index < 0 || index >= _count then
                raise (ArgumentOutOfRangeException(nameof index, "Index is out of range."))
            &_arr.[index]

    member _.ItemByEntity
        with get(entity: Yotsuba) : 'T byref =
            if entity.Id < 0 || entity.Id >= _count then
                raise (ArgumentOutOfRangeException("entity.Id", "Index is out of range."))
            &_arr.[entity.Id]

    member _.SetByUint(index: uint, value: 'T) =
        if int index >= _count then
            raise (ArgumentOutOfRangeException(nameof index, "Index is out of range."))
        _arr.[int index] <- value

    member _.ToArray() =
        let result = Array.zeroCreate _count
        Array.Copy(_arr, result, _count)
        result

    member _.ToList() =
        let list = List<'T>(_count)
        list.AddRange(_arr.AsSpan(0, _count))
        list

    member _.AsReadOnlySpan() : ReadOnlySpan<'T> =
        _arr.AsSpan(0, _count) |> ReadOnlySpan.op_Implicit

    member _.AsSpan() : Span<'T> =
        _arr.AsSpan(0, _count)

    member _.Clear() =
        _count <- 0
        predetCurrentSizeIndex <- 0
        Array.Clear(_arr, 0, _predetSizesOfYTB.[0])

    member _.Find(predicate: Predicate<'T>) : 'T byref =
        let index = Array.FindIndex(_arr, 0, _count, predicate)
        if index = -1 then
            &_defaultValue
        else
            &_arr.[index]
