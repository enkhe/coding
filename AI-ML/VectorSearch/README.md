# Vector Search

> Find similar items by embedding-vector distance. The substrate of RAG and semantic search.

## Core Concepts

- **Embedding** — high-dimensional vector representation of text/image/audio. Same model = same space.
- **Distance metric** — `cosine` (default for text), `dot product` (when vectors are pre-normalized), `L2` (Euclidean).
- **Index types:**
  - **Flat** — exact, slow (O(N)), best for < 100k items.
  - **HNSW** (Hierarchical Navigable Small World) — fast, high-recall, more memory.
  - **IVF / IVFPQ** — partitioned + quantized, lower memory, recall trades.
- **Hybrid search** — BM25 lexical + vector + RRF (reciprocal rank fusion). Best of both.
- **Re-ranking** — cheap retrieval (top-K) + expensive cross-encoder rerank on the top-K.

## "To Be Dangerous" Cheatsheet

| Store | Strengths | When to use |
|---|---|---|
| **pgvector** | Lives next to your data; `JOIN` with relational data; cheap | You already use Postgres |
| **SQL Server 2025 `vector`** | Lives next to relational data on SQL Server | Microsoft shop |
| **Azure AI Search** | Hybrid search built-in, semantic ranker, indexers | Mixed text + vector + filters |
| **Qdrant** | Fast HNSW, payload filtering, GA stable | Standalone, OSS |
| **Pinecone** | Hosted, simple SDK, good for fast start | When ops minimization matters |
| **Weaviate** | Schema-aware, modules (BM25, OpenAI) | Multi-modal, schema-rich |

## Quick Reference (pgvector + ASP.NET Core 10)

```sql
CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE doc_chunks (
    id          uuid PRIMARY KEY,
    doc_id      uuid NOT NULL,
    content     text NOT NULL,
    embedding   vector(1536) NOT NULL,
    created_at  timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX doc_chunks_embedding_idx
    ON doc_chunks USING hnsw (embedding vector_cosine_ops)
    WITH (m = 16, ef_construction = 64);
```

```csharp
// EF Core 10 with Pgvector.EntityFrameworkCore
public sealed class DocChunk
{
    public Guid Id { get; set; }
    public Guid DocId { get; set; }
    public string Content { get; set; } = "";
    public Vector Embedding { get; set; } = null!;
}

var hits = await db.DocChunks
    .OrderBy(c => c.Embedding.CosineDistance(queryEmbedding))
    .Take(10)
    .ToListAsync(ct);
```

## Hybrid search (RRF)

Score each item by `1/(k+rank_lex) + 1/(k+rank_vec)` (typical k=60). Re-sort.

## Common Pitfalls

- Mixing embedding models — vectors from `text-embedding-3-small` are not comparable to `text-embedding-3-large`.
- Storing un-normalized vectors but using `dot product` distance.
- HNSW recall too low — tune `ef_construction` (build) and `ef_search` (query).
- Embedding chunks too large or too small — semantic loss either way; 256-512 tokens is a typical sweet spot.
- Forgetting filters — `WHERE tenant_id = $1 AND ...` before/after the vector op affects recall and performance.

## Examples in this folder

- [`pgvector-schema.sql`](pgvector-schema.sql) — DDL + index
- [`PgVectorExample.cs`](PgVectorExample.cs) — EF Core 10 query
- [`AzureAISearch.cs`](AzureAISearch.cs) — hybrid search with semantic ranker

## See also

- [../RAG](../RAG/) · [../MicrosoftExtensionsAI](../MicrosoftExtensionsAI/) · [../../Database/VectorDb](../../Database/VectorDb/)
