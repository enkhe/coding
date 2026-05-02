# Azure AI Search

> Hybrid search service: BM25 + vector + semantic ranker, plus indexers from Blob/SQL/Cosmos.

## Core Concepts

- **Index** — schema (fields), config (analyzers, vector profile, semantic config)
- **Field types** — `Edm.String`, `Collection(Edm.Single)` for vectors, complex types
- **Indexer** — pulls from a data source (Blob, SQL DB, Cosmos), runs skillsets, populates the index
- **Skillsets** — built-in OCR, entity recognition, embeddings (Azure OpenAI), or custom (REST)
- **Semantic ranker** — neural re-rank of top results; pricing tier dependent
- **Vector search** — HNSW with cosine/dot/L2

## Quick Reference

```csharp
var indexClient = new SearchIndexClient(new Uri("https://my.search.windows.net"), credential);
var searchClient = new SearchClient(new Uri("https://my.search.windows.net"), "docs", credential);

var options = new SearchOptions
{
    Size = 10,
    QueryType = SearchQueryType.Semantic,
    SemanticSearch = new() { SemanticConfigurationName = "default-semantic" },
    VectorSearch = new()
    {
        Queries = { new VectorizedQuery(queryEmbedding) { KNearestNeighborsCount = 50, Fields = { "embedding" } } }
    },
    Filter = "tenant_id eq 't1'",
};
var results = await searchClient.SearchAsync<Doc>(queryText, options);
```

## Common Pitfalls

- Querying without a filter on `tenant_id` → cross-tenant leakage
- Forgetting to enable `Replicas` for query throughput / `Partitions` for index size
- Putting full document body in the index (slow updates) — keep slim, store body in Blob

## Examples in this folder

- [`Search.cs`](Search.cs) — hybrid query
- [`indexer.json`](indexer.json) — Blob indexer with embedding skill

## See also

- [../../../AI-ML/VectorSearch](../../../AI-ML/VectorSearch/) · [../../../Database/VectorDb](../../../Database/VectorDb/) · [../../../AI-ML/RAG](../../../AI-ML/RAG/)
