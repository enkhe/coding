# Microsoft Entra ID

> Workforce IdP. For app-side .NET integration patterns see [`Security/Authentication/Entra`](../../../Security/Authentication/Entra/) — this folder is about **the resource itself**.

## Concepts

- **Tenant** — your org's Entra directory
- **App registration** — your service in Entra; client ID + (cert/secret) + reply URLs + scopes
- **Service principal** — the runtime identity of the app reg in a tenant
- **Managed identity** — automatically-managed SP for Azure resources (no secrets)
- **Conditional Access** — policies (require MFA, compliant device, location)
- **PIM** — privileged identity management; just-in-time elevation
- **Federated credentials** — OIDC trust between Entra and other identities (GitHub, K8s)

## Common operations

```bash
# Create app registration
az ad app create --display-name orders-api \
  --sign-in-audience AzureADMyOrg

# Federated credential for GitHub Actions OIDC (no client secret)
az ad app federated-credential create --id <app-id> --parameters '{
  "name":"gha-main",
  "issuer":"https://token.actions.githubusercontent.com",
  "subject":"repo:contoso/infra:ref:refs/heads/main",
  "audiences":["api://AzureADTokenExchange"]
}'

# Assign role to a service principal
az role assignment create \
  --assignee-object-id <sp-object-id> \
  --assignee-principal-type ServicePrincipal \
  --role "Storage Blob Data Reader" \
  --scope "/subscriptions/<sub>/resourceGroups/<rg>/providers/Microsoft.Storage/storageAccounts/<sa>"
```

## Managed identity (preferred over secrets)

```csharp
// .NET picks up MI automatically via DefaultAzureCredential
var blob = new BlobServiceClient(new Uri(uri), new DefaultAzureCredential());
```

## See also

- [../../../Security/Authentication/Entra](../../../Security/Authentication/Entra/) · [../../../Security/Authentication/OpenIdConnect](../../../Security/Authentication/OpenIdConnect/) · [../../../Security/SecretsManagement](../../../Security/SecretsManagement/)
