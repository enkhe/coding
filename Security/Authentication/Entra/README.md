# Microsoft Entra (ID + External ID)

> Workforce identity (Entra ID) and customer identity (Entra External ID). The .NET 2026 default.

## Core Concepts

- **Entra ID** (workforce) — your employees / partners. Conditional Access, MFA, SSO, role-based.
- **Entra External ID** (CIAM) — customer identity. Sign-up flows, social IdPs, custom domains, passkeys.
- **App registration** — your service in Entra. Has client ID, optional secret/cert, redirect URIs, scopes/roles.
- **Multi-tenant** — `common` or `organizations` authority lets users from any Entra tenant sign in.
- **Scopes vs App roles** — scopes are *delegated* (acting on behalf of user), app roles are *application* (service-to-service or RBAC).
- **Microsoft.Identity.Web** — the .NET wrapper that wires OIDC, MSAL, downstream token acquisition.

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Sign-in middleware | `services.AddMicrosoftIdentityWebApp(cfg)` |
| Protect a Web API | `services.AddMicrosoftIdentityWebApi(cfg)` |
| Call a downstream API | `.EnableTokenAcquisitionToCallDownstreamApi().AddDownstreamApi(...).AddInMemoryTokenCaches()` |
| Get a client | `IDownstreamApi.CallApiForUserAsync<T>(...)` |
| Require a scope | `[RequiredScope("Orders.Read")]` |
| Require a role | `[Authorize(Roles = "OrdersAdmin")]` |
| Conditional Access | enforced by Entra; your code surfaces `ClaimsChallenge` if needed |

## Quick Reference (web app)

```csharp
builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(["api://orders/Orders.Read"])
    .AddDownstreamApi("Orders", builder.Configuration.GetSection("DownstreamApis:Orders"))
    .AddInMemoryTokenCaches();

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
```

## Quick Reference (Web API)

```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(o =>
    o.AddPolicy("orders:read", p => p.RequireScope("Orders.Read")));

app.MapGet("/orders", () => Results.Ok())
   .RequireAuthorization("orders:read");
```

## appsettings.json

```jsonc
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "<tenant-guid-or-common>",
    "ClientId": "<client-id>",
    "ClientCertificates": [
      { "SourceType": "KeyVault", "KeyVaultUrl": "https://kv.vault.azure.net", "KeyVaultCertificateName": "app-cert" }
    ],
    "CallbackPath": "/signin-oidc",
    "Scopes": "openid profile offline_access User.Read"
  },
  "DownstreamApis": {
    "Orders": {
      "BaseUrl": "https://orders.example.com/",
      "Scopes": "api://orders/Orders.Read"
    }
  }
}
```

## Entra External ID specifics

- **User flows** — sign-up, sign-in, password reset; configured in portal.
- **Custom domains** for branding (`login.example.com`).
- **Passkeys** are a built-in factor.
- **Custom claims provider** (REST hook) to enrich tokens at issue time.
- **Different authority host** — `https://<tenant>.ciamlogin.com/<tenant>.onmicrosoft.com/v2.0`.

## Common Pitfalls

- Client secrets in config — use **certificates** or **managed identity** (Federated Identity Credentials).
- Skipping `EnableTokenAcquisitionToCallDownstreamApi` and trying to grab tokens manually.
- Putting scopes in `AzureAd:Scopes` AND on per-call `AddDownstreamApi` — first one wins inconsistently.
- Multi-tenant apps with no validation of the issuer — anyone's tenant can authenticate.
- Caching tokens in-memory only on multi-instance apps — use distributed cache.

## Examples in this folder

- [`Program.WebApp.cs`](Program.WebApp.cs) — sign-in + downstream API
- [`Program.WebApi.cs`](Program.WebApi.cs) — protected API with scopes
- [`appsettings.json`](appsettings.json) — full config

## See also

- [../OpenIdConnect](../OpenIdConnect/) · [../OAuth2](../OAuth2/) · [../../../Cloud/Azure/EntraId](../../../Cloud/Azure/EntraId/)
