# SQL Server

> T-SQL essentials, performance internals, and SQL Server 2025 features (vector, JSON type) for senior .NET engineers.

## Core Concepts

- **Engine layout**: Storage engine (pages, extents, B-trees), Query optimizer (cost-based), Buffer pool (8KB pages cached in RAM), Transaction log (write-ahead).
- **Page = 8KB**, extent = 8 pages = 64KB. Row cannot span pages (except LOB).
- **Clustered index = the table.** A heap (no clustered) is rare and usually wrong.
- **Statistics drive plans.** Stale stats = bad plans. `AUTO_UPDATE_STATISTICS` is on by default; large tables need `STATS_STREAM` or scheduled updates.
- **Plan cache**: parameterized queries reuse plans. Ad-hoc with literals = cache bloat.
- **Tempdb is shared** by sorts, hashes, snapshot isolation, temp tables. Configure multiple data files.
- **MAXDOP and Cost Threshold for Parallelism**: defaults (0 / 5) are wrong for OLTP. Start with `MAXDOP = 8` (or vCPU/2) and `CTFP = 50`.

### Isolation levels (SQL Server)

| Level | Reads see | Pitfalls |
|-------|----------|----------|
| `READ UNCOMMITTED` (NOLOCK) | Dirty data, ghost rows, duplicate rows | Never use in financials. Returns wrong counts under load. |
| `READ COMMITTED` (default) | Committed only, blocks on writers | Non-repeatable reads. |
| `READ COMMITTED SNAPSHOT` (RCSI) | Row-version snapshot | Tempdb pressure. Set DB-level: `ALTER DATABASE X SET READ_COMMITTED_SNAPSHOT ON`. |
| `REPEATABLE READ` | Same row each time in txn | Holds shared locks, deadlocks. |
| `SNAPSHOT` | Txn-start snapshot | Update conflicts throw 3960. |
| `SERIALIZABLE` | Phantom-free | Range locks, deadlock prone. |

**Default modern recommendation**: enable RCSI on all OLTP databases.

## "To Be Dangerous" Cheatsheet

- Always read the **actual** plan, not the estimated. Use `SET STATISTICS IO, TIME ON` for cost.
- A `Key Lookup` operator means your nonclustered index is missing `INCLUDE` columns.
- `Hash Match` on small inputs = bad estimate. Check stats / parameter sniffing.
- Parameter sniffing fix: `OPTION (RECOMPILE)` for ad-hoc, `OPTIMIZE FOR UNKNOWN` for procs, or local variable trick.
- Fragmentation < 5% leave alone, 5-30% reorganize, > 30% rebuild.
- `ALTER INDEX REBUILD WITH (ONLINE = ON, RESUMABLE = ON)` since SQL 2017.
- Columnstore = batch mode + 10x compression. Use for fact tables > 1M rows.
- Temporal tables (`WITH SYSTEM_VERSIONING = ON`) give point-in-time queries for free.
- SQL 2025 `vector(1536)` type + `VECTOR_DISTANCE('cosine', a, b)` makes SQL Server a viable vector store.
- SQL 2025 native `json` type (binary, indexable) replaces `nvarchar(max)` for JSON columns.

## Quick Reference

### Find slow queries

```sql
SELECT TOP 20
    qs.execution_count,
    qs.total_elapsed_time / qs.execution_count AS avg_us,
    qs.total_logical_reads / qs.execution_count AS avg_reads,
    SUBSTRING(qt.text, qs.statement_start_offset/2 + 1,
        (CASE qs.statement_end_offset WHEN -1 THEN DATALENGTH(qt.text)
         ELSE qs.statement_end_offset END - qs.statement_start_offset)/2 + 1) AS query
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
ORDER BY avg_us DESC;
```

### Wait stats (where time is going)

```sql
SELECT TOP 10 wait_type, wait_time_ms, signal_wait_time_ms,
       waiting_tasks_count
FROM sys.dm_os_wait_stats
WHERE wait_type NOT IN ('SLEEP_TASK','BROKER_TASK_STOP','HADR_FILESTREAM_IOMGR_IOCOMPLETION')
ORDER BY wait_time_ms DESC;
```

### Subfolders

- [Schemas/](./Schemas/README.md)
- [Indexes/](./Indexes/README.md)
- [StoredProcedures/](./StoredProcedures/README.md)
- [Functions/](./Functions/README.md)
- [Views/](./Views/README.md)
- [Migrations/](./Migrations/README.md)

## Common Pitfalls

- `NOLOCK` everywhere - returns wrong data under concurrent updates.
- Scalar UDFs in `WHERE` / `SELECT` - row-by-row execution. SQL 2019+ can inline some, but not all.
- Implicit conversion (e.g., `nvarchar` column compared to `varchar` literal) - kills index seeks.
- `ORDER BY` without supporting index causes a sort spill to tempdb on large sets.
- `sp_executesql` with concatenated SQL is still injection-prone.
- Forgetting `SET NOCOUNT ON` in procs (extra round-trips).
- Big `IN (...)` lists - use TVPs or `STRING_SPLIT`.

## Examples in this folder

- [Schemas/Schema.sql](./Schemas/Schema.sql)
- [Indexes/Indexes.sql](./Indexes/Indexes.sql)
- [Indexes/IndexUsageQueries.sql](./Indexes/IndexUsageQueries.sql)
- [StoredProcedures/usp_GetOrders.sql](./StoredProcedures/usp_GetOrders.sql)
- [StoredProcedures/usp_UpsertCustomer.sql](./StoredProcedures/usp_UpsertCustomer.sql)
- [Functions/Functions.sql](./Functions/Functions.sql)
- [Views/Views.sql](./Views/Views.sql)
- [Migrations/2026_01_AddIsActive_Expand.sql](./Migrations/2026_01_AddIsActive_Expand.sql)

## See also

- [EntityFramework](../EntityFramework/README.md)
- [Dapper](../Dapper/README.md)
- Microsoft Learn: SQL Server 2025 vector type docs
