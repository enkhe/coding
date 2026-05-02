// BloomFilter.cs - probabilistic set membership.
// No false negatives; tunable false-positive rate p with optimal parameters
//   m = -(n * ln p) / (ln 2)^2  bits
//   k = (m / n) * ln 2          hash functions
// Cannot delete entries (use a counting bloom filter for that).

using System;
using System.Collections;
using System.Text;

namespace AlgorithmsAndDataStructures.DataStructures;

public sealed class BloomFilter<T> where T : notnull
{
    private readonly BitArray _bits;
    private readonly int _hashCount;
    private readonly int _size;

    public BloomFilter(int expectedItems, double falsePositiveRate = 0.01)
    {
        if (expectedItems <= 0) throw new ArgumentOutOfRangeException(nameof(expectedItems));
        if (falsePositiveRate is <= 0 or >= 1) throw new ArgumentOutOfRangeException(nameof(falsePositiveRate));

        _size = (int)Math.Ceiling(-(expectedItems * Math.Log(falsePositiveRate)) / (Math.Log(2) * Math.Log(2)));
        _hashCount = (int)Math.Max(1, Math.Round((_size / (double)expectedItems) * Math.Log(2)));
        _bits = new BitArray(_size);
    }

    public void Add(T item)
    {
        foreach (int idx in Indices(item)) _bits[idx] = true;
    }

    public bool MightContain(T item)
    {
        foreach (int idx in Indices(item))
            if (!_bits[idx]) return false;
        return true;
    }

    private System.Collections.Generic.IEnumerable<int> Indices(T item)
    {
        // Double hashing: combine two independent hashes into k positions.
        // h_i(x) = (h1 + i * h2) mod m
        byte[] bytes = Encoding.UTF8.GetBytes(item.ToString() ?? string.Empty);
        int h1 = (int)Fnv1a(bytes, 2166136261u);
        int h2 = (int)Fnv1a(bytes, 16777619u);
        for (int i = 0; i < _hashCount; i++)
        {
            int combined = unchecked(h1 + i * h2);
            int idx = (int)((uint)combined % (uint)_size);
            yield return idx;
        }
    }

    private static uint Fnv1a(byte[] data, uint seed)
    {
        uint hash = seed;
        foreach (byte b in data)
        {
            hash ^= b;
            hash *= 16777619u;
        }
        return hash;
    }
}
