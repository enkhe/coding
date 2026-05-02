# Runbook — Database Failover

**Service:** orders-api → Azure SQL (or Postgres Flexible)
**Owner:** platform / DBA
**Last reviewed:** 2026-04-12

## Purpose

Promote the geo-replica when the primary is unavailable.

## Signals

- Connection refused from app for >2 min
- Primary status `Failed` in Azure portal / `pg_isready` failure
- Read latency p99 > 5s

## Pre-flight

- [ ] Confirm scope: full region outage vs single-server issue
- [ ] Notify customer-comms — degradation post in status page (read-only mode if applicable)

## Failover steps (Azure SQL active geo-replication)

```bash
# Check status
az sql db replica list-links \
  -g rg-orders-prod -s orders-sql-east -n orders

# Forced failover
az sql db replica set-primary \
  --resource-group rg-orders-prod \
  --server orders-sql-west \
  --name orders \
  --allow-data-loss
```

After failover:

1. Update DNS or connection-string secret in Key Vault to point to the new primary.
2. Restart app pods to pick up new connection: `kubectl rollout restart deploy/orders-api -n orders`.
3. Verify writes: `POST /test-order` (synthetic).

## Postgres Flexible Server

```bash
az postgres flexible-server restart \
  -g rg-orders-prod -n orders-pg --failover Forced
```

## Validation

- [ ] `/health/ready` returns 200 across all pods
- [ ] Recent rows visible (`SELECT max(created_at) FROM orders;`)
- [ ] No DLQ growth on `orders.placed`

## Rollback (if planned failover went wrong)

If old primary is healthy and you want to fail back:
- Wait for replication to catch up (`pg_stat_replication` / Azure portal lag metric)
- Repeat the failover command targeting the original

## Escalation

- DBA on-call (PagerDuty)
- Platform manager if customer-impacting > 30 min

## Related

- [Deploy & rollback](runbook-deploy-rollback.md)
- [Error-budget burn](../../Observability/SLOs/runbook-error-budget-burn.md)
