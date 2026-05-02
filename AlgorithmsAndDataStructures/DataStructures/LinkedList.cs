// LinkedList.cs - generic doubly linked list with sentinel head/tail.
// Educational; in production use System.Collections.Generic.LinkedList<T>.

using System;
using System.Collections;
using System.Collections.Generic;

namespace AlgorithmsAndDataStructures.DataStructures;

public sealed class DoublyLinkedList<T> : IEnumerable<T>
{
    public sealed class Node
    {
        public T Value;
        public Node? Prev;
        public Node? Next;
        public Node(T value) => Value = value;
    }

    private readonly Node _head = new(default!); // sentinel
    private readonly Node _tail = new(default!); // sentinel
    public int Count { get; private set; }

    public DoublyLinkedList()
    {
        _head.Next = _tail;
        _tail.Prev = _head;
    }

    // O(1) - add to end.
    public Node AddLast(T value)
    {
        var node = new Node(value);
        InsertBefore(_tail, node);
        return node;
    }

    // O(1) - add to front.
    public Node AddFirst(T value)
    {
        var node = new Node(value);
        InsertBefore(_head.Next!, node);
        return node;
    }

    // O(1) - remove a known node.
    public void Remove(Node node)
    {
        if (node == _head || node == _tail) throw new ArgumentException("sentinel");
        node.Prev!.Next = node.Next;
        node.Next!.Prev = node.Prev;
        node.Prev = node.Next = null;
        Count--;
    }

    // O(n) - remove first occurrence by value.
    public bool Remove(T value, IEqualityComparer<T>? cmp = null)
    {
        cmp ??= EqualityComparer<T>.Default;
        for (var n = _head.Next; n != _tail; n = n!.Next)
            if (cmp.Equals(n!.Value, value))
            {
                Remove(n);
                return true;
            }
        return false;
    }

    private void InsertBefore(Node next, Node node)
    {
        node.Prev = next.Prev;
        node.Next = next;
        next.Prev!.Next = node;
        next.Prev = node;
        Count++;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (var n = _head.Next; n != _tail; n = n!.Next)
            yield return n!.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
