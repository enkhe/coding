# Algorithms

> Classic algorithm catalog with idiomatic C# (.NET 10) reference implementations.

## Core Concepts

- **Sorting**: comparison sorts (quick, merge, heap) at O(n log n); non-comparison (counting, radix, bucket) at O(n + k) for bounded keys.
- **Searching**: linear O(n), binary O(log n) on sorted input, hash lookup O(1) average.
- **Graph**: BFS for shortest path on unweighted, DFS for topological sort and cycle detection, Dijkstra/A* for weighted shortest path, Bellman-Ford for negative edges, Kruskal/Prim for MST.
- **Dynamic programming**: optimal substructure + overlapping subproblems. Memoize then tabulate. Watch state size.
- **Greedy**: locally optimal choices, prove with exchange argument or matroid.
- **Divide and conquer**: split, solve, combine. Master theorem gives complexity.
- **Backtracking**: enumerate with pruning - n-queens, sudoku, permutations, subsets.
- **Bit manipulation**: XOR tricks, popcount, bitmask DP, set representation.
- **String**: KMP and Z-algorithm O(n + m), Rabin-Karp with rolling hash, suffix arrays/automata for advanced.

## "To Be Dangerous" Cheatsheet

| Family        | When to use                                  | Canonical algorithm                | Complexity         |
| ------------- | -------------------------------------------- | ---------------------------------- | ------------------ |
| Sort          | Order data                                   | Quicksort / mergesort              | O(n log n)         |
| Search sorted | Find element                                 | Binary search                      | O(log n)           |
| Unweighted SP | Min hops                                     | BFS                                | O(V + E)           |
| Weighted SP   | Non-negative                                 | Dijkstra                           | O((V+E) log V)     |
| Weighted SP   | Negative edges                               | Bellman-Ford                       | O(V * E)           |
| All-pairs SP  | Dense small graph                            | Floyd-Warshall                     | O(V^3)             |
| MST           | Min weight spanning tree                     | Kruskal (sparse), Prim (dense)     | O(E log V)         |
| String search | Single pattern                               | KMP                                | O(n + m)           |
| String search | Multiple patterns                            | Aho-Corasick                       | O(n + m + z)       |
| DP            | Optimal substructure + overlap               | Tabulation                         | varies             |
| Subsets       | 2^n enumeration                              | Bitmask                            | O(2^n * n)         |

### Picking a sort

- `Array.Sort` / `List<T>.Sort` use Tim-style introsort; ship with that unless you need stability.
- Use `OrderBy` (LINQ) for stable sort when stability matters.
- Implement custom only for radix/counting (integer keys) or external sort (data > RAM).

### Picking a graph traversal

- Pure connectivity or shortest hops: BFS.
- Topological order or cycle detection: DFS with color states.
- Weighted shortest path with non-negative weights: Dijkstra with `PriorityQueue<TNode, double>`.

## Quick Reference

- Use `Span<T>` slicing in recursion to avoid allocations.
- Pass comparers via `IComparer<T>` for custom orderings; avoid LINQ for hot paths.
- Prefer `PriorityQueue<TElement, TPriority>` (added in .NET 6) over hand-rolled heaps.
- Use `BitOperations.PopCount` and `TrailingZeroCount` for bit DP.
- For DP arrays, prefer `int[]` / `long[]` over `Dictionary<>`; cache locality wins.

## Common Pitfalls

- Off-by-one in binary search: prefer `lo + (hi - lo) / 2` and decide on `lo <= hi` vs `lo < hi` upfront.
- Using DFS recursion on million-node graphs: switch to iterative with `Stack<T>`.
- Dijkstra with negative weights: use Bellman-Ford instead.
- Forgetting to mark visited nodes leads to infinite loops on cyclic graphs.
- DP without checking state count: 2^30 states do not fit in RAM.
- Quicksort worst case on already-sorted input: pick a random or median-of-three pivot.

## Examples in this folder

- [Sorting.cs](./Sorting.cs) - quicksort, mergesort, heapsort.
- [BinarySearch.cs](./BinarySearch.cs) - canonical and lower-bound variants.
- [Bfs_Dfs.cs](./Bfs_Dfs.cs) - graph traversal templates.
- [Dijkstra.cs](./Dijkstra.cs) - shortest path with `PriorityQueue<,>`.
- [DP_Knapsack.cs](./DP_Knapsack.cs) - 0/1 and unbounded knapsack.
- [KMP.cs](./KMP.cs) - failure function and pattern matching.

## See also

- [DataStructures/](../DataStructures/README.md)
- [LeetCode/](../LeetCode/README.md)
- [.NET PriorityQueue docs](https://learn.microsoft.com/dotnet/api/system.collections.generic.priorityqueue-2)
