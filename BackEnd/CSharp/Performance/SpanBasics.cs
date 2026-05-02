// SpanBasics.cs
// Span<T> / ReadOnlySpan<T> are ref structs: stack-only, no heap alloc, no GC.

using System.Runtime.CompilerServices;

namespace BackEnd.CSharp.Performance;

public static class SpanBasics
{
    // Stackalloc small buffer, slice, parse — zero allocation.
    public static int SumCsv(ReadOnlySpan<char> text)
    {
        var total = 0;
        foreach (var range in text.Split(','))
        {
            var part = text[range].Trim();
            if (part.Length > 0)
                total += int.Parse(part);
        }
        return total;
    }

    // Format an int into a stack buffer instead of allocating a string.
    [SkipLocalsInit]
    public static int IntToCharsStack(int value, Span<char> destination)
    {
        if (!value.TryFormat(destination, out var written))
            throw new ArgumentException("Buffer too small.", nameof(destination));
        return written;
    }
}
