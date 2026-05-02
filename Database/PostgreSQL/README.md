# PostgreSQL

> The Swiss-army RDBMS. JSONB, arrays, FTS, ranges, partitioning, materialized views, **pgvector** for AI.

## "To Be Dangerous" Cheatsheet

| Need | Pattern |
|---|---|
| JSON | `data jsonb` + GIN index `(data jsonb_path_ops)` |
| Arrays | `tags text[]` + GIN index |
| Full-text search | `tsvector` generated column + GIN index |
| UUID | `uuid PRIMARY KEY DEFAULT gen_random_uuid()` |
| Range types | `tstzrange` for time intervals + GiST exclude constraint |
| Partitioning | `PARTITION BY RANGE (created_at)` for time-series |
| Materialized view | `CREATE MATERIALIZED VIEW ... REFRESH CONCURRENTLY` |
| Vector search | `CREATE EXTENSION vector` then `vector(1536)` + HNSW index |
| Inspect | `EXPLAIN (ANALYZE, BUFFERS) ...` |
| Pool | PgBouncer in transaction mode |

## Quick Reference

```sql
CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS pg_trgm;
CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE orders (
    id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     uuid NOT NULL,
    amount      numeric(18,2) NOT NULL CHECK (amount > 0),
    metadata    jsonb NOT NULL DEFAULT '{}',
    placed_at   timestamptz NOT NULL DEFAULT now(),
    tsv         tsvector GENERATED ALWAYS AS (to_tsvector('english', metadata->>'note')) STORED
);

CREATE INDEX idx_orders_user_placed ON orders (user_id, placed_at DESC);
CREATE INDEX idx_orders_metadata    ON orders USING gin (metadata jsonb_path_ops);
CREATE INDEX idx_orders_tsv         ON orders USING gin (tsv);
```

## Common Pitfalls

- `SERIAL`/`BIGSERIAL` — legacy; prefer `GENERATED ALWAYS AS IDENTITY`
- `varchar(n)` for length limit — use `text` + `CHECK`
- Long transactions block VACUUM → MVCC bloat
- Index bloat — periodic `REINDEX CONCURRENTLY`

## See also

- [../VectorDb](../VectorDb/) · [../EntityFramework](../EntityFramework/) · [../Dapper](../Dapper/)
