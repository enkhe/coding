# Vector Databases

> Persistence + similarity search at scale. Choose by ops complexity, recall, and how close to your relational data you need it.

## Landscape

| Store | Strengths | Weaknesses | When to choose |
|---|---|---|---|
| **pgvector** | Lives next to your data; transactional; cheap | Slower than dedicated for very large sets | You're on Postgres |
| **SQL Server 2025 vector** | Same; Microsoft shop | Newer, fewer integrations | You're on SQL Server |
| **Azure AI Search** | Hybrid + semantic ranker built-in; indexers | More moving parts; cost | Mixed text + vector + filters in Azure |
| **Qdrant** | Fast HNSW; payload filters; OSS | Self-host or hosted-only | Standalone, good defaults |
| **Pinecone** | Hosted, simple | Vendor lock-in | Fast start, ops minimization |
| **Weaviate** | Schema-aware; modules | Heavier | Schema-rich, multi-modal |
| **Milvus** | Massive scale | Complex ops | Very large fleets |

## Distance metrics quick map

- **Cosine** — text embeddings (default). Magnitude-invariant.
- **Dot product** — when vectors are already normalized.
- **L2 (Euclidean)** — image features, where magnitude carries information.

## "To Be Dangerous" Cheatsheet

| Question | Answer |
|---|---|
| Index for < 100k items | Flat (exact) is fine |
| Index for 100k–10M | HNSW (most stores' default) |
| Index for > 10M | IVFPQ, ScaNN, or shard with HNSW |
| Tune recall | HNSW `ef_construction` (build), `ef_search` (query) |
| Hybrid retrieval | BM25 + vector + RRF; or rely on the store's built-in (AI Search) |
| Multi-tenant | Filter by `tenant_id` BEFORE the vector op (pre-filter > post-filter) |

## Quick Reference (pgvector)

See [`pgvector-schema.sql`](pgvector-schema.sql) for DDL + hybrid query.

## Quick Reference (Qdrant in C#)

```csharp
using Qdrant.Client;
using Qdrant.Client.Grpc;

var client = new QdrantClient("localhost", 6334);

await client.CreateCollectionAsync("docs", new VectorParams
{
    Size = 1536,
    Distance = Distance.Cosine,
});

await client.UpsertAsync("docs", new[]
{
    new PointStruct
    {
        Id = new PointId { Uuid = Guid.NewGuid().ToString() },
        Vectors = new[] { 0.1f, 0.2f, /*...*/ },
        Payload = { ["tenant_id"] = "t1", ["doc_id"] = "d1" },
    },
});

var results = await client.SearchAsync(
    "docs",
    new[] { 0.1f, 0.2f, /*...*/ },
    filter: MatchKeyword("tenant_id", "t1"),
    limit: 10);
```

## Common Pitfalls

- Mixing embedding models = vectors not comparable. Re-embed on model change.
- No per-tenant filter → cross-tenant leakage.
- HNSW recall tested only on synthetic data → tune on real distributions.
- Unbounded `payload` → memory blowup; keep payload small (foreign keys, not full docs).

## Examples in this folder

- [`pgvector-schema.sql`](pgvector-schema.sql) — DDL + hybrid query
- [`Qdrant.cs`](Qdrant.cs) — quickstart
- [`AzureAISearch.cs`](AzureAISearch.cs) — index + hybrid search

## See also

- [../../AI-ML/VectorSearch](../../AI-ML/VectorSearch/) · [../../AI-ML/RAG](../../AI-ML/RAG/) · [../PostgreSQL](../PostgreSQL/)
