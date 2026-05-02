# Pulumi

> IaC in real programming languages (.NET / TypeScript / Python / Go). Use when you want types, conditionals, and library reuse.

## Core Concepts

- **Stack** — environment instance (`dev`, `prod`); has its own config + state
- **Resources** — created via SDK calls; deps inferred from references
- **Outputs** — async values; combine with `Output.Tuple(...).Apply(...)`
- **Components** — reusable groups of resources (`Pulumi.ComponentResource`)
- **Backends** — Pulumi Service (default), S3, Azure Blob, self-hosted

## "To Be Dangerous" Cheatsheet

| Need | Command |
|---|---|
| Create stack | `pulumi stack init dev` |
| Preview | `pulumi preview` |
| Deploy | `pulumi up` |
| Destroy | `pulumi destroy` |
| Set config | `pulumi config set --secret azureSubscriptionId xxx` |
| Refresh state | `pulumi refresh` |

## Quick Reference (.NET)

```csharp
using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

return await Deployment.RunAsync(() =>
{
    var rg = new ResourceGroup("rg-orders", new() { Tags = new() { ["app"] = "orders", ["env"] = "prod" } });

    var sa = new StorageAccount("storders", new()
    {
        ResourceGroupName = rg.Name,
        Sku = new SkuArgs { Name = SkuName.Standard_LRS },
        Kind = Kind.StorageV2,
        MinimumTlsVersion = MinimumTlsVersion.TLS1_2,
        AllowBlobPublicAccess = false,
    });

    return new Dictionary<string, object?>
    {
        ["resourceGroup"] = rg.Name,
        ["storageEndpoint"] = sa.PrimaryEndpoints.Apply(e => e.Blob),
    };
});
```

## Common Pitfalls

- Treating Pulumi state as ephemeral — store in a real backend with locking
- Using language abstractions to dynamically generate resources without naming → "renaming" rebuilds everything
- Forgetting `--secret` for sensitive config → secrets in plaintext

## Examples in this folder

- [`Program.cs`](Program.cs) — .NET stack
- [`Pulumi.yaml`](Pulumi.yaml) · [`Pulumi.dev.yaml`](Pulumi.dev.yaml)

## See also

- [../Bicep](../Bicep/) · [../Terraform](../Terraform/)
