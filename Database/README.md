# Database

> Polyglot persistence reference: relational, document, key-value, vector, and caching layers for the .NET 2026 stack.

## Core Concepts

- **Polyglot persistence**: pick the storage that matches the access pattern, not the brand. Most production systems use 2-3 stores (OLTP relational + cache + search/vector).
- **CAP / PACELC**: under partition, choose Consistency vs Availability; in normal ops, pick Latency vs Consistency. Postgres/SQL Server lean CP; Cassandra/Mongo (default) lean AP.
- **OLTP vs OLAP vs HTAP**: row-store for transactional, columnstore for analytical, hybrid (SQL Server columnstore on memory-optimized, Postgres + Citus, Snowflake) for both.
- **ACID vs BASE**: relational gives ACID; eventually-consistent stores trade durability/latency.
- **Index = read tax on writes**: every index speeds reads and slows inserts/updates. Measure both.
- **Expand-Contract**: zero-downtime schema change in 3 phases (add new, dual-write/backfill, remove old).

## "To Be Dangerous" Cheatsheet

- Default isolation level for SQL Server is `READ COMMITTED` with locks; turn on `READ_COMMITTED_SNAPSHOT` to avoid blocking. Postgres default is `READ COMMITTED` with MVCC, no flag needed.
- The N+1 query is the #1 ORM bug. Detect with logging, fix with `Include` / `Join` / projection / batching.
- A clustered index defines physical row order. Pick narrow, increasing, unique, immutable (e.g., `bigint identity` or sequential GUID, not `newid()`).
- Covering index = `INCLUDE` non-key columns to avoid key lookup.
- `EXPLAIN ANALYZE` (Postgres) / Actual Execution Plan (SQL Server) are non-negotiable for any query > 100ms.
- Always parameterize. String concatenation = SQL injection + plan cache pollution.
- Cache invalidation: prefer TTL + versioned keys over manual purge.
- Vector + BM25 hybrid search with RRF (Reciprocal Rank Fusion) beats either alone for RAG.

## Quick Reference

### Polyglot decision matrix

| Need | Pick | Why |
|------|------|-----|
| Transactional writes, joins, reporting | SQL Server / Postgres | ACID, mature optimizer |
| Document with flexible shape | MongoDB / Postgres JSONB | Schema-on-read |
| Sub-ms key lookup, session, cache | Redis | In-memory, rich types |
| Full-text search | Postgres FTS / Elastic / Azure AI Search | Inverted index, BM25 |
| Vector similarity (RAG) | pgvector / Qdrant / Azure AI Search / SQL Server 2025 vector | ANN indexes (HNSW) |
| Time-series | TimescaleDB / InfluxDB | Compression, downsampling |
| Append-only events | EventStoreDB / Kafka + Postgres | Stream semantics |
| Embedded / edge | SQLite | Zero-ops, file-based |
| Graph traversal | Neo4j / SQL Server graph | Relationship-first queries |

### Topic index

- [SqlServer/](./SqlServer/README.md) - T-SQL, indexes, procs, migrations, SQL 2025 vector
- [EntityFramework/](./EntityFramework/README.md) - EF Core 10
- [Dapper/](./Dapper/README.md) - micro-ORM hot paths
- [PostgreSQL/](./PostgreSQL/README.md) - JSONB, pgvector, partitioning
- [MySQL/](./MySQL/README.md) - MySQL 8 essentials
- [SQLite/](./SQLite/README.md) - embedded scenarios
- [MongoDB/](./MongoDB/README.md) - document modeling, aggregation
- [Redis/](./Redis/README.md) - caching, streams, locks
- [Caching/](./Caching/README.md) - HybridCache, stampede protection
- [VectorDb/](./VectorDb/README.md) - pgvector, Qdrant, Azure AI Search

## Common Pitfalls

- Treating an ORM as a database. Always know the SQL it emits.
- Adding indexes without measuring write impact or verifying the optimizer uses them.
- Long-running transactions holding locks while waiting on external IO.
- Storing JSON blobs in relational columns and then needing to query inside them constantly (move to JSONB / document store).
- Cache without TTL or invalidation strategy = staleness bug factory.
- Using `SELECT *` over the wire from .NET — defeats covering indexes and ships unused bytes.
- Soft deletes without filtered indexes — every query pays the `WHERE IsDeleted = 0` tax.

## Examples in this folder

- [SqlServer/Schemas/Schema.sql](./SqlServer/Schemas/Schema.sql)
- [SqlServer/Indexes/Indexes.sql](./SqlServer/Indexes/Indexes.sql)
- [EntityFramework/AppDbContext.cs](./EntityFramework/AppDbContext.cs)
- [Dapper/OrderRepository.cs](./Dapper/OrderRepository.cs)
- [Redis/RedisExamples.cs](./Redis/RedisExamples.cs)
- [VectorDb/pgvector-schema.sql](./VectorDb/pgvector-schema.sql)

## See also

- [.NET 2026 roadmap](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [AI-ML/RAG](../AI-ML/) (cross-link from VectorDb)
