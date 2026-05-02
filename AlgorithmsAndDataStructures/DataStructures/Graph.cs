// Graph.cs - generic adjacency-list graph with BFS / DFS helpers.
// Space: O(V + E). Use for sparse graphs; switch to a 2D matrix for dense.

using System.Collections.Generic;

namespace AlgorithmsAndDataStructures.DataStructures;

public sealed class Graph<TNode> where TNode : notnull
{
    private readonly Dictionary<TNode, List<TNode>> _adj = new();
    public bool Directed { get; }

    public Graph(bool directed = false) => Directed = directed;

    public IEnumerable<TNode> Nodes => _adj.Keys;

    public void AddNode(TNode n) => _adj.TryAdd(n, new List<TNode>());

    public void AddEdge(TNode u, TNode v)
    {
        AddNode(u);
        AddNode(v);
        _adj[u].Add(v);
        if (!Directed) _adj[v].Add(u);
    }

    public IReadOnlyList<TNode> Neighbors(TNode n)
        => _adj.TryGetValue(n, out var l) ? l : (IReadOnlyList<TNode>)System.Array.Empty<TNode>();

    // BFS distances from `source`.
    public Dictionary<TNode, int> Bfs(TNode source)
    {
        var dist = new Dictionary<TNode, int> { [source] = 0 };
        var q = new Queue<TNode>();
        q.Enqueue(source);
        while (q.Count > 0)
        {
            var u = q.Dequeue();
            foreach (var v in Neighbors(u))
                if (!dist.ContainsKey(v))
                {
                    dist[v] = dist[u] + 1;
                    q.Enqueue(v);
                }
        }
        return dist;
    }

    // Iterative DFS preorder.
    public List<TNode> Dfs(TNode source)
    {
        var visited = new HashSet<TNode>();
        var order = new List<TNode>();
        var stack = new Stack<TNode>();
        stack.Push(source);
        while (stack.Count > 0)
        {
            var u = stack.Pop();
            if (!visited.Add(u)) continue;
            order.Add(u);
            var neigh = Neighbors(u);
            for (int i = neigh.Count - 1; i >= 0; i--)
                if (!visited.Contains(neigh[i])) stack.Push(neigh[i]);
        }
        return order;
    }
}
