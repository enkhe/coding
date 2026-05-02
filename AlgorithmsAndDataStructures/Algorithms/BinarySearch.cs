// BinarySearch.cs - canonical, lower-bound, and upper-bound variants.
//
// Bug magnet: get the invariant right and write it as a comment.
// Use `lo + (hi - lo) / 2` to avoid integer overflow.

using System;
using System.Collections.Generic;

namespace AlgorithmsAndDataStructures.Algorithms;

public static class BinarySearch
{
    // Classic search: returns index of target or -1. O(log n) time, O(1) space.
    // Invariant: target, if present, is in [lo..hi].
    public static int Search<T>(IReadOnlyList<T> sorted, T target, IComparer<T>? cmp = null)
    {
        cmp ??= Comparer<T>.Default;
        int lo = 0, hi = sorted.Count - 1;
        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2;
            int c = cmp.Compare(sorted[mid], target);
            if (c == 0) return mid;
            if (c < 0) lo = mid + 1;
            else hi = mid - 1;
        }
        return -1;
    }

    // LowerBound: smallest index i such that sorted[i] >= target.
    // Returns sorted.Count if target is greater than every element.
    // Equivalent to C++ std::lower_bound.
    public static int LowerBound<T>(IReadOnlyList<T> sorted, T target, IComparer<T>? cmp = null)
    {
        cmp ??= Comparer<T>.Default;
        int lo = 0, hi = sorted.Count;
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            if (cmp.Compare(sorted[mid], target) < 0) lo = mid + 1;
            else hi = mid;
        }
        return lo;
    }

    // UpperBound: smallest index i such that sorted[i] > target.
    public static int UpperBound<T>(IReadOnlyList<T> sorted, T target, IComparer<T>? cmp = null)
    {
        cmp ??= Comparer<T>.Default;
        int lo = 0, hi = sorted.Count;
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            if (cmp.Compare(sorted[mid], target) <= 0) lo = mid + 1;
            else hi = mid;
        }
        return lo;
    }

    // Binary search on the answer: smallest x in [lo..hi] for which predicate is true.
    // Predicate must be monotonic (false ... false true ... true).
    public static long FirstTrue(long lo, long hi, Func<long, bool> predicate)
    {
        while (lo < hi)
        {
            long mid = lo + (hi - lo) / 2;
            if (predicate(mid)) hi = mid;
            else lo = mid + 1;
        }
        return lo;
    }
}
