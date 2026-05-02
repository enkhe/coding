# AI-ML

> Master index for the AI-ML pillar — LLMs, RAG, agents, classical ML, MLOps. Aligned with the .NET 2026 senior/architect roadmap.

## Core Concepts

- **LLM era split** — generative (Claude, GPT, Gemini, Llama) vs classical ML (scikit-learn, ML.NET, PyTorch).
- **Provider-agnostic .NET** — `Microsoft.Extensions.AI` is the new abstraction layer (`IChatClient`, `IEmbeddingGenerator`).
- **RAG over fine-tune** — for most enterprise workloads, retrieval beats fine-tuning on cost, freshness, and citation quality.
- **Vector + lexical hybrid** — pure vector loses on exact matches; combine with BM25 + RRF.
- **Agents = LLM + tools + loop** — the loop and tool surface matter more than the model.
- **Eval is product** — without golden sets and regression suites, every prompt change is a coin flip.

## "To Be Dangerous" Cheatsheet

### Sampling parameters

| Param | Range | Effect | When |
|---|---|---|---|
| `temperature` | 0.0-2.0 | Randomness | 0 for extraction/classification, 0.7 for chat, 1.0+ for creative |
| `top_p` | 0.0-1.0 | Nucleus sampling | Use instead of temperature, not both |
| `max_tokens` | int | Output cap | Always set; cost + latency control |
| `stop` | string[] | Hard stop sequences | For structured output framing |
| `seed` | int | Determinism (best-effort) | Reproducibility in eval |

### RAG vs Fine-tune

| Need | Choose | Why |
|---|---|---|
| Fresh/changing facts | RAG | No retrain |
| Citations required | RAG | Source linkage |
| Domain style/tone | Fine-tune | Behavior, not knowledge |
| Schema/format adherence | Structured output / fine-tune | Cheaper than schema in prompt at scale |
| Cost-sensitive at high volume | Fine-tune small model | Smaller distilled model |

### Embedding distance metrics

| Metric | Use | Notes |
|---|---|---|
| Cosine | Default for text embeddings | Magnitude-invariant |
| Dot product | Pre-normalized vectors | Faster, equivalent to cosine if unit-norm |
| L2 (Euclidean) | Image features, geometric | Magnitude matters |

### Agent loop

```
plan -> act (tool call) -> observe (tool result) -> reflect -> repeat
```

Stop conditions: max iterations, terminal tool, model returns no tool call.

### Prompt cache (Anthropic)

- Mark `cache_control: { type: "ephemeral" }` on stable prefix (system, large docs, tool defs).
- Cache hit = ~90% input cost reduction, ~85% latency reduction.
- TTL 5 min default, 1 hour with extended cache. Order matters: cache from longest stable prefix first.

### Evaluation discipline

| Stage | Method | Output |
|---|---|---|
| Smoke | Manual + handful of cases | Sanity |
| Regression | Golden dataset, deterministic checks | Pass/fail per case |
| Quality | LLM-as-judge with rubric | Score per dimension |
| Production | Sampling + human review | Drift detection |

### Cost telemetry to log

- Tokens in/out per call (prompt + completion + cached).
- Latency p50/p95.
- Tool call count per request.
- Model + version (cost shifts on swap).
- Cache hit rate.

## Folder Index

| Folder | Topic |
|---|---|
| [LLMs](LLMs/) | Model families, sampling, function calling, structured output |
| [MicrosoftExtensionsAI](MicrosoftExtensionsAI/) | `IChatClient`, embeddings, middleware pipeline |
| [SemanticKernel](SemanticKernel/) | Kernel, plugins, planners, agents |
| [RAG](RAG/) | Pipeline, chunking, hybrid search, reranking |
| [VectorSearch](VectorSearch/) | pgvector, SQL Server vector, Azure AI Search, Qdrant |
| [Agents](Agents/) | Agent loop, ReAct, multi-agent orchestration |
| [PromptEngineering](PromptEngineering/) | Few-shot, CoT, caching, guardrails |
| [Evaluation](Evaluation/) | Golden sets, LLM-as-judge, RAG metrics |
| [MachineLearning](MachineLearning/) | Classical ML with scikit-learn |
| [MLNet](MLNet/) | ML.NET 4 — `MLContext`, ONNX |
| [DeepLearning](DeepLearning/) | NN basics, PyTorch |
| [NLP](NLP/) | Tokenization, transformers, Hugging Face |
| [ComputerVision](ComputerVision/) | CNNs, torchvision, OpenCV |
| [MLOps](MLOps/) | MLflow, registries, deployment, monitoring |
| [Notebooks](Notebooks/) | Polyglot Notebooks conventions |

## Common Pitfalls

- Picking the biggest model when a small model + good prompt + RAG would win.
- Vector search without hybrid lexical fallback — fails on names, IDs, exact terms.
- No eval harness — you cannot improve what you cannot measure.
- Caching the whole prompt instead of structuring stable-prefix-first.
- Logging only success; missing tool-call traces makes agent debugging impossible.

## See also

- [.NET 2026 Senior/Architect Roadmap](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [Microsoft.Extensions.AI docs](https://learn.microsoft.com/dotnet/ai/)
- [Anthropic prompt caching](https://docs.anthropic.com/en/docs/build-with-claude/prompt-caching)
