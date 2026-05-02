# MySQL

> MySQL 8+. JSON, window functions, CTEs, InnoDB by default.

## "To Be Dangerous" Cheatsheet

| Need | Pattern |
|---|---|
| JSON column | `metadata JSON` + `JSON_EXTRACT(metadata, '$.note')` |
| Generated column | `note VARCHAR(255) AS (metadata->>'$.note') STORED` (then index) |
| CTE | `WITH cte AS (...) SELECT ...` |
| Window function | `ROW_NUMBER() OVER (PARTITION BY user_id ORDER BY placed_at DESC)` |
| Soft delete | `deleted_at TIMESTAMP NULL` + filtered queries |
| Pagination | Keyset (`WHERE id > :lastSeenId LIMIT 20`) — not OFFSET on big tables |
| UUID v7 (MySQL 8.0+) | `UUID_TO_BIN(UUID(), 1)` |
| Sane defaults | InnoDB engine; `utf8mb4` charset; `utf8mb4_0900_ai_ci` collation |
| Replication | Async by default; consider Group Replication for HA |

## Quick Reference

```sql
CREATE TABLE orders (
    id           BINARY(16) PRIMARY KEY DEFAULT (UUID_TO_BIN(UUID(), 1)),
    user_id      BINARY(16) NOT NULL,
    amount       DECIMAL(18,2) NOT NULL,
    metadata     JSON NOT NULL,
    placed_at    TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    deleted_at   TIMESTAMP(6) NULL,
    KEY idx_user_placed (user_id, placed_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
```

## Common Pitfalls

- `utf8` is 3-byte (broken!); always use `utf8mb4`
- `OFFSET` pagination on big tables — slow; use keyset
- Implicit type coercion — `WHERE varchar_col = 1` causes full scan
- Default isolation `REPEATABLE READ` differs from Postgres `READ COMMITTED`

## See also

- [../PostgreSQL](../PostgreSQL/) · [../EntityFramework](../EntityFramework/)
