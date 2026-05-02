// Manual JWT validation against a JWKS endpoint. In ASP.NET Core 10 you would
// normally just `AddJwtBearer`, but knowing the moving parts pays off when
// debugging or building custom token validators.
//
// dotnet add package Microsoft.IdentityModel.Protocols.OpenIdConnect
// dotnet add package Microsoft.IdentityModel.Tokens

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

public sealed class JwtValidator
{
    private readonly ConfigurationManager<OpenIdConnectConfiguration> _configManager;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtValidator(string authority, string audience)
    {
        _issuer = authority.TrimEnd('/');
        _audience = audience;

        _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"{_issuer}/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever { RequireHttps = true })
        {
            AutomaticRefreshInterval = TimeSpan.FromHours(24),
            RefreshInterval = TimeSpan.FromMinutes(5)
        };
    }

    public async Task<ClaimsPrincipal> ValidateAsync(string token, CancellationToken ct = default)
    {
        var config = await _configManager.GetConfigurationAsync(ct);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,

            ValidateAudience = true,
            ValidAudience = _audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),

            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = config.SigningKeys,

            // Defence-in-depth: lock down algorithms.
            ValidAlgorithms = ["RS256", "ES256"],

            RequireSignedTokens = true,
            RequireExpirationTime = true,

            NameClaimType = "name",
            RoleClaimType = "roles"
        };

        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var principal = handler.ValidateToken(token, parameters, out var validated);

        // Extra hardening: check `kid` matched a current key, not a stale one.
        if (validated is JwtSecurityToken jwt && config.SigningKeys.All(k => k.KeyId != jwt.Header.Kid))
            throw new SecurityTokenInvalidSigningKeyException("kid not found in JWKS");

        return principal;
    }
}
