namespace Benchmarks.Core.Scene

open System
open System.Collections.Generic

type EventManager() =
    let _subscribers = Dictionary<Type, List<Delegate>>()

    static let mutable _instance: EventManager = EventManager()

    static member Instance
        with get() = _instance

    member _.Subscribe<'T>(handler: Action<'T>) =
        let t = typeof<'T>
        let mutable list = Unchecked.defaultof<List<Delegate>>
        if not (_subscribers.TryGetValue(t, &list)) then
            list <- List<Delegate>()
            _subscribers.[t] <- list
        list.Add(handler)

    member _.Publish<'T>(eventData: 'T) =
        let t = typeof<'T>
        match _subscribers.TryGetValue(t) with
        | true, list ->
            for handler in list do
                (handler :?> Action<'T>).Invoke(eventData)
        | _ -> ()

    member _.Clear() =
        _subscribers.Clear()

    static member Reset() =
        _instance <- EventManager()
