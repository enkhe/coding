# Cloud

> Master index for cloud platforms (Azure, AWS, GCP) and FinOps practices, aligned to the .NET 2026 senior/architect roadmap.

## Core Concepts

- **Compute spectrum**: VM -> Container Orchestrator (AKS/EKS/GKE) -> Managed Containers (ACA, Cloud Run, Fargate) -> PaaS (App Service) -> Serverless (Functions, Lambda).
- **Identity-first**: workload identity / managed identity beats static secrets every time.
- **Network isolation**: private endpoints + service endpoints + WAF; default-deny egress where possible.
- **Data plane vs control plane**: RBAC at the control plane, data-plane RBAC where supported (Storage, Service Bus, Key Vault, SQL).
- **Cost = architecture**: scale-to-zero, lifecycle tiers, spot/savings, right-sized SKUs.

## "To Be Dangerous" Cheatsheet

- Always use **Managed Identity** for service-to-service auth in Azure (`DefaultAzureCredential`).
- Tag every resource: `costCenter`, `env`, `owner`, `app`, `dataClassification`.
- Pick a **region pair** for DR; deploy across **availability zones** for HA.
- Use **private endpoints** for PaaS data services in production; disable public network access.
- Prefer **RBAC** over access policies / shared keys (Key Vault, Storage).
- Architect for **scale-to-zero** when traffic is bursty (ACA, Functions Consumption, Cloud Run).
- Externalize config: Key Vault + App Configuration; never bake secrets into images.
- Pick **Hyperscale** for fast-growing SQL; **Serverless** SQL for dev/test.
- For events: Service Bus = transactional, Event Hubs = telemetry, Event Grid = reactive.

## Quick Reference

| Need | Azure | AWS | GCP |
|---|---|---|---|
| Web app PaaS | App Service | Elastic Beanstalk | App Engine |
| Managed containers | Container Apps | ECS Fargate | Cloud Run |
| Kubernetes | AKS | EKS | GKE |
| Functions | Azure Functions | Lambda | Cloud Functions |
| Object store | Blob Storage | S3 | Cloud Storage |
| Queue | Service Bus / Storage Queue | SQS | Pub/Sub |
| Topic / pub-sub | Service Bus Topic / Event Grid | SNS / EventBridge | Pub/Sub |
| RDBMS | Azure SQL | RDS | Cloud SQL |
| NoSQL | Cosmos DB | DynamoDB | Firestore / Spanner |
| Secrets | Key Vault | Secrets Manager | Secret Manager |
| Identity | Entra ID | IAM / Cognito | Cloud IAM / Identity Platform |
| Search | AI Search | OpenSearch | Vertex AI Search |

## Common Pitfalls

- Using account keys / connection strings instead of managed identity.
- Skipping tags -> chargeback impossible.
- Public endpoints on prod data services.
- Single region (no region pair, no AZ awareness).
- Choosing AKS when Container Apps would do (operational tax).
- No budget alerts -> bill shock.

## Examples in this folder

- [Azure index](./Azure/README.md)
- [AWS quick map](./AWS/README.md)
- [GCP quick map](./GCP/README.md)
- [FinOps practices](./FinOps/README.md)

## See also

- [.NET 2026 roadmap](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [DevOps domain](../DevOps/README.md)
