// CSharp14Features.cs
// Annotated tour of C# 14 (.NET 10) language features.
// Compile target: net10.0, LangVersion=14 (or preview).

using System;
using System.Collections.Generic;

namespace BackEnd.CSharp.CSharp14;

// ---------------------------------------------------------------------------
// 1. Primary constructors (recap from C# 12) + field-backed properties (C# 14)
// ---------------------------------------------------------------------------
public sealed class User(string email)
{
    // The primary-constructor parameter `email` is captured as instance state.
    // We expose it through a property that uses the new `field` keyword for
    // its compiler-synthesized backing field; the setter normalizes input.
    public string Email
    {
        get => field;
        set => field = (value ?? throw new ArgumentNullException(nameof(value))).Trim().ToLowerInvariant();
    } = email;

    // Required + init for compiler-enforced initialization at construction.
    public required string DisplayName { get; init; }
}

// ---------------------------------------------------------------------------
// 2. Extension members: properties AND statics, not just methods.
// ---------------------------------------------------------------------------
public static class StringExtensions
{
    extension(string s)
    {
        // Extension *property*
        public bool IsBlank => string.IsNullOrWhiteSpace(s);

        // Extension *instance method* (also still supported)
        public string Truncate(int max) =>
            s.Length <= max ? s : s[..max];
    }

    extension(string)
    {
        // Extension *static* member: callable as `string.Hello`.
        public static string Hello => "hello";
    }
}

// ---------------------------------------------------------------------------
// 3. params collections: ReadOnlySpan<T>, IEnumerable<T>, etc.
//    Avoids the implicit T[] allocation that classic params requires.
// ---------------------------------------------------------------------------
public static class Aggregates
{
    public static int Sum(params ReadOnlySpan<int> values)
    {
        var total = 0;
        foreach (var v in values) total += v;
        return total;
    }

    public static string Join(string sep, params IEnumerable<string> parts) =>
        string.Join(sep, parts);
}

// ---------------------------------------------------------------------------
// 4. Lambda parameter modifiers: ref / in / out / scoped on lambdas.
// ---------------------------------------------------------------------------
public delegate void RefAction<T>(ref T value);

public static class LambdaModifiers
{
    public static readonly RefAction<int> Increment = (ref int x) => x++;

    public static void Demo()
    {
        var n = 41;
        Increment(ref n);
        Console.WriteLine(n); // 42
    }
}

// ---------------------------------------------------------------------------
// 5. Unbound generic name in `nameof`.
// ---------------------------------------------------------------------------
public static class NameOfDemo
{
    public static readonly string Open = nameof(Dictionary<,>);   // "Dictionary"
    public static readonly string Closed = nameof(Dictionary<int, string>); // "Dictionary"
}

// ---------------------------------------------------------------------------
// 6. First-class span conversions: T[] flows into Span<T> / ReadOnlySpan<T>
//    transparently in more positions, including overload resolution.
// ---------------------------------------------------------------------------
public static class SpanConversions
{
    public static int Total(ReadOnlySpan<int> xs)
    {
        var t = 0;
        foreach (var x in xs) t += x;
        return t;
    }

    public static void Use()
    {
        int[] arr = [1, 2, 3, 4];
        var t = Total(arr); // implicit T[] -> ReadOnlySpan<T>
        Console.WriteLine(t);
    }
}

// ---------------------------------------------------------------------------
// 7. Partial events (and partial constructors) — pair with source generators.
//    Declaration in one file, implementation in another.
// ---------------------------------------------------------------------------
public partial class Bus
{
    public partial event EventHandler<string>? Message;

    public void Publish(string msg) => OnMessage(msg);
    private partial void OnMessage(string msg);
}

public partial class Bus
{
    public partial event EventHandler<string>? Message
    {
        add => field += value;     // `field` legal in partial event accessors
        remove => field -= value;
    }

    private partial void OnMessage(string msg) => Message?.Invoke(this, msg);
}

// ---------------------------------------------------------------------------
// Entry point so you can run a quick smoke test.
// ---------------------------------------------------------------------------
internal static class Program
{
    public static void Main()
    {
        var u = new User("  Foo@Bar.com ") { DisplayName = "Foo" };
        Console.WriteLine($"{u.DisplayName} <{u.Email}>");          // foo@bar.com
        Console.WriteLine("   ".IsBlank);                            // True
        Console.WriteLine(string.Hello);                              // hello
        Console.WriteLine(Aggregates.Sum(1, 2, 3, 4));                // 10
        LambdaModifiers.Demo();                                       // 42
        SpanConversions.Use();                                        // 10
        Console.WriteLine(NameOfDemo.Open);                           // Dictionary
    }
}
