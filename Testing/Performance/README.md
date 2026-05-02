# Performance Tests

> Measure throughput, latency, and allocations - separate micro-benchmarks from load tests.

## Core Concepts

- **Micro-benchmarks (BenchmarkDotNet)**: nanosecond-accurate measurements of single methods. Always run in `Release`, outside the debugger.
- **Load tests (NBomber, k6)**: drive the deployed system at realistic concurrency and durations; capture latency percentiles, not averages.
- **Profile, do not guess**: PerfView, dotTrace, or `dotnet-trace` for hotspots; `dotnet-counters` for live metrics.
- **Avoid the streetlight effect**: optimize the slow path, not the obvious path.

## "To Be Dangerous" Cheatsheet

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net100)]
public class StringBench
{
    [Params(10, 1_000)] public int N;

    [Benchmark(Baseline = true)]
    public string Concat() { var s = ""; for (int i = 0; i < N; i++) s += "x"; return s; }

    [Benchmark]
    public string Builder() { var sb = new StringBuilder(); for (int i = 0; i < N; i++) sb.Append('x'); return sb.ToString(); }
}
```

```bash
# Run benchmarks
dotnet run -c Release --project Benchmarks

# k6 load
k6 run --vus 50 --duration 2m loadtest.k6.js
```

| Tool             | Purpose                                                |
|------------------|--------------------------------------------------------|
| BenchmarkDotNet  | Micro-bench, allocations, JIT/AOT comparisons          |
| NBomber          | In-proc load test, scenario DSL, .NET native           |
| k6               | HTTP load test, JS scenarios, CI friendly              |
| dotnet-counters  | Live process metrics                                   |
| dotnet-trace     | Sampling profiler, ETW/EventPipe                       |
| PerfView         | Deep CLR analysis (Windows)                            |

## Quick Reference

- Latency: report **p50, p95, p99**, never just mean.
- Warmup: BenchmarkDotNet auto-warms; for k6 use `stages` with ramp-up.
- Compare: `[Benchmark(Baseline = true)]` to anchor relative results.
- Track regressions: store BenchmarkDotNet `BenchmarkDotNet.Artifacts/results` JSON in CI; diff in PRs.

## Common Pitfalls

- Benchmarking in Debug (10-100x slower).
- Allocating in the timed section by accident (closures, boxing).
- Coordinated omission in load tests - use open-model k6 with `arrival-rate` executor.
- Tiny load durations - run >= 2 minutes, the JIT and GC need time to settle.
- Local-laptop benchmarks treated as production truth.

## Examples in this folder

- [MyBenchmark.cs](./MyBenchmark.cs) - BenchmarkDotNet with `[MemoryDiagnoser]`
- [LoadScenario.cs](./LoadScenario.cs) - NBomber HTTP scenario
- [loadtest.k6.js](./loadtest.k6.js) - k6 ramped HTTP load

## See also

- [../../Observability/Metrics/README.md](../../Observability/Metrics/README.md) - what to measure in prod
- [../../Observability/SLOs/README.md](../../Observability/SLOs/README.md) - latency budgets
