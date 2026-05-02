using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Testing.Performance;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net100)]
public class StringBuildingBenchmark
{
    [Params(10, 100, 1_000)]
    public int Iterations;

    [Benchmark(Baseline = true)]
    public string Concatenation()
    {
        var s = string.Empty;
        for (var i = 0; i < Iterations; i++) s += "x";
        return s;
    }

    [Benchmark]
    public string StringBuilder()
    {
        var sb = new StringBuilder(Iterations);
        for (var i = 0; i < Iterations; i++) sb.Append('x');
        return sb.ToString();
    }

    [Benchmark]
    public string CreateString() => new('x', Iterations);
}

public static class Program
{
    public static void Main(string[] args) =>
        BenchmarkRunner.Run<StringBuildingBenchmark>(args: args);
}
