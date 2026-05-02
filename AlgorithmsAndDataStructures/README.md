# Algorithms and Data Structures

> Master index and complexity cheatsheet for the algorithms, data structures, system design, and competitive-programming sections of the .NET 2026 senior/architect roadmap.

## Core Concepts

- **Algorithm analysis**: time and space complexity expressed in Big-O (worst), Big-Theta (tight), Big-Omega (best). Amortized analysis for dynamic structures (e.g., `List<T>` resize).
- **Trade-off triangle**: time vs. space vs. code complexity. Pick the structure that matches your dominant operation.
- **Data structure mental model**: every structure is a contract `(operations, invariants, complexity)`. Know the BCL equivalent for each one.
- **Algorithm families**: divide-and-conquer, dynamic programming, greedy, backtracking, graph traversal, string matching, bit manipulation.
- **System design lens**: at scale, the right data structure becomes the right service (KV store, queue, cache, search index).

## "To Be Dangerous" Cheatsheet

### Big-O of common operations

| Structure                | Access | Search   | Insert   | Delete   | Notes                                  |
| ------------------------ | ------ | -------- | -------- | -------- | -------------------------------------- |
| Array / `T[]`            | O(1)   | O(n)     | O(n)     | O(n)     | Contiguous, cache friendly             |
| `List<T>`                | O(1)   | O(n)     | O(1)*    | O(n)     | Amortized append, doubling             |
| `LinkedList<T>`          | O(n)   | O(n)     | O(1)     | O(1)     | Pointer-heavy, poor cache locality     |
| `Stack<T>` / `Queue<T>`  | O(1)   | O(n)     | O(1)     | O(1)     | LIFO / FIFO                            |
| `Dictionary<K,V>`        | O(1)*  | O(1)*    | O(1)*    | O(1)*    | Hash map, worst case O(n)              |
| `HashSet<T>`             | -      | O(1)*    | O(1)*    | O(1)*    | Set semantics                          |
| `SortedDictionary<K,V>`  | O(log n) | O(log n) | O(log n) | O(log n) | Red-black tree                       |
| `SortedSet<T>`           | -      | O(log n) | O(log n) | O(log n) | Range queries                          |
| `PriorityQueue<E,P>`     | O(1)   | O(n)     | O(log n) | O(log n) | Min-heap by default                    |
| Trie                     | -      | O(m)     | O(m)     | O(m)     | m = key length                         |
| BST (balanced)           | O(log n) | O(log n) | O(log n) | O(log n) | AVL, Red-Black                       |
| B-Tree                   | O(log n) | O(log n) | O(log n) | O(log n) | DB indexes (high fan-out)              |
| Disjoint Set Union       | -      | O(alpha(n)) | O(alpha(n)) | -    | Near constant with path compression    |
| Bloom Filter             | -      | O(k)     | O(k)     | -        | Probabilistic, no deletes              |

\* amortized average.

### Big-O of common algorithms

| Algorithm           | Time             | Space    | Stable | Notes                              |
| ------------------- | ---------------- | -------- | ------ | ---------------------------------- |
| Bubble / Insertion  | O(n^2)           | O(1)     | yes    | Simple, fine for tiny n            |
| Merge sort          | O(n log n)       | O(n)     | yes    | Linked lists, external sort        |
| Quick sort          | O(n log n) avg   | O(log n) | no     | In place, cache friendly           |
| Heap sort           | O(n log n)       | O(1)     | no     | No worst-case quadratic            |
| Counting / Radix    | O(n + k)         | O(n + k) | yes    | Integer keys, bounded range        |
| Binary search       | O(log n)         | O(1)     | -      | Sorted input only                  |
| BFS                 | O(V + E)         | O(V)     | -      | Shortest path on unweighted        |
| DFS                 | O(V + E)         | O(V)     | -      | Topological sort, cycle detection  |
| Dijkstra (heap)     | O((V + E) log V) | O(V)     | -      | Non-negative weights               |
| Bellman-Ford        | O(V * E)         | O(V)     | -      | Handles negative edges             |
| Floyd-Warshall      | O(V^3)           | O(V^2)   | -      | All-pairs shortest                 |
| KMP / Rabin-Karp    | O(n + m)         | O(m)     | -      | String search                      |
| 0/1 Knapsack DP     | O(n * W)         | O(n * W) | -      | Pseudo-polynomial                  |

## Quick Reference

- Default `Dictionary<,>` to lookups, `HashSet<>` to membership, `List<>` to ordered collections, `PriorityQueue<,>` to scheduling.
- Reach for `SortedSet<>` / `SortedDictionary<,>` only when you need ordered iteration or range queries.
- Use `Span<T>` and `ReadOnlySpan<T>` for hot-path slicing without allocation.
- Use `ArrayPool<T>.Shared` for reusable buffers.
- Prefer iterative DFS with explicit stack to recursive DFS to avoid stack overflow on deep graphs.
- Memoize first, then convert to bottom-up DP only if you need to.

## Common Pitfalls

- Quoting average-case Big-O without noting worst case (hash table degenerate to O(n)).
- Using `LinkedList<T>` "because it's O(1) insert" while ignoring cache misses; `List<T>` almost always wins.
- Forgetting that `Dictionary<,>` enumeration order is *not* guaranteed across runtimes.
- Using `BinaryFormatter` (removed in .NET 10) or unsafe deserialization for graph persistence.
- Mixing reference equality with value equality in custom keys; override `GetHashCode` and `Equals` together.
- Treating recursion depth as free; CLR default stack is 1 MB.

## Examples in this folder

- [Algorithms/](./Algorithms/README.md) - sorting, searching, graph, DP, string, bit.
- [DataStructures/](./DataStructures/README.md) - generic C# implementations of the classics.
- [SystemDesign/](./SystemDesign/README.md) - architecture primitives and design briefs.
- [LeetCode/](./LeetCode/README.md) - curated practice path with sample solutions.
- [HackerRank/](./HackerRank/README.md) - domain progression and submissions.
- [CodeSignal/](./CodeSignal/README.md) - assessment formats and time-boxing tactics.

## See also

- [.NET 2026 senior/architect roadmap](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [.NET runtime collections source](https://github.com/dotnet/runtime/tree/main/src/libraries/System.Collections)
- *Introduction to Algorithms* (CLRS), *Designing Data-Intensive Applications* (Kleppmann).
