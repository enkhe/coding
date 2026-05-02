// MinHeap.cs - generic binary heap (min-heap by default).
// Insert: O(log n). ExtractMin: O(log n). Peek: O(1). BuildHeap from list: O(n).
// Production: prefer System.Collections.Generic.PriorityQueue<TElement, TPriority>.

using System;
using System.Collections.Generic;

namespace AlgorithmsAndDataStructures.DataStructures;

public sealed class MinHeap<T>
{
    private readonly List<T> _data = new();
    private readonly IComparer<T> _cmp;

    public MinHeap(IComparer<T>? cmp = null) => _cmp = cmp ?? Comparer<T>.Default;

    public int Count => _data.Count;

    public void Push(T value)
    {
        _data.Add(value);
        SiftUp(_data.Count - 1);
    }

    public T Peek()
    {
        if (_data.Count == 0) throw new InvalidOperationException("empty");
        return _data[0];
    }

    public T Pop()
    {
        if (_data.Count == 0) throw new InvalidOperationException("empty");
        T top = _data[0];
        int last = _data.Count - 1;
        _data[0] = _data[last];
        _data.RemoveAt(last);
        if (_data.Count > 0) SiftDown(0);
        return top;
    }

    private void SiftUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (_cmp.Compare(_data[i], _data[parent]) >= 0) break;
            (_data[i], _data[parent]) = (_data[parent], _data[i]);
            i = parent;
        }
    }

    private void SiftDown(int i)
    {
        int n = _data.Count;
        while (true)
        {
            int left = 2 * i + 1, right = 2 * i + 2, smallest = i;
            if (left < n && _cmp.Compare(_data[left], _data[smallest]) < 0) smallest = left;
            if (right < n && _cmp.Compare(_data[right], _data[smallest]) < 0) smallest = right;
            if (smallest == i) return;
            (_data[i], _data[smallest]) = (_data[smallest], _data[i]);
            i = smallest;
        }
    }
}
