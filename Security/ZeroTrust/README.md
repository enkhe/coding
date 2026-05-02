# Zero Trust

> "Never trust, always verify." Network position is not authentication.

## Core Principles

1. **Verify explicitly** — every request authenticated and authorized, not just at the edge.
2. **Use least privilege** — just-enough, just-in-time access; default deny.
3. **Assume breach** — design as if the network is hostile; minimize blast radius.

## "To Be Dangerous" Cheatsheet

| Concern | Implementation |
|---|---|
| User identity | OIDC + MFA / Passkeys + Conditional Access |
| Service identity | Managed identity / workload identity (no shared secrets) |
| East-west traffic | mTLS (service mesh: Linkerd, Istio, or platform-managed) |
| Authorization | Per-endpoint policies + RBAC/ABAC at data layer (RLS, ABAC tags) |
| Network | Private endpoints, no public ingress for internal services |
| Secrets | Key Vault + managed identity; rotate; audit access |
| Devices | Conditional Access checks compliance + posture |
| Logging | Append-only, tamper-evident, exported off-host |
| Just-in-time | PIM/PAM elevation with approval + auto-revocation |

## Application of zero trust to ASP.NET Core 10

```csharp
// Always authenticate, even between internal services.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = builder.Configuration["Identity:Authority"];
        o.Audience = builder.Configuration["Identity:Audience"];
    });

builder.Services.AddAuthorization(o =>
{
    o.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();    // every endpoint requires auth unless [AllowAnonymous]
});

// mTLS to downstream:
builder.Services.AddHttpClient("payments")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        SslOptions = new()
        {
            ClientCertificates = new(){ /* loaded from KV */ },
            RemoteCertificateValidationCallback = (sender, cert, chain, errs) =>
                cert is not null && IsTrustedFingerprint(cert.Thumbprint!),
        }
    });
```

## What zero trust is **not**

- "Just put a firewall around it"
- "VPN = secure"
- A product you buy in one shot — it's an architectural posture across identity, network, app, data, and ops

## Common Pitfalls

- Internal-only API → no auth → first lateral move owns it
- Service principals as "spray and pray" — too many scopes, never rotated
- mTLS for ingress only, plaintext east-west
- Logs that the attacker can edit — push to write-only sinks

## Examples in this folder

- [`fallback-policy.cs`](fallback-policy.cs) — default-deny ASP.NET Core auth
- [`mtls-client.cs`](mtls-client.cs) — mTLS HttpClient

## See also

- [../ThreatModeling](../ThreatModeling/) · [../Authentication/Entra](../Authentication/Entra/) · [../DataProtection](../DataProtection/)
