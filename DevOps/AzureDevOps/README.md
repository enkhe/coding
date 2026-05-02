# Azure DevOps

> YAML pipelines on Azure DevOps Services. Stages, templates (steps/jobs/stages), service connections, environments + approvals.

## Core Concepts

- **Pipeline** — top-level YAML triggered by branch/PR/schedule
- **Stages → Jobs → Steps** — three nesting levels; stages can have approvals
- **Templates** — reuse via `template:` (steps/jobs/stages); parameterize with `parameters:`
- **Service connections** — auth to Azure / Docker registries / external systems; prefer **workload identity** over secrets
- **Environments** — gated targets with approvals & deployment history
- **Library** — variable groups + secure files + Key Vault links

## "To Be Dangerous" Cheatsheet

| Need | Snippet |
|---|---|
| Variable group | `variables: - group: orders-prod` |
| Secret from KV | Add Azure Key Vault link to library; reference variable name |
| Stage approval | `environment: name: prod` (configure approvers in portal) |
| Reusable steps | `- template: templates/build-dotnet.yml` |
| Manual trigger only | `trigger: none` |
| Multi-repo | `resources: { repositories: [...] }` |

## Quick Reference

See [`azure-pipelines.yml`](azure-pipelines.yml) and [`templates/build-dotnet.yml`](templates/build-dotnet.yml).

## Common Pitfalls

- Plain-text secrets in YAML → use library / Key Vault link
- Stage runs after a failed earlier stage because of `dependsOn` mistake
- No environment approval on prod → unauthorized deploys
- Self-hosted agents lacking outbound HTTPS → mysterious pipe errors

## See also

- [../GitHubActions](../GitHubActions/) · [../../Cloud/Azure](../../Cloud/Azure/)
