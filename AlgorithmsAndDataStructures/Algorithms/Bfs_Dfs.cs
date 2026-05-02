// Bfs_Dfs.cs - graph traversal templates on adjacency-list graphs.
// O(V + E) time and O(V) space for both. Iterative DFS avoids stack overflow.

using System.Collections.Generic;

namespace AlgorithmsAndDataStructures.Algorithms;

public static class GraphTraversal
{
    // BFS: shortest hops from `source` on an unweighted graph.
    // Returns a distances dictionary; unreachable nodes are absent.
    public static Dictionary<int, int> Bfs(Dictionary<int, List<int>> graph, int source)
    {
        var dist = new Dictionary<int, int> { [source] = 0 };
        var queue = new Queue<int>();
        queue.Enqueue(source);

        while (queue.Count > 0)
        {
            int u = queue.Dequeue();
            if (!graph.TryGetValue(u, out var neighbors)) continue;
            foreach (int v in neighbors)
            {
                if (dist.ContainsKey(v)) continue;
                dist[v] = dist[u] + 1;
                queue.Enqueue(v);
            }
        }
        return dist;
    }

    // Iterative DFS: returns nodes in pre-order.
    public static List<int> DfsPreorder(Dictionary<int, List<int>> graph, int source)
    {
        var order = new List<int>();
        var visited = new HashSet<int>();
        var stack = new Stack<int>();
        stack.Push(source);

        while (stack.Count > 0)
        {
            int u = stack.Pop();
            if (!visited.Add(u)) continue;
            order.Add(u);

            if (!graph.TryGetValue(u, out var neighbors)) continue;
            // Push in reverse so iteration order matches recursive DFS.
            for (int i = neighbors.Count - 1; i >= 0; i--)
                if (!visited.Contains(neighbors[i]))
                    stack.Push(neighbors[i]);
        }
        return order;
    }

    // Topological sort via DFS with three-color marking.
    // Returns null if the graph has a cycle.
    public static List<int>? TopoSort(Dictionary<int, List<int>> graph)
    {
        const int White = 0, Gray = 1, Black = 2;
        var color = new Dictionary<int, int>();
        foreach (int node in graph.Keys) color[node] = White;
        var order = new List<int>();

        bool Visit(int u)
        {
            if (color[u] == Gray) return false; // back edge -> cycle
            if (color[u] == Black) return true;
            color[u] = Gray;
            if (graph.TryGetValue(u, out var neighbors))
                foreach (int v in neighbors)
                {
                    color.TryAdd(v, White);
                    if (!Visit(v)) return false;
                }
            color[u] = Black;
            order.Add(u);
            return true;
        }

        foreach (int node in graph.Keys)
            if (color[node] == White && !Visit(node))
                return null;

        order.Reverse();
        return order;
    }
}
