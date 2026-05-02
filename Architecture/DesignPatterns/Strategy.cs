// Strategy: encapsulate interchangeable algorithms behind a common port.
// In modern .NET, prefer keyed services or a delegate over hand-rolled registries.

namespace Architecture.DesignPatterns.Strategy;

public sealed record Parcel(decimal WeightKg, string Destination);

public interface IShipping
{
    Task<decimal> QuoteAsync(Parcel parcel, CancellationToken ct);
}

public sealed class FedExShipping : IShipping
{
    public Task<decimal> QuoteAsync(Parcel p, CancellationToken ct) =>
        Task.FromResult(5m + p.WeightKg * 1.20m);
}

public sealed class UpsShipping : IShipping
{
    public Task<decimal> QuoteAsync(Parcel p, CancellationToken ct) =>
        Task.FromResult(4m + p.WeightKg * 1.35m);
}

public sealed class ShippingQuoter(IServiceProvider sp)
{
    public Task<decimal> QuoteAsync(Parcel p, string carrier, CancellationToken ct) =>
        sp.GetRequiredKeyedService<IShipping>(carrier).QuoteAsync(p, ct);
}

// Composition root:
// services.AddKeyedScoped<IShipping, FedExShipping>("fedex");
// services.AddKeyedScoped<IShipping, UpsShipping>("ups");

internal static class KeyedServiceShim
{
    public static T GetRequiredKeyedService<T>(this IServiceProvider sp, object key) =>
        throw new NotImplementedException("Provided by Microsoft.Extensions.DependencyInjection 8+");
}
