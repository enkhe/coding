# Functions

> User-defined functions in T-SQL: scalar UDF, inline TVF, multi-statement TVF. Use inline TVFs liberally; avoid scalar UDFs and MSTVFs in hot paths.

## Core Concepts

| Type | Returns | Plan behavior | Use it? |
|------|---------|---------------|---------|
| **Scalar UDF** | single value | Pre-2019: row-by-row, no parallelism. 2019+: scalar UDF inlining (auto). | Avoid in `WHERE`/`SELECT` over big sets unless inlined. |
| **Inline TVF** (iTVF) | table from a single `SELECT` | Inlined into outer query - same speed as a view. | Yes - the workhorse. |
| **Multi-statement TVF** (MSTVF) | table built imperatively | Treated as a table variable, fixed cardinality estimate (1 row pre-2017, 100 since). Bad plans on large data. | Avoid for large sets. |

- **Determinism**: a function is deterministic if same input -> same output. Required for indexing computed columns and for `WITH SCHEMABINDING`.
- **`WITH SCHEMABINDING`**: locks referenced objects (table can't be dropped). Required for indexed computed columns and indexed views.
- **Side effects**: forbidden inside functions (no `INSERT/UPDATE/DELETE` on permanent tables, no `EXEC`).

## "To Be Dangerous" Cheatsheet

- Convert MSTVFs to iTVFs by collapsing logic into one `SELECT` (often using CTEs).
- Convert scalar UDFs to iTVFs that return a single column - then call with `CROSS APPLY`.
- Verify scalar UDF inlining: `SELECT name, is_inlineable FROM sys.sql_modules m JOIN sys.objects o ON m.object_id = o.object_id WHERE o.type = 'FN';`
- Add `WITH SCHEMABINDING` whenever possible - the optimizer trusts the function more.
- For string parsing prefer `STRING_SPLIT` (built-in, set-based) over user UDFs.

## Quick Reference

```sql
-- Inline TVF (preferred)
CREATE OR ALTER FUNCTION app.fn_OpenOrders(@CustomerId bigint)
RETURNS TABLE WITH SCHEMABINDING AS
RETURN
(
    SELECT OrderId, TotalAmount, PlacedUtc
    FROM   app.OrderHeader
    WHERE  CustomerId = @CustomerId AND Status = 1
);

-- Usage with CROSS APPLY
SELECT c.CustomerId, o.OrderId, o.TotalAmount
FROM   app.Customer c
CROSS APPLY app.fn_OpenOrders(c.CustomerId) o;
```

## Common Pitfalls

- Scalar UDF in `WHERE` -> non-sargable, full scan. Even with inlining, complex bodies fall back.
- MSTVF estimated as 100 rows -> wrong join strategy on millions.
- Forgetting `SCHEMABINDING` -> can't index computed columns referencing the function.
- Functions calling other functions -> opaque to optimizer.

## Examples in this folder

- [Functions.sql](./Functions.sql) - one of each kind, with a "do this instead" rewrite for the bad ones.

## See also

- [../Indexes/README.md](../Indexes/README.md)
- [../Views/README.md](../Views/README.md)
