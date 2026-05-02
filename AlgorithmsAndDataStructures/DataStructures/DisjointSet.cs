// DisjointSet.cs - Union-Find with union-by-rank and path compression.
// Amortized near O(1) per operation: O(alpha(n)).

using System;

namespace AlgorithmsAndDataStructures.DataStructures;

public sealed class DisjointSet
{
    private readonly int[] _parent;
    private readonly int[] _rank;
    public int Count { get; }

    public DisjointSet(int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        Count = count;
        _parent = new int[count];
        _rank = new int[count];
        for (int i = 0; i < count; i++) _parent[i] = i;
    }

    public int Find(int x)
    {
        // Path compression: point every node on the path to the root.
        while (_parent[x] != x)
        {
            _parent[x] = _parent[_parent[x]];
            x = _parent[x];
        }
        return x;
    }

    // Returns true if a merge actually happened.
    public bool Union(int a, int b)
    {
        int ra = Find(a), rb = Find(b);
        if (ra == rb) return false;

        // Union by rank: attach the smaller tree under the larger.
        if (_rank[ra] < _rank[rb]) (ra, rb) = (rb, ra);
        _parent[rb] = ra;
        if (_rank[ra] == _rank[rb]) _rank[ra]++;
        return true;
    }

    public bool Connected(int a, int b) => Find(a) == Find(b);
}
