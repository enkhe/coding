// Open/Closed Principle
// Add behavior by adding code, not editing existing code.

namespace Architecture.SOLID.Ocp;

public sealed record Order(decimal Subtotal, string CustomerTier);

// BAD: every new tier edits the same switch.
public static class Pricing_Bad
{
    public static decimal Total(Order o) => o.CustomerTier switch
    {
        "Gold"   => o.Subtotal * 0.90m,
        "Silver" => o.Subtotal * 0.95m,
        _        => o.Subtotal,
    };
}

// GOOD: strategies are added without touching existing code.
public interface IDiscountStrategy
{
    bool AppliesTo(Order order);
    decimal Apply(decimal subtotal);
}

public sealed class GoldDiscount   : IDiscountStrategy { public bool AppliesTo(Order o) => o.CustomerTier == "Gold";   public decimal Apply(decimal s) => s * 0.90m; }
public sealed class SilverDiscount : IDiscountStrategy { public bool AppliesTo(Order o) => o.CustomerTier == "Silver"; public decimal Apply(decimal s) => s * 0.95m; }

public sealed class PriceCalculator(IEnumerable<IDiscountStrategy> strategies)
{
    public decimal Total(Order o) =>
        strategies.FirstOrDefault(s => s.AppliesTo(o))?.Apply(o.Subtotal) ?? o.Subtotal;
}
