# FinOps

> Make cost a first-class engineering signal. Tag, observe, right-size, scale-to-zero, automate.

## Core Principles

1. **Tag everything** — without tags you can't allocate, you can't optimize.
2. **Show cost where decisions are made** — dashboards in front of the team that owns the resource.
3. **Right-size on data** — over-provisioning is the dominant waste; CPU at 5% util is a lift opportunity.
4. **Scale to zero** when load is bursty (ACA Consumption, Functions, AKS HPA + KEDA).
5. **Reserved / savings plans** for steady-state baseline; on-demand for the spikes.

## "To Be Dangerous" Cheatsheet

| Category | What to do |
|---|---|
| Tagging | Enforce via Azure Policy / AWS SCP — required tags `cost-center`, `app`, `env`, `owner` |
| Allocation | Cost Mgmt views grouped by tag; Budgets per cost-center |
| Right-sizing | Use Azure Advisor / AWS Compute Optimizer; verify with prod metrics |
| Scale to zero | ACA Consumption, AWS Lambda, GCP Cloud Run, Functions, KEDA |
| Storage tiers | Hot → Cool → Cold → Archive; lifecycle policies on retention |
| Egress | CDN/Front Door, regional pinning, peering, private endpoints |
| Idle resources | Schedule shut-down for non-prod (nightly + weekends) |
| Anomaly | Cost anomaly alerts at the subscription / account level |

## Required tags (recommended set)

| Tag | Purpose |
|---|---|
| `cost-center` | Where the bill rolls up to |
| `app` | The application/service |
| `env` | `prod` / `staging` / `dev` / `sandbox` |
| `owner` | Team email or DL |
| `created-by` | Pipeline run or human |
| `data-classification` | `public` / `internal` / `confidential` / `restricted` |

## Quick Reference (Azure Policy enforcing required tags)

```bicep
targetScope = 'subscription'

resource requireTags 'Microsoft.Authorization/policyAssignments@2024-04-01' = {
  name: 'require-cost-tags'
  properties: {
    displayName: 'Require cost-center, app, env tags'
    policyDefinitionId: '/providers/Microsoft.Authorization/policyDefinitions/871b6d14-10aa-478d-b590-94f262ecfa99' // 'Require a tag and its value'
    parameters: {
      tagName: { value: 'cost-center' }
    }
  }
}
```

## KQL — top-spending resources last 30d

```kusto
Usage
| where TimeGenerated > ago(30d)
| summarize cost = sum(PreTaxCost) by ResourceId, Tags = tostring(Tags)
| top 50 by cost desc
```

## Common Pitfalls

- "We'll right-size later" → never happens; bake into the first deploy.
- Reserved instances bought before steady-state proven → unused commit.
- Egress through public endpoints because of "simplicity" → bills explode at scale.
- Dev resources never shut down → quietly the second-largest line item.
- Aggregation only at subscription level → no team-level signal.

## Examples in this folder

- [`tagging-policy.bicep`](tagging-policy.bicep) — required-tags policy
- [`costs.kql`](costs.kql) — query top spenders + month-over-month deltas
- [`shutdown-nonprod.yaml`](shutdown-nonprod.yaml) — scheduled shutdown via GitHub Actions

## See also

- [../Azure](../Azure/) · [../../Docs/Roadmaps](../../Docs/Roadmaps/) · [../../Observability/SLOs](../../Observability/SLOs/)
