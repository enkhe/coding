# Container Apps (ACA)

> Serverless managed Kubernetes-derived runtime with scale-to-zero, KEDA, and Dapr.

## Core Concepts

- **Environment**: shared boundary (VNet, Log Analytics, Dapr) for one or more container apps.
- **Container App**: runs revisions of a container image with ingress + scaling rules.
- **Revisions**: immutable; multiple-revision mode enables traffic splitting (canary, blue/green).
- **Scaling**: HTTP, KEDA scalers (Service Bus queue length, Kafka lag, CPU, custom).
- **Dapr** (built-in): pub/sub, state, secrets, service invocation; opt-in per app.
- **Workload identity**: system or user-assigned MI; map to env vars or use SDK.

## "To Be Dangerous" Cheatsheet

- Set `minReplicas: 0` for scale-to-zero; mind cold start (1-3s typical).
- For HTTP: rule on `concurrent requests`, ~10-100 per replica.
- For queue workers: KEDA `azure-servicebus` rule, target ~5 messages per replica.
- Use **internal-only ingress** for backend services; expose front-door via APIM or Front Door.
- Pull images from **ACR via MI**; no admin user.
- Configure **probes**: startup/liveness/readiness same as K8s.
- Use **secrets** at the app level; reference from env vars.

## When ACA wins over AKS

- HTTP/queue microservices, no need for custom CRDs.
- Want scale-to-zero without provisioning node pools.
- Prefer managed Dapr/KEDA over installing them.
- Small platform team; cannot run AKS day-2 ops.

## Quick Reference

- Multi-revision mode: `--revision-mode multiple`, then `traffic` weights.
- Bind ACR: `--registry-server <acr>.azurecr.io --registry-identity system`.
- Custom domain: managed cert auto-issued for FQDNs CNAMEd to env.

## Common Pitfalls

- Leaving single-revision mode and expecting traffic split.
- Forgetting **CPU/Memory** limits -> capped scale-out.
- Using consumption profile and complaining about cold start instead of switching to **Dedicated workload profile**.

## Examples in this folder

- [main.bicep](./main.bicep) - environment + app + KEDA Service Bus scaler.

## See also

- [AKS](../AKS/README.md)
- [App Service comparison](../AppService/README.md)
- [Aspire](../Aspire/README.md)
- [GitOps](../../../DevOps/GitOps/README.md)
