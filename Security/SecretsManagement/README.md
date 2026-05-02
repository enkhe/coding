# Secrets Management

> Secrets in vaults, identities for access, rotation on a schedule. Never in source control or app config.

## Where secrets go

| Environment | Where |
|---|---|
| Local dev | `dotnet user-secrets` (per-project, outside repo) |
| CI/CD | Pipeline secrets (GitHub Encrypted Secrets, AzDo Library) |
| Cloud runtime | **Key Vault** / Secrets Manager / HashiCorp Vault |

## Identity for access

- **Managed Identity** (Azure) / **IRSA** (AWS EKS) / **Workload Identity** (GKE) — preferred
- **OIDC** from CI to cloud — short-lived tokens, no static keys
- **Federated credentials** (Entra) for GitHub Actions / GitLab / Bitbucket

## .NET 10 patterns

```csharp
// Local dev
// dotnet user-secrets init
// dotnet user-secrets set "Db:ConnectionString" "..."

// Production — Key Vault
builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["KeyVault:Uri"]!),
    new DefaultAzureCredential());

// Read like normal config
var conn = builder.Configuration["Db:ConnectionString"];
```

## Rotation discipline

| Asset | Rotate every |
|---|---|
| API keys (3rd-party) | 90 days |
| DB passwords | 90 days (or use managed identity → no password) |
| Signing keys (data protection) | 90 days (auto via DataProtection) |
| Certificates | Before expiry; auto via cert manager / KV |

## Common Pitfalls

- `appsettings.Production.json` checked in with secrets
- Sharing one service principal across multiple apps → blast radius
- Long-lived `AZURE_CREDENTIALS` GitHub secret → use OIDC instead
- Logging configuration values → leak via logs

## See also

- [../DataProtection](../DataProtection/) · [../Authentication/Entra](../Authentication/Entra/) · [../../Cloud/Azure/KeyVault](../../Cloud/Azure/KeyVault/)
