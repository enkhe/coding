# Azure Storage

> Blobs, queues, tables, files. Multi-purpose, cheap, durable.

## Services

| Service | Use |
|---|---|
| **Blobs** | Files, images, large objects |
| **Queues** | Simple messages (use Service Bus for richer needs) |
| **Tables** | Wide-key-value (Cosmos DB Table API is the modern path) |
| **Files** | SMB/NFS shares |

## Blob types

| Type | Use |
|---|---|
| **Block blob** | Default; up to ~190 TiB |
| **Append blob** | Logs (append-only) |
| **Page blob** | VHDs / random read-write |

## Tiers + lifecycle

```
Hot   → Cool   → Cold   → Archive
write+read    write rare   read rare    deep archive (rehydrate)
```

Set lifecycle policies per container — auto-tier old blobs.

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Auth | `DefaultAzureCredential` (managed identity → no keys) |
| Upload | `BlobClient.UploadAsync(stream, overwrite: true)` |
| Download | `BlobClient.DownloadStreamingAsync()` |
| List | `container.GetBlobsAsync(prefix: "users/")` |
| SAS (read-only, time-limited) | `BlobClient.GenerateSasUri(...)` |
| Server-side copy | `BlobClient.SyncCopyFromUriAsync(srcSasUri)` |

## Quick Reference

```csharp
// Package: Azure.Storage.Blobs, Azure.Identity
var service = new BlobServiceClient(new Uri("https://mystorage.blob.core.windows.net"),
                                    new DefaultAzureCredential());
var container = service.GetBlobContainerClient("orders");
await container.CreateIfNotExistsAsync();

await container.GetBlobClient("2026/04/order-123.json").UploadAsync(stream, overwrite: true);
```

## Common Pitfalls

- Storing secrets / connection strings — use managed identity instead
- Not setting lifecycle policy → archive-tier blobs at full hot price forever
- Blocking the LB with synchronous upload of huge files
- SAS tokens with start time off by clock skew → 403s

## See also

- [../KeyVault](../KeyVault/) · [../../../Security/SecretsManagement](../../../Security/SecretsManagement/)
