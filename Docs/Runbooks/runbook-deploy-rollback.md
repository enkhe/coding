# Runbook — Deploy & Rollback

**Service:** orders-api
**Owner:** platform team
**Last reviewed:** 2026-04-12

## Purpose

How to deploy and (more importantly) roll back the orders-api safely.

## Signals to start a rollback

- Error rate > 2× baseline for 5 minutes
- p99 latency > 1s for 5 minutes
- Dependency saturation alert from a downstream the new version touches
- Customer report from `#orders-support`

## Pre-deploy checks

- [ ] CI green on the target SHA
- [ ] Image signed (`cosign verify`)
- [ ] SBOM attached
- [ ] Migration plan reviewed (expand-contract: which phase?)
- [ ] On-call awake

## Deploy

```bash
gh workflow run release.yml \
  -f target_sha=<sha> \
  -f env=prod
```

Watch:
- `https://grafana.contoso.com/d/orders/orders-red`
- Tempo for new spans `service.name = "orders-api"`

## Rollback

If error rate climbs after deploy:

```bash
PREVIOUS_SHA=$(git log --pretty=%H -n 2 | tail -1)
gh workflow run release.yml \
  -f target_sha=$PREVIOUS_SHA \
  -f env=prod
```

Verify:

```bash
curl -fsS https://api.contoso.com/health/ready
curl -fsSI https://api.contoso.com/version | grep 'X-Version:'
```

## If a database migration was involved

- **Expand-only migration?** No DB action; just app rollback.
- **Backfill running?** Pause backfill; app rollback is safe.
- **Contract phase?** Rollback is **not safe**. Page the DBA on-call. We will roll forward to a hotfix.

## Escalation

- 5 minutes elevated errors → page platform on-call
- 15 minutes → page eng manager
- 30 minutes → page director + open INC channel

## Related

- [Error-budget burn](../../Observability/SLOs/runbook-error-budget-burn.md)
- [Database failover](runbook-database-failover.md)
