// Custom IClaimsTransformation - enrich the principal after AuthN, e.g. attach
// permissions resolved from your own store. Runs on every authenticated request,
// so cache aggressively.

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;

public sealed class AppClaimsTransformer(IMemoryCache cache) : IClaimsTransformation
{
    private const string TransformedFlag = "ext:transformed";

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return Task.FromResult(principal);

        // Idempotent: avoid duplicate claims on every request.
        if (identity.HasClaim(c => c.Type == TransformedFlag))
            return Task.FromResult(principal);

        var subject = principal.FindFirst("sub")?.Value
                      ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (subject is null) return Task.FromResult(principal);

        var permissions = cache.GetOrCreate(
            $"perms:{subject}",
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                // pretend we hit a DB / API
                return new[] { "orders:read", "orders:write" };
            }) ?? [];

        foreach (var p in permissions)
            identity.AddClaim(new Claim("permission", p));

        identity.AddClaim(new Claim(TransformedFlag, "1"));
        return Task.FromResult(principal);
    }
}
