open BenchmarkDotNet.Running

[<EntryPoint>]
let main args =
    BenchmarkSwitcher.FromAssembly(typeof<Benchmarks.Benchmarks.FullSceneBenchmarks>.Assembly).Run(args) |> ignore
    0
