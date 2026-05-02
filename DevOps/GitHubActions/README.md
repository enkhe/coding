# GitHub Actions

> CI/CD primitive for the modern open-source default. Workflows, jobs, matrix, reusable workflows, OIDC.

## Core Concepts

- **Workflow** — YAML file in `.github/workflows/`
- **Job** — runs on a single runner; jobs in a workflow can run in parallel
- **Step** — single command or action invocation
- **Matrix** — run a job across multiple combos (OS × language version × etc.)
- **Reusable workflow** — `workflow_call` lets one workflow invoke another (DRY pipelines)
- **OIDC** — keyless cloud auth; no long-lived secrets
- **Environments** — protection rules (required reviewers, secrets, deployment branch policy)

## "To Be Dangerous" Cheatsheet

| Need | Snippet |
|---|---|
| Trigger on push/PR | `on: { push: { branches: [main] }, pull_request: {} }` |
| Schedule | `on: { schedule: [{ cron: '0 2 * * *' }] }` |
| Manual | `on: { workflow_dispatch: {} }` |
| OIDC | `permissions: { id-token: write, contents: read }` |
| Cache .NET deps | `actions/cache@v4` over `~/.nuget/packages` keyed on lockfile |
| Path filter | `paths: ['src/**','tests/**']` |
| Environment gate | `environment: { name: prod, url: https://... }` |

## Quick Reference

See:
- [`dotnet-ci.yml`](dotnet-ci.yml) — build / test / containerize / push
- [`release.yml`](release.yml) — OIDC deploy to Azure with environment approval
- [`reusable-build.yml`](reusable-build.yml) — invoked by callers
- [`security-scan.yml`](security-scan.yml) — CodeQL + SBOM + Trivy

## Common Pitfalls

- Long-lived `AZURE_CREDENTIALS` secrets → use OIDC instead
- `actions/checkout` without `fetch-depth: 0` for tools that need history (gitversion, conventional commits)
- `if: github.ref == 'refs/heads/main'` only on jobs that should be main-only — easy to forget
- No matrix `fail-fast: false` → first failure kills the rest, debugging is hard
- Forgetting `concurrency` → multiple deploys racing

## See also

- [../AzureDevOps](../AzureDevOps/) · [../Jenkins](../Jenkins/) · [../../Security/SupplyChain](../../Security/SupplyChain/)
