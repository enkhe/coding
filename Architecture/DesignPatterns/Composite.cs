// Composite: treat individual objects and groups uniformly.
// Canonical fit: file systems, scene graphs, AST nodes.

namespace Architecture.DesignPatterns.Composite;

public abstract record Node(string Name)
{
    public abstract long Size { get; }
}

public sealed record FileNode(string Name, long Bytes) : Node(Name)
{
    public override long Size => Bytes;
}

public sealed record DirectoryNode(string Name, IReadOnlyList<Node> Children) : Node(Name)
{
    public override long Size => Children.Sum(c => c.Size);
}

// Usage:
// var tree = new DirectoryNode("root", [
//     new FileNode("a.txt", 100),
//     new DirectoryNode("sub", [ new FileNode("b.txt", 250) ])
// ]);
// Console.WriteLine(tree.Size); // 350
