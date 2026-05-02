// Strongly-typed identifier — a value object that prevents passing the wrong Guid.

namespace Architecture.DDD;

public readonly record struct OrderId(Guid Value)
{
    public static OrderId New() => new(Guid.NewGuid());
    public override string ToString() => $"order_{Value:N}";
}

public readonly record struct CustomerId(Guid Value)
{
    public static CustomerId New() => new(Guid.NewGuid());
}
