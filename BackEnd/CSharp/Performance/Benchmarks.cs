// Benchmarks.cs
// Run with: dotnet run -c Release --project Performance.csproj
// Requires NuGet: BenchmarkDotNet
//
// Always benchmark before optimizing. Compare allocations, not just time.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BackEnd.CSharp.Performance;

[MemoryDiagnoser]
public class StringSumBench
{
    private readonly string _csv = string.Join(",", Enumerable.Range(0, 100));

    [Benchmark(Baseline = true)]
    public int LinqAndSplit() =>
        _csv.Split(',').Select(int.Parse).Sum();

    [Benchmark]
    public int SpanBased() => SpanBasics.SumCsv(_csv);
}

internal static class BenchEntry
{
    public static void Main() => BenchmarkRunner.Run<StringSumBench>();
}
