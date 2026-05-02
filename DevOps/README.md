# DevOps

> CI/CD, containers, IaC, and pipeline automation index for the .NET 2026 platform.

## Core Concepts

- **Build once, deploy many**: immutable artifacts (container image, NuGet, zip) promoted across environments.
- **IaC everywhere**: Bicep/Terraform/Pulumi -> no click-ops in prod.
- **GitOps for clusters**: Flux/ArgoCD reconcile desired state from Git.
- **OIDC > secrets**: federated workload identity from CI to cloud.
- **Progressive delivery**: blue/green, canary, feature flags.
- **Supply-chain security**: SBOM (Syft), image scan (Trivy), signing (cosign), provenance (SLSA).

## "To Be Dangerous" Cheatsheet

- **Multi-stage Dockerfiles** + chiseled Ubuntu / distroless base for .NET 10.
- Use **`.dockerignore`** to skip `bin/`, `obj/`, `.git/`, secrets.
- Pin tool versions in CI (`global.json`, action `@vX`).
- Use **environments + approvals** in GitHub Actions / Azure DevOps for prod gates.
- **OIDC federated credentials** for cloud auth (no long-lived secrets).
- Cache wisely: NuGet packages, Docker layers, Gradle / npm.
- **Reusable workflows / templates** for DRY pipelines.
- Bicep `what-if`, Terraform `plan`, Pulumi `preview` -> require approval before apply.

## Quick Reference

| Concern | Tool of choice |
|---|---|
| Container image | Docker (BuildKit) / `dotnet publish /t:PublishContainer` |
| Orchestrator | Kubernetes (AKS/EKS/GKE) |
| Package mgr | Helm |
| GitOps | Flux v2 / ArgoCD |
| IaC (Azure-native) | Bicep |
| IaC (multi-cloud) | Terraform / Pulumi |
| CI (OSS / GitHub) | GitHub Actions |
| CI (enterprise Azure) | Azure DevOps Pipelines |
| CI (legacy / on-prem) | Jenkins |

## Common Pitfalls

- Single-stage Dockerfile -> bloated image, leaked source.
- Shared service-principal secrets in CI -> rotate forever.
- No `what-if`/`plan` gate on IaC changes.
- One mega-pipeline that mixes build, test, deploy without environment isolation.
- Missing PDB / probes in Kubernetes -> rolling updates take outages.

## Examples in this folder

- [Docker](./Docker/README.md), [Kubernetes](./Kubernetes/README.md), [Helm](./Helm/README.md), [GitOps](./GitOps/README.md)
- [Bicep](./Bicep/README.md), [Terraform](./Terraform/README.md), [Pulumi](./Pulumi/README.md), [ARM](./ArmTemplates/README.md)
- [GitHub Actions](./GitHubActions/README.md), [Azure DevOps](./AzureDevOps/README.md), [Jenkins](./Jenkins/README.md)

## See also

- [Cloud domain](../Cloud/README.md)
- [.NET 2026 roadmap](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
