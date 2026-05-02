# Azure

> Master index of Azure services in this repo, focused on .NET 2026 senior/architect concerns.

## Core Concepts

- **Subscription -> Resource Group -> Resource**; tag at RG and resource.
- **Region pair** (e.g. eastus / westus3) for paired-region DR.
- **Availability Zones** (where supported) for in-region HA.
- **Managed Identity (MI)** is the auth substrate; system-assigned for single resource, user-assigned for sharing across resources.
- **Private endpoints** + **Private DNS zones** isolate PaaS data plane.
- **RBAC** (control plane) and data-plane RBAC (Storage Blob Data Contributor, Key Vault Secrets User, Service Bus Data Receiver, etc.).

## "To Be Dangerous" Cheatsheet

- `DefaultAzureCredential` in code -> works locally (Azure CLI), in Azure (MI), in CI (OIDC).
- Tagging policy: `costCenter`, `env`, `owner`, `app`, `dataClassification` enforced via Azure Policy.
- Use **Azure Verified Modules** or **Bicep public registry** for known-good IaC.
- **Application Insights + Log Analytics** workspace per environment, KQL for everything.
- Networking baseline: hub/spoke, Azure Firewall or NAT Gateway, Private DNS zones.
- Set **budgets and anomaly alerts** at subscription and RG levels.
- Use **deployment stacks** (Bicep) for cleanup-on-delete semantics.

## Quick Reference

| Service | Use when | Folder |
|---|---|---|
| App Service | Stateful PaaS web app, slots, easy auth | [AppService](./AppService/README.md) |
| Container Apps | Microservice w/ scale-to-zero, Dapr/KEDA | [ContainerApps](./ContainerApps/README.md) |
| AKS | Full K8s, custom networking, GitOps, Workload Identity | [AKS](./AKS/README.md) |
| Functions | Event-driven, short-lived, isolated worker .NET 10 | [Functions](./Functions/README.md) |
| Service Bus | Transactional messaging, sessions, DLQ | [ServiceBus](./ServiceBus/README.md) |
| Storage | Blob/Queue/Table/Files, lifecycle tiers, SAS | [Storage](./Storage/README.md) |
| Key Vault | Secrets/keys/certs, soft-delete, RBAC | [KeyVault](./KeyVault/README.md) |
| Entra ID | App regs, MSAL, multi-tenant, scopes/roles | [EntraId](./EntraId/README.md) |
| SQL Database | Hyperscale, Serverless, geo-replication | [SqlDatabase](./SqlDatabase/README.md) |
| AI Search | Vector + hybrid + semantic ranking | [AISearch](./AISearch/README.md) |
| Aspire | Local orchestration + azd deploy to ACA | [Aspire](./Aspire/README.md) |

## Common Pitfalls

- Forgetting the **paired region** when planning DR.
- Granting `Contributor` instead of a specific data-plane role.
- Public network access left on PaaS.
- App Settings holding secrets instead of Key Vault references.
- Not configuring health probes / startup time on App Service / ACA.

## Examples in this folder

- See subfolders linked in the Quick Reference table.

## See also

- [.NET 2026 roadmap](../../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [DevOps/Bicep](../../DevOps/Bicep/README.md)
- [Cloud overview](../README.md)
