# OWASP Top 10 — 2021 (with .NET 10 mitigations)

| # | Risk | Mitigations |
|---|---|---|
| **A01** | Broken Access Control | Default-deny (`FallbackPolicy`); per-endpoint policies; row-level security; never trust client roles |
| **A02** | Cryptographic Failures | TLS 1.2+; AES-GCM; never MD5/SHA-1 for security; KDFs for passwords; HSTS |
| **A03** | Injection (SQL, XSS, etc.) | EF Core / parameterized queries; output encoding (Razor encodes by default); CSP for XSS |
| **A04** | Insecure Design | Threat model per bounded context; rate limiting; abuse cases |
| **A05** | Security Misconfiguration | Default-deny CORS; secure cookies; redact errors in prod (`UseExceptionHandler`); minimal images |
| **A06** | Vulnerable Components | Dependabot / Renovate; SBOM; pin versions; restricted feeds |
| **A07** | Identification & AuthN Failures | OIDC + PKCE; passkeys; no plaintext passwords; lockout after N tries |
| **A08** | Software & Data Integrity | Signed packages (cosign); locked deps; verify upstream attestations; SRI for CDN scripts |
| **A09** | Logging & Monitoring | Structured logs; centralized + searchable; alerts on auth failures, errors, anomalies |
| **A10** | SSRF | Validate outbound URLs; block private IPs from user-supplied URLs; outbound proxy allow-list |

## .NET 10 specifics

```csharp
// Anti-forgery (default for cookies-auth)
app.UseAntiforgery();

// HSTS + HTTPS redirect
app.UseHsts();
app.UseHttpsRedirection();

// Security headers
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    ctx.Response.Headers.Append("X-Frame-Options", "DENY");
    ctx.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    ctx.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; script-src 'self'; object-src 'none'; frame-ancestors 'none'");
    await next();
});

// Request body size limit (DoS)
builder.Services.Configure<KestrelServerOptions>(o => o.Limits.MaxRequestBodySize = 1_048_576); // 1 MB

// Rate limiting (A05 + DoS)
builder.Services.AddRateLimiter(o =>
    o.AddFixedWindowLimiter("default", c => { c.PermitLimit = 100; c.Window = TimeSpan.FromSeconds(10); }));
```

## Common Pitfalls

- Trusting `X-Forwarded-For` without configuring forwarded-headers middleware
- Using `string.Format` for SQL — parameterize
- Logging full request bodies → token leakage
- CORS = `*` in prod
- Custom auth schemes — almost always wrong; use OIDC

## See also

- [../ThreatModeling](../ThreatModeling/) · [../SupplyChain](../SupplyChain/) · [../ZeroTrust](../ZeroTrust/)
