# Data Protection

> ASP.NET Core Data Protection — encrypt cookies, anti-forgery tokens, and short-lived bearer payloads. Get the key ring right or your sessions die at restart.

## Core Concepts

- **Key ring** — set of keys; current one signs/encrypts, older ones still verify. Rotates automatically (default 90 days).
- **Persistence** — keys must persist across instances and restarts (Blob Storage, Redis, file share).
- **Protection** — keys at rest must be encrypted (Key Vault, DPAPI, certificate).
- **Purposes** — string used to derive a sub-key, isolating different uses (`"cookies"`, `"forgery"`).
- **Time-limited protection** — `ITimeLimitedDataProtector` for short-lived payloads (password reset tokens).

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Single instance dev | `services.AddDataProtection()` |
| Multi-instance | `.PersistKeysToAzureBlobStorage(blobUri)` (or Redis, file share) |
| Encrypt at rest | `.ProtectKeysWithAzureKeyVault(keyId, credential)` |
| App name (key isolation) | `.SetApplicationName("orders")` |
| Use protector | `var p = provider.CreateProtector("Email.Tokens"); p.Protect(...)` |
| Time-limited | `p.ToTimeLimitedDataProtector().Protect(payload, TimeSpan.FromHours(1))` |

## Quick Reference

```csharp
builder.Services.AddDataProtection()
    .SetApplicationName("orders")
    .PersistKeysToAzureBlobStorage(
        new Uri(builder.Configuration["DataProtection:BlobUri"]!),
        new DefaultAzureCredential())
    .ProtectKeysWithAzureKeyVault(
        new Uri(builder.Configuration["DataProtection:KeyId"]!),
        new DefaultAzureCredential());
```

## Common Pitfalls

- Using the default in-memory key ring on multi-instance services → inconsistent decryption.
- Two services sharing a key ring without `SetApplicationName` → cross-contamination.
- Persisting keys but **not** encrypting at rest → blob compromise = full key compromise.
- Rotating keys but using a payload format with no version envelope → can't decrypt old data.

## Examples in this folder

- [`Program.cs`](Program.cs) — full setup
- [`EncryptDecrypt.cs`](EncryptDecrypt.cs) — `IDataProtector` use

## See also

- [../Cryptography](../Cryptography/) · [../SecretsManagement](../SecretsManagement/) · [../../Cloud/Azure/KeyVault](../../Cloud/Azure/KeyVault/)
