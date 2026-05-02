# Azure Key Vault

> Secrets, keys, certificates. Always behind managed identity, never client secrets in config.

## Three asset types

| Asset | Use |
|---|---|
| **Secret** | Connection strings, API keys |
| **Key** | Encryption keys (RSA/EC); HSM-backed available |
| **Certificate** | TLS / mTLS certs; auto-renew via configured CA |

## Auth model

- **Access policies** (legacy) → **RBAC** (modern, preferred)
- **Soft delete** ON by default (retains 90 days)
- **Purge protection** — irreversible; prevents accidental purge

## Quick Reference

```csharp
// Package: Azure.Security.KeyVault.Secrets, Azure.Identity
var client = new SecretClient(new Uri("https://my-kv.vault.azure.net"),
                              new DefaultAzureCredential());
KeyVaultSecret secret = await client.GetSecretAsync("Db-ConnectionString");
string value = secret.Value;
```

```csharp
// In ASP.NET Core 10:
builder.Configuration.AddAzureKeyVault(
    new Uri("https://my-kv.vault.azure.net"),
    new DefaultAzureCredential());

// Then read like any IConfiguration:
var conn = builder.Configuration["Db:ConnectionString"];
```

## Bicep snippet

```bicep
resource kv 'Microsoft.KeyVault/vaults@2024-04-01-preview' = {
  name: 'kv-orders-prod'
  location: location
  properties: {
    tenantId: subscription().tenantId
    sku: { family: 'A', name: 'standard' }
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: true
    publicNetworkAccess: 'Disabled'
    networkAcls: { defaultAction: 'Deny', bypass: 'AzureServices' }
  }
}
```

## Common Pitfalls

- Embedding KV URL but pulling secrets via shared client secret → defeats the point. Use managed identity.
- App restarts on every secret rotation — use `AddAzureKeyVault` with reload interval (`KeyVaultSecretManager`)
- Disabling soft delete / purge protection in prod → permanent data loss risk
- Public network on KV in prod → use private endpoints

## See also

- [../EntraId](../EntraId/) · [../../../Security/SecretsManagement](../../../Security/SecretsManagement/) · [../../../Security/DataProtection](../../../Security/DataProtection/)
