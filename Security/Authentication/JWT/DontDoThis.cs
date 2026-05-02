// Anti-patterns. Do not ship any of this. Each block is annotated with the
// specific failure mode.

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

public static class DontDoThis
{
    // BAD #1 - decoding without validation. ReadJwtToken does NO signature/issuer/audience checks.
    public static void DecodeOnly(string token)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Console.WriteLine(jwt.Subject); // attacker-controlled value, treated as truth.
    }

    // BAD #2 - HS256 with hardcoded secret. Anyone with the binary can mint tokens.
    public static void HardcodedHmac()
    {
        var key = new SymmetricSecurityKey("super-secret-shared"u8.ToArray());
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        _ = creds; // ...issue tokens here. Do not.
    }

    // BAD #3 - allowing `none` algorithm.
    public static TokenValidationParameters AcceptingNone() => new()
    {
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidAlgorithms = ["none"], // catastrophic; reject any unsigned token.
    };

    // BAD #4 - skipping audience validation. Token issued for service A is accepted by service B.
    public static TokenValidationParameters NoAudience() => new()
    {
        ValidateIssuer = true,
        ValidateAudience = false, // token confusion attacks become trivial.
        ValidateLifetime = true,
    };

    // BAD #5 - huge clock skew "to be safe". An expired-by-an-hour token is still valid.
    public static TokenValidationParameters HugeSkew() => new()
    {
        ClockSkew = TimeSpan.FromHours(1),
    };

    // BAD #6 - storing JWT in localStorage in a SPA -> XSS exfiltration. Use http-only cookie + BFF.
}
