// Specification: encapsulate a predicate so it can be combined and reused.
// Pairs nicely with EF Core: convert specs to expression trees for SQL translation.

using System.Linq.Expressions;

namespace Architecture.DesignPatterns.Specification;

public sealed record Order(Guid Id, decimal Total, string Country, DateTimeOffset PlacedAt);

public abstract class Spec<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();
    public bool IsSatisfiedBy(T candidate) => ToExpression().Compile()(candidate);

    public static Spec<T> operator &(Spec<T> a, Spec<T> b) => new AndSpec<T>(a, b);
    public static Spec<T> operator |(Spec<T> a, Spec<T> b) => new OrSpec<T>(a, b);
}

public sealed class HighValueOrder(decimal min) : Spec<Order>
{
    public override Expression<Func<Order, bool>> ToExpression() => o => o.Total >= min;
}

public sealed class FromCountry(string code) : Spec<Order>
{
    public override Expression<Func<Order, bool>> ToExpression() => o => o.Country == code;
}

internal sealed class AndSpec<T>(Spec<T> a, Spec<T> b) : Spec<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        var x = a.ToExpression();
        var y = b.ToExpression();
        var p = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(Expression.Invoke(x, p), Expression.Invoke(y, p));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }
}

internal sealed class OrSpec<T>(Spec<T> a, Spec<T> b) : Spec<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        var x = a.ToExpression();
        var y = b.ToExpression();
        var p = Expression.Parameter(typeof(T));
        var body = Expression.OrElse(Expression.Invoke(x, p), Expression.Invoke(y, p));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }
}

// Usage:
// var spec = new HighValueOrder(1000m) & new FromCountry("US");
// var query = db.Orders.Where(spec.ToExpression());
