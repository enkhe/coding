// Liskov Substitution Principle
// A subtype must be usable wherever the supertype is expected without surprises.

namespace Architecture.SOLID.Lsp;

// BAD: Square inherits from Rectangle but breaks the invariant
// that setting Width is independent of Height.
public class Rectangle_Bad
{
    public virtual int Width  { get; set; }
    public virtual int Height { get; set; }
    public int Area => Width * Height;
}

public sealed class Square_Bad : Rectangle_Bad
{
    public override int Width  { get => base.Width; set { base.Width = value; base.Height = value; } }
    public override int Height { get => base.Height; set { base.Width = value; base.Height = value; } }
}

// GOOD: prefer composition + a common abstraction; do not pretend "is-a".
public interface IShape { int Area { get; } }

public readonly record struct Rectangle(int Width, int Height) : IShape
{
    public int Area => Width * Height;
}

public readonly record struct Square(int Side) : IShape
{
    public int Area => Side * Side;
}
