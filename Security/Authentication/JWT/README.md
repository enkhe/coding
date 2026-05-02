# JSON Web Token (JWT)

> Compact, URL-safe, signed (and optionally encrypted) token format.

## Core Concepts

A JWT has three base64url-encoded parts joined by dots:

```
header.payload.signature
```

- **Header**: `{"alg":"RS256","kid":"abc","typ":"JWT"}` - which key/algorithm.
- **Payload (claims)**: `iss`, `sub`, `aud`, `exp`, `nbf`, `iat`, `jti`, plus app claims.
- **Signature**: cryptographic signature over `header.payload`.

JWS = signed; JWE = encrypted (rarer). "JWT" usually means JWS.

## Signing Algorithms

| Alg | Type | Use |
| --- | --- | --- |
| `RS256` | RSA-SHA256 | Default for federated; clients verify via JWKS |
| `ES256` | ECDSA P-256 | Modern, smaller, fast |
| `EdDSA` | Ed25519 | Newer; if your stack supports |
| `HS256` | HMAC-SHA256 | Symmetric; same-org only, short-lived |
| `none` | none | Never. Reject in code. |

## "To Be Dangerous" Cheatsheet

- **Always validate**: `iss`, `aud`, `exp`, `nbf`, signature, `kid` against JWKS.
- Allow tiny clock skew (1-2 min), no more.
- **Reject `alg=none`** and **algorithm confusion** (HS using public key as secret).
- Use **JWKS** with caching + auto-refresh. Microsoft.IdentityModel handles this.
- Treat tokens as **bearer secrets**: HTTPS only, never log raw tokens.
- For sensitive ops, require **token binding** (DPoP, mTLS) or step-up auth.

## Typical Validation in ASP.NET Core 10

```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.Authority = "https://issuer/";
        opts.Audience = "api://orders";
        opts.MapInboundClaims = false;
        opts.TokenValidationParameters.ValidIssuer = "https://issuer/";
        opts.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(1);
        opts.TokenValidationParameters.ValidAlgorithms = ["RS256", "ES256"];
        opts.RequireHttpsMetadata = true;
    });
```

## Quick Reference

| Claim | Meaning |
| --- | --- |
| `iss` | Issuer |
| `sub` | Subject (stable user id) |
| `aud` | Audience (target API) |
| `exp` | Expiration (unix seconds) |
| `nbf` | Not-before |
| `iat` | Issued at |
| `jti` | JWT ID (replay defense) |
| `nonce` | OIDC ID token only |
| `azp` | Authorized party (client_id) |
| `scp`/`scope` | Granted scopes |

## Common Pitfalls

- Validating signature but not `aud`/`iss` -> token confusion.
- Trusting any claim from a token your app issued without re-checking expiry.
- Long-lived JWTs (>1h) without revocation - prefer short access + refresh.
- Putting PII in claims unnecessarily - tokens leak in logs.
- Using HS256 with leaked secret in client-side code.

## Examples in this folder

- [JwtValidationExample.cs](./JwtValidationExample.cs) - manual JWKS-based validation
- [DontDoThis.cs](./DontDoThis.cs) - what NOT to do, with annotations

## See also

- [OpenIdConnect](../OpenIdConnect/README.md)
- [OAuth2](../OAuth2/README.md)
- [RFC 7519 - JWT](https://datatracker.ietf.org/doc/html/rfc7519)
- [JWT BCP - RFC 8725](https://datatracker.ietf.org/doc/html/rfc8725)
