// RegexExamples.cs
// [GeneratedRegex] compiles to IL at build time -> faster startup, AOT-safe.

using System.Text.RegularExpressions;

namespace BackEnd.CSharp.SourceGenerators;

public static partial class Patterns
{
    [GeneratedRegex(@"^[a-z0-9._-]+@[a-z0-9.-]+\.[a-z]{2,}$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    public static partial Regex Email();

    [GeneratedRegex(@"\d+")]
    public static partial Regex Digits();
}

public static class PatternsDemo
{
    public static bool IsEmail(string s) => Patterns.Email().IsMatch(s);

    public static IEnumerable<string> ExtractNumbers(string s) =>
        Patterns.Digits().Matches(s).Select(m => m.Value);
}
