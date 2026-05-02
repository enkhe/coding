// Sorting.cs - reference comparison sorts in idiomatic .NET 10.
//
// Prefer Array.Sort / List<T>.Sort in production - the BCL ships an
// introsort hybrid (quick + heap + insertion) that beats hand-rolled
// implementations. The code below is for study and interviews.

using System;
using System.Collections.Generic;

namespace AlgorithmsAndDataStructures.Algorithms;

public static class Sorting
{
    // Quicksort: average O(n log n), worst O(n^2), space O(log n).
    // In-place, not stable. Lomuto partition; randomized pivot to avoid
    // adversarial inputs.
    public static void QuickSort<T>(T[] a, IComparer<T>? cmp = null)
    {
        cmp ??= Comparer<T>.Default;
        QuickSortRange(a, 0, a.Length - 1, cmp, new Random(42));
    }

    private static void QuickSortRange<T>(T[] a, int lo, int hi, IComparer<T> cmp, Random rng)
    {
        while (lo < hi)
        {
            int pivotIndex = lo + rng.Next(hi - lo + 1);
            (a[pivotIndex], a[hi]) = (a[hi], a[pivotIndex]);
            T pivot = a[hi];

            int i = lo - 1;
            for (int j = lo; j < hi; j++)
            {
                if (cmp.Compare(a[j], pivot) <= 0)
                {
                    i++;
                    (a[i], a[j]) = (a[j], a[i]);
                }
            }
            (a[i + 1], a[hi]) = (a[hi], a[i + 1]);
            int p = i + 1;

            // Recurse on smaller side, iterate on larger - bounds stack at O(log n).
            if (p - lo < hi - p)
            {
                QuickSortRange(a, lo, p - 1, cmp, rng);
                lo = p + 1;
            }
            else
            {
                QuickSortRange(a, p + 1, hi, cmp, rng);
                hi = p - 1;
            }
        }
    }

    // Mergesort: O(n log n) time, O(n) space. Stable. Ideal for linked lists,
    // external sort, or when stability is required.
    public static void MergeSort<T>(T[] a, IComparer<T>? cmp = null)
    {
        cmp ??= Comparer<T>.Default;
        var buffer = new T[a.Length];
        MergeSortRange(a, buffer, 0, a.Length - 1, cmp);
    }

    private static void MergeSortRange<T>(T[] a, T[] buf, int lo, int hi, IComparer<T> cmp)
    {
        if (lo >= hi) return;
        int mid = lo + (hi - lo) / 2;
        MergeSortRange(a, buf, lo, mid, cmp);
        MergeSortRange(a, buf, mid + 1, hi, cmp);
        Merge(a, buf, lo, mid, hi, cmp);
    }

    private static void Merge<T>(T[] a, T[] buf, int lo, int mid, int hi, IComparer<T> cmp)
    {
        int i = lo, j = mid + 1, k = lo;
        while (i <= mid && j <= hi)
            buf[k++] = cmp.Compare(a[i], a[j]) <= 0 ? a[i++] : a[j++];
        while (i <= mid) buf[k++] = a[i++];
        while (j <= hi) buf[k++] = a[j++];
        Array.Copy(buf, lo, a, lo, hi - lo + 1);
    }

    // Heapsort: O(n log n), O(1) extra space. Not stable. Good when you need
    // guaranteed worst-case performance.
    public static void HeapSort<T>(T[] a, IComparer<T>? cmp = null)
    {
        cmp ??= Comparer<T>.Default;
        int n = a.Length;
        for (int i = n / 2 - 1; i >= 0; i--) SiftDown(a, i, n, cmp);
        for (int end = n - 1; end > 0; end--)
        {
            (a[0], a[end]) = (a[end], a[0]);
            SiftDown(a, 0, end, cmp);
        }
    }

    private static void SiftDown<T>(T[] a, int start, int end, IComparer<T> cmp)
    {
        int root = start;
        while (true)
        {
            int child = 2 * root + 1;
            if (child >= end) return;
            if (child + 1 < end && cmp.Compare(a[child], a[child + 1]) < 0) child++;
            if (cmp.Compare(a[root], a[child]) >= 0) return;
            (a[root], a[child]) = (a[child], a[root]);
            root = child;
        }
    }
}
