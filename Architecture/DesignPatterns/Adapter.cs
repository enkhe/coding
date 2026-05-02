// Adapter: translate one interface into another that a client expects.
// Common at the boundary between your domain port and a vendor SDK.

namespace Architecture.DesignPatterns.Adapter;

// Our domain port — what our application wants:
public interface IPaymentGateway
{
    Task<string> ChargeAsync(decimal amount, string currency, string token, CancellationToken ct);
}

// Vendor SDK shape — what we are stuck with:
public sealed class LegacyStripeSdk
{
    public LegacyChargeResult CreateCharge(LegacyChargeRequest req) =>
        new(true, $"ch_{Guid.NewGuid():N}");
}

public sealed record LegacyChargeRequest(int AmountCents, string Currency, string SourceToken);
public sealed record LegacyChargeResult(bool Success, string Id);

// Adapter: maps domain port → vendor SDK.
public sealed class StripeAdapter(LegacyStripeSdk sdk) : IPaymentGateway
{
    public Task<string> ChargeAsync(decimal amount, string currency, string token, CancellationToken ct)
    {
        var result = sdk.CreateCharge(new LegacyChargeRequest((int)(amount * 100), currency, token));
        if (!result.Success) throw new InvalidOperationException("Charge failed");
        return Task.FromResult(result.Id);
    }
}
