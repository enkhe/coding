// Trie.cs - prefix tree for ASCII / Unicode strings.
// Operations: O(m) where m = key length. Space O(total chars * alphabet).

using System.Collections.Generic;

namespace AlgorithmsAndDataStructures.DataStructures;

public sealed class Trie
{
    private sealed class Node
    {
        public Dictionary<char, Node> Children = new();
        public bool IsTerminal;
    }

    private readonly Node _root = new();

    public void Insert(string word)
    {
        var node = _root;
        foreach (char c in word)
        {
            if (!node.Children.TryGetValue(c, out var next))
            {
                next = new Node();
                node.Children[c] = next;
            }
            node = next;
        }
        node.IsTerminal = true;
    }

    public bool Search(string word) => Find(word) is { IsTerminal: true };

    public bool StartsWith(string prefix) => Find(prefix) is not null;

    private Node? Find(string s)
    {
        var node = _root;
        foreach (char c in s)
        {
            if (!node.Children.TryGetValue(c, out var next)) return null;
            node = next;
        }
        return node;
    }
}
