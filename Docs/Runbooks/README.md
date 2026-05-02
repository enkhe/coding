# Runbooks

> Runbooks-as-code. Versioned next to the service. Exercised in game days.

## What every runbook needs

1. **Purpose** — when to use this runbook
2. **Signals** — alerts/dashboards that trigger it
3. **Mitigation steps** — ordered, copy-pasteable
4. **Rollback** — how to undo if mitigation makes things worse
5. **Escalation** — who to wake at what severity
6. **Related** — links to other runbooks and dashboards

## Catalog

- [Deploy & rollback](runbook-deploy-rollback.md)
- [Database failover](runbook-database-failover.md)
- [Error-budget burn](../../Observability/SLOs/runbook-error-budget-burn.md)

## Conventions

- File name: `runbook-<purpose>.md`
- One purpose per runbook (don't mix incidents)
- Use copy-pasteable commands; assume fingers shaking at 3am
- Include `# Last reviewed: YYYY-MM-DD` so stale ones are visible

## See also

- [../Templates/runbook-template.md](../Templates/runbook-template.md) · [../../Observability/SLOs](../../Observability/SLOs/)
