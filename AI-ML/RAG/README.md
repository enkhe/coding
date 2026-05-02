# RAG (Retrieval-Augmented Generation)

> Inject fresh, source-cited context into LLM calls. Retrieval over fine-tune for facts that change.

## Pipeline

```
[ ingest ] -> [ chunk ] -> [ embed ] -> [ store (vector + lexical) ]
                                                |
                                                v
[ query rewrite ] -> [ retrieve (hybrid) ] -> [ rerank ] -> [ build context ] -> [ generate w/ citations ]
```

## Core Concepts

- **Chunking** — split source into ~256-1024 token windows with 10-20% overlap. Respect structure (headings, code blocks, sentences).
- **Embeddings** — `text-embedding-3-small` (1536d, cheap), `text-embedding-3-large` (3072d), `e5-mistral` (open). Same model for ingest + query.
- **Hybrid search** — vector (semantic) + BM25 (lexical) combined via Reciprocal Rank Fusion (RRF). Pure vector loses on names, IDs, exact terms.
- **Reranking** — cross-encoder (`ms-marco-MiniLM`, Cohere Rerank, Voyage Rerank) on top-k=20-50 to top-n=3-8. Big quality jump.
- **Query rewriting** — LLM expands abbreviations, decomposes multi-hop questions, generates HyDE pseudo-answers.
- **Citation discipline** — include source IDs in the prompt; instruct the model to cite `[doc_id]`. Reject answers without citations during eval.
- **Eval metrics** — faithfulness (no hallucinations), answer relevance, context precision/recall (Ragas).

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Chunk size | 512 tokens, 64 overlap, split on headers | Markdown/docs |
| Chunk size | Whole function/class, no overlap | Code |
| Embedding | `text-embedding-3-small` cosine | Default |
| Hybrid | Vector top-50 + BM25 top-50, RRF k=60 | Always |
| Rerank | Cohere `rerank-3` or local cross-encoder, top-5 to LLM | Quality bump worth latency |
| Query rewrite | Cheap model expands acronyms, splits multi-hop | Conversational |
| Citation | Pass `[doc_id]` in context, require in answer | Compliance, trust |
| Eval | Ragas faithfulness + relevance, golden Q/A set | Pre-prod gate |

## Quick Reference

### C# RAG with Microsoft.Extensions.AI + EF Core 10 vector

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

public class Doc
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public string Source { get; set; } = "";
    [Column(TypeName = "vector(1536)")] public float[] Embedding { get; set; } = [];
}

// Retrieve
var qEmb = (await embedder.GenerateAsync([query])).First().Vector.ToArray();
var hits = await db.Docs
    .OrderBy(d => EF.Functions.VectorDistance("cosine", d.Embedding, qEmb))
    .Take(5)
    .ToListAsync();

// Build context with citations
var context = string.Join("\n\n", hits.Select(h => $"[doc:{h.Id}]\n{h.Text}"));
var resp = await chat.GetResponseAsync(
    [new ChatMessage(ChatRole.System, "Answer using only the context. Cite as [doc:N]."),
     new ChatMessage(ChatRole.User, $"Context:\n{context}\n\nQuestion: {query}")],
    new ChatOptions { Temperature = 0f });
```

### Chunking heuristic (Python)

```python
def chunk_markdown(text, target=512, overlap=64):
    # split on H1/H2 first, then sentences if too long
    ...
```

## Common Pitfalls

- Different embedding models for ingest vs query — silent semantic drift.
- Tiny chunks (< 100 tokens) — no context, retriever returns noise.
- Huge chunks (> 2000 tokens) — context window blown, generation degrades.
- Pure vector search — fails on product codes, error messages, person names.
- No reranker — top-50 from ANN is noisy; LLM gets distracted.
- No citations enforced — model confabulates plausible answers.
- Rebuilding embeddings on every change — version your embedding model and chunks.

## Examples in this folder

- [`rag-pipeline.cs`](rag-pipeline.cs) — End-to-end ingest + retrieve + generate (EF Core 10 vector)
- [`chunking.py`](chunking.py) — Markdown header-aware chunker
- [`hybrid-rrf.py`](hybrid-rrf.py) — Vector + BM25 with reciprocal rank fusion
- [`query-rewrite.cs`](query-rewrite.cs) — HyDE-style query expansion

## See also

- [VectorSearch](../VectorSearch/) — pgvector / SQL Server / Azure AI Search
- [Evaluation](../Evaluation/) — Ragas-style metrics
- [PromptEngineering](../PromptEngineering/) — citation prompting
