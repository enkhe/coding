# Data Structures

> Generic C# (.NET 10) implementations of the data structures every senior should be able to whiteboard, with notes on the BCL equivalents you should reach for in production.

## Core Concepts

- **Linear**: array, dynamic array (`List<T>`), linked list (singly/doubly), stack, queue, deque (`LinkedList<T>` or `Deque<T>` library).
- **Associative**: hash map (`Dictionary<,>`), set (`HashSet<>`), sorted map / set (`SortedDictionary<,>`, `SortedSet<>`).
- **Trees**: BST, self-balancing (AVL, Red-Black, Treap), B-Tree (DB indexes), segment tree, Fenwick / BIT.
- **Tries**: prefix tree for strings; radix / Patricia trie for compact storage; suffix trie for substring queries.
- **Heaps**: binary heap (`PriorityQueue<,>`), d-ary, Fibonacci (better amortized decrease-key), pairing heap.
- **Graphs**: adjacency list (sparse), adjacency matrix (dense, O(V^2) memory).
- **Specialized**: disjoint-set union (DSU/Union-Find), bloom filter (probabilistic membership), count-min sketch, LRU cache, skip list.

## "To Be Dangerous" Cheatsheet

| Need                                  | Reach for                          | BCL equivalent                          |
| ------------------------------------- | ---------------------------------- | --------------------------------------- |
| Random access by index                | Array / `List<T>`                  | `T[]`, `List<T>`                        |
| O(1) insert at both ends              | Doubly linked list / deque         | `LinkedList<T>`                         |
| LIFO                                  | Stack                              | `Stack<T>`                              |
| FIFO                                  | Queue                              | `Queue<T>`                              |
| Key lookup                            | Hash map                           | `Dictionary<TKey, TValue>`              |
| Membership                            | Hash set                           | `HashSet<T>`                            |
| Sorted iteration / range scan         | Balanced BST                       | `SortedSet<T>`, `SortedDictionary<,>`   |
| Top-K / scheduling                    | Binary heap                        | `PriorityQueue<TElement, TPriority>`    |
| Prefix queries on strings             | Trie                               | (custom)                                |
| Connectivity / cycles                 | DSU                                | (custom)                                |
| Probabilistic membership (no false neg) | Bloom filter                     | (custom)                                |
| Bounded LRU cache                     | Doubly linked list + hash map      | `MemoryCache` (different semantics)     |
| Concurrent dictionary                 | Lock-striped hash map              | `ConcurrentDictionary<,>`               |
| Concurrent queue                      | Multi-producer queue               | `ConcurrentQueue<T>`, `Channel<T>`      |

### Hash map collision handling

- .NET `Dictionary<,>` uses separate chaining with prime-sized buckets.
- Worst case is O(n) when hash codes collide; ensure `GetHashCode` is well distributed.
- For randomized hash protection, the BCL uses `RandomizedStringEqualityComparer` for strings under attack.

### Picking a tree

- **AVL**: stricter balance (height differences <= 1), faster lookups, slower mutations. Good for read-heavy.
- **Red-Black**: looser balance, faster mutations. Used by `SortedDictionary<,>`.
- **B-Tree**: high fan-out, optimized for disk pages. Used by SQL Server / Postgres indexes.
- **Treap / Skip list**: simpler to implement, randomized balance.

## Quick Reference

- Override `Equals` and `GetHashCode` together; use `HashCode.Combine` (NET 6+).
- `EqualityComparer<T>.Default` uses `IEquatable<T>` if implemented.
- Use `record` types for immutable value-equality semantics.
- For thread-safe shared state, prefer immutable structures (`ImmutableDictionary<,>`) or channels over locking.

## Common Pitfalls

- Mutating a key after inserting it into a hash-based collection (lost lookups).
- Sharing `Dictionary<,>` across threads without a lock - use `ConcurrentDictionary<,>`.
- Building trie with `Dictionary<char, Node>` per node when input alphabet is small - waste of allocations.
- Using `Queue<T>` for priority work; you need `PriorityQueue<,>`.
- Forgetting that `SortedDictionary<,>` is O(log n), not O(1).

## Examples in this folder

- [LinkedList.cs](./LinkedList.cs) - doubly linked list with sentinel.
- [Trie.cs](./Trie.cs) - prefix tree with insert/search/startsWith.
- [MinHeap.cs](./MinHeap.cs) - generic binary heap.
- [Graph.cs](./Graph.cs) - adjacency list with BFS/DFS helpers.
- [LruCache.cs](./LruCache.cs) - hash + doubly linked list, O(1) operations.
- [DisjointSet.cs](./DisjointSet.cs) - union by rank + path compression.
- [BloomFilter.cs](./BloomFilter.cs) - k hash functions, m-bit array.

## See also

- [Algorithms/](../Algorithms/README.md)
- [.NET collections](https://learn.microsoft.com/dotnet/standard/collections/)
- [System.Collections.Immutable](https://learn.microsoft.com/dotnet/api/system.collections.immutable)
