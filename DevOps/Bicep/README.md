# Bicep

> Azure-native IaC. Replaces ARM templates. Compiles to ARM under the hood.

## Core Concepts

- **Modules** — reusable units; can come from local files, Bicep registry, or a public OCI registry
- **Scopes** — subscription / management group / resource group / tenant
- **`what-if`** — preview changes before deploy
- **Parameters / outputs** — typed; outputs feed into other modules
- **`existing` keyword** — reference an existing resource without redeploying

## "To Be Dangerous" Cheatsheet

| Need | Pattern |
|---|---|
| Deploy | `az deployment sub create -l eastus -f main.bicep -p env=prod` |
| Preview | `az deployment sub what-if -l eastus -f main.bicep` |
| Format / lint | `bicep build`, `bicep format`, `bicep lint` |
| Module from registry | `module x 'br:contoso.azurecr.io/bicep/modules/storage:1.0' = {...}` |
| Decorators | `@description`, `@minLength`, `@allowed`, `@secure` |
| Params file | `main.bicepparam` (typed) |

## Quick Reference (sub-scope deployment)

```bicep
targetScope = 'subscription'

@description('Environment name (prod/staging/dev).')
@allowed(['prod','staging','dev'])
param env string

@description('Primary location.')
param location string = deployment().location

var rgName = 'rg-orders-${env}'

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: rgName
  location: location
  tags: { app: 'orders', env: env, 'cost-center': 'platform' }
}

module storage 'modules/storage.bicep' = {
  scope: rg
  name: 'storage'
  params: { name: 'sto${uniqueString(rg.id)}', location: location, tags: rg.tags }
}

output storageEndpoint string = storage.outputs.endpoint
```

## Common Pitfalls

- Forgetting `targetScope` → wrong-scope deploys
- Not running `what-if` in CI → surprise deletes
- Hard-coded SKUs across envs → cost surprises
- Secrets in params instead of Key Vault references → secret leakage in deploy logs

## Examples in this folder

- [`main.bicep`](main.bicep) · [`main.bicepparam`](main.bicepparam) · [`modules/storage.bicep`](modules/storage.bicep)

## See also

- [../Terraform](../Terraform/) · [../Pulumi](../Pulumi/) · [../../Cloud/Azure](../../Cloud/Azure/)
