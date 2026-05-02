// Value object: equality by value, immutable, behavior-bearing.

namespace Architecture.DDD;

public readonly record struct Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency) => new(0m, currency);

    public Money Add(Money other) => SameCurrency(other) with { Amount = Amount + other.Amount };
    public Money Subtract(Money other) => SameCurrency(other) with { Amount = Amount - other.Amount };
    public Money Multiply(int qty) => this with { Amount = Amount * qty };

    public static Money operator +(Money a, Money b) => a.Add(b);
    public static Money operator -(Money a, Money b) => a.Subtract(b);

    private Money SameCurrency(Money other) =>
        Currency == other.Currency
            ? this
            : throw new InvalidOperationException($"currency mismatch: {Currency} vs {other.Currency}");

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
