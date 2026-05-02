# NLP

> Natural language processing. Pre-LLM: tokenize, embed, classify. With LLMs: prompt + structure + RAG.

## Tasks map

| Task | Pre-LLM tooling | LLM-era |
|---|---|---|
| Classification | scikit-learn + TF-IDF; fine-tuned BERT | Few-shot prompt + structured output |
| NER | spaCy, fine-tuned models | Prompt + JSON schema |
| Summarization | pegasus, BART | Prompt with summary style guide |
| Translation | MarianMT, NLLB | Prompt with style controls |
| Sentiment | small finetuned model | Few-shot prompt |
| Embeddings | `sentence-transformers` | OpenAI `text-embedding-3` family |

## Tokenization

```python
from transformers import AutoTokenizer
tok = AutoTokenizer.from_pretrained("bert-base-uncased")
ids = tok("Hello, world", padding=True, truncation=True, max_length=128, return_tensors="pt")
```

## Hugging Face quick reference

```python
from transformers import pipeline

# Zero-shot classification
zs = pipeline("zero-shot-classification", model="facebook/bart-large-mnli")
zs("I want my money back", candidate_labels=["refund", "complaint", "inquiry"])

# Fine-tuned model inference
clf = pipeline("text-classification", model="distilbert-base-uncased-finetuned-sst-2-english")
clf("This product is amazing")
```

## Embeddings

```python
from sentence_transformers import SentenceTransformer
model = SentenceTransformer("all-MiniLM-L6-v2")
embs = model.encode(["hello", "world"], normalize_embeddings=True)  # 384-dim
```

## When to fine-tune vs prompt

- **Prompt** — quick, no infra, works for 80%+ of tasks
- **Fine-tune** — need consistent style, higher throughput at lower cost, sensitive data
- **RAG** — knowledge that changes; cheaper than fine-tuning for facts

## See also

- [../LLMs](../LLMs/) · [../RAG](../RAG/) · [../VectorSearch](../VectorSearch/)
