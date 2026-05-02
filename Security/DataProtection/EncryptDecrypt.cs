// Use IDataProtector for short-lived payloads (email confirmation, password reset).
using Microsoft.AspNetCore.DataProtection;

namespace Security.DataProtection;

public sealed class EmailTokenService(IDataProtectionProvider provider)
{
    private readonly ITimeLimitedDataProtector _protector =
        provider.CreateProtector("Email.Tokens.v1").ToTimeLimitedDataProtector();

    public string CreateConfirmationToken(Guid userId, TimeSpan ttl)
    {
        var payload = $"{userId:N}|{Guid.NewGuid():N}";
        return _protector.Protect(payload, ttl);
    }

    public bool TryConsume(string token, out Guid userId)
    {
        userId = default;
        try
        {
            var payload = _protector.Unprotect(token);
            var parts = payload.Split('|');
            return parts.Length == 2 && Guid.TryParseExact(parts[0], "N", out userId);
        }
        catch (System.Security.Cryptography.CryptographicException)
        {
            // expired, tampered, or wrong purpose
            return false;
        }
    }
}
