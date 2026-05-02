# App Service

> Managed PaaS for web apps and APIs with deployment slots, easy auth, and custom containers.

## Core Concepts

- **App Service Plan** = compute (SKU + scale); multiple apps can share a plan.
- **Deployment slots** (Standard+): blue/green, swap with preview, warm-up, slot-sticky settings.
- **Easy Auth** (App Service Authentication): zero-code OIDC for Entra/Google/GitHub.
- **Custom containers**: run any image (Linux); pull from ACR via managed identity.
- **VNet integration** + **Private Endpoints** for inbound/outbound isolation.

## "To Be Dangerous" Cheatsheet

- SKU picks: **B1** dev, **P1v3 / P2v3** prod, **Premium v3 zone-redundant** for HA.
- Always set **Always On** + **Health check path** for prod.
- Use **slot swap** with `applicationInitialization` for zero-downtime.
- Reference Key Vault secrets via `@Microsoft.KeyVault(...)` in app settings.
- **64-bit + ARR affinity off** for stateless APIs.
- Enable **Application Insights** auto-instrumentation.

## When to choose

| App Service wins | Container Apps wins |
|---|---|
| Stateful sticky sessions | Scale to zero |
| Slot swap workflow | Microservice mesh / Dapr |
| Built-in EasyAuth | KEDA event scaling |
| Mature .NET hosting model | Per-revision traffic split |

## Quick Reference

- App settings precedence: slot settings -> app settings -> container env vars.
- Health check: HTTP path with 2xx/3xx; 5 consecutive failures -> instance pulled.
- WEBSITE_RUN_FROM_PACKAGE=1 for immutable deploys.

## Common Pitfalls

- Forgetting **slot-sticky** flag on env-specific app settings.
- Using FTP creds; prefer **OIDC publish profile** or **managed identity to ACR**.
- Hot deploying via Kudu in prod (lose slot benefits).

## Examples in this folder

- [azure-pipelines.yml](./azure-pipelines.yml) - build, ACR push, deploy to slot, swap.

## See also

- [Container Apps](../ContainerApps/README.md)
- [Aspire](../Aspire/README.md)
- [DevOps/AzureDevOps](../../../DevOps/AzureDevOps/README.md)
