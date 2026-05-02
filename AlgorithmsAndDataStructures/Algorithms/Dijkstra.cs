// Dijkstra.cs - single-source shortest path on a weighted graph with
// non-negative edges. O((V + E) log V) time, O(V) space.
// Uses .NET 6+ PriorityQueue<TElement, TPriority>.

using System.Collections.Generic;

namespace AlgorithmsAndDataStructures.Algorithms;

public readonly record struct Edge(int To, double Weight);

public static class Dijkstra
{
    public static Dictionary<int, double> ShortestPaths(
        Dictionary<int, List<Edge>> graph,
        int source)
    {
        var dist = new Dictionary<int, double> { [source] = 0.0 };
        var pq = new PriorityQueue<int, double>();
        pq.Enqueue(source, 0.0);

        while (pq.TryDequeue(out int u, out double d))
        {
            // Skip stale entries - PriorityQueue does not support decrease-key,
            // so we lazy-delete by comparing against the recorded distance.
            if (d > dist[u]) continue;
            if (!graph.TryGetValue(u, out var edges)) continue;

            foreach (var (v, w) in edges)
            {
                double next = d + w;
                if (!dist.TryGetValue(v, out double cur) || next < cur)
                {
                    dist[v] = next;
                    pq.Enqueue(v, next);
                }
            }
        }
        return dist;
    }
}
