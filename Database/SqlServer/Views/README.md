# SQL Server Views

> Stored queries. Use for read-side projections in CQRS, security boundaries, and reporting.

## Kinds

| Kind | Notes |
|---|---|
| **Standard view** | Logical query; no storage |
| **Indexed (materialized) view** | Persisted, must satisfy strict rules; great for repeated heavy aggregates |
| **Schema-bound view** | `WITH SCHEMABINDING` — required for indexed views; prevents underlying schema changes |

## "To Be Dangerous" Cheatsheet

```sql
-- Standard view
CREATE OR ALTER VIEW app.v_orders_for_user
AS
SELECT o.id, o.user_id, o.amount, o.placed_at,
       u.email, u.display_name
FROM dbo.orders o
JOIN dbo.users  u ON u.id = o.user_id;

-- Schema-bound + indexed (aggregate cached)
CREATE OR ALTER VIEW app.v_orders_daily
WITH SCHEMABINDING AS
SELECT CAST(placed_at AS date) AS day,
       COUNT_BIG(*) AS order_count,
       SUM(amount)  AS total_amount
FROM dbo.orders
GROUP BY CAST(placed_at AS date);
GO
CREATE UNIQUE CLUSTERED INDEX ix_v_orders_daily ON app.v_orders_daily (day);
```

## Common Pitfalls

- Indexed views require `WITH SCHEMABINDING` and many rules (no outer joins, no `*`, deterministic functions only)
- Updates through views — limited and surprising; prefer querying view, modifying base
- Forgetting `OR ALTER` → migration breaks on second run
- Performance on huge views without proper indexes on base tables

## See also

- [../Indexes](../Indexes/) · [../../EntityFramework](../../EntityFramework/)
