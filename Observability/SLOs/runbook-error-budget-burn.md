# Runbook — Error Budget Burn

**Service:** orders-api
**Trigger:** SLO burn-rate alert (page or ticket).
**Owner:** platform team.

## Signals to check (in order)

1. Grafana dashboard `Orders / RED` — look at error rate and p99 latency in the last 1h.
2. Tempo traces filtered by `service.name = "orders-api"` and `status = ERROR` for representative failures.
3. Recent deploys: `gh run list --workflow=release.yml --limit 5`.
4. Dependencies — Postgres CPU, Redis hit rate, downstream services' SLO status.

## Mitigation

- **If correlated with a recent deploy** → roll back via the platform team's release tooling.
- **If a dependency is degraded** → enable feature flag for graceful degradation (skip non-critical writes); page the owning team.
- **If runaway client** → enable rate limit on the offending tenant's API key (see `rate-limit-policies.yaml`).

## Rollback

- Trigger `release.yml` workflow with `target_sha = <previous_green_sha>`.
- Verify by watching error rate panel — should drop within 2 minutes.

## Post-incident

- Write a blameless post-mortem using `Docs/Templates/incident-postmortem-template.md`.
- File action items in the `INC` tracker.
- If budget exhausted, freeze non-critical merges until next month resets.

## Related

- [SLO definitions](slo-definitions.yaml)
- [../OpenTelemetry/README.md](../OpenTelemetry/README.md)
