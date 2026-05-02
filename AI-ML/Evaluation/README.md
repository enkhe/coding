# LLM Evaluation

> Don't ship LLM features without an eval harness. Vibes are not a regression test.

## Core Concepts

- **Golden dataset** — hand-curated input/expected pairs you trust.
- **Deterministic metrics** — exact match, F1, BLEU, ROUGE. Cheap, blunt.
- **Reference-free metrics** — coherence, fluency, length, refusal-rate.
- **LLM-as-judge** — another model scores responses on rubrics. Useful but not free, can drift.
- **RAG-specific:**
  - **Faithfulness** — answer is supported by retrieved context (no hallucination)
  - **Relevance** — retrieved context relevant to the question
  - **Context recall** — fraction of needed info retrieved
- **Regression suite** — run on every change to prompts/models/embeddings.
- **Online eval** — production telemetry (thumbs, edits, deflection) closes the loop.

## "To Be Dangerous" Cheatsheet

| Tool | Strengths |
|---|---|
| **Promptfoo** | YAML configs, providers map, snapshot testing |
| **DeepEval** | Python-native, RAG metrics, pytest integration |
| **Ragas** | Specialized RAG evals (faithfulness, relevance, recall) |
| **Inspect** | Anthropic's eval framework |
| **Custom xUnit** | Inline in your dotnet test suite for deterministic checks |

## Quick Reference (Promptfoo YAML)

```yaml
prompts:
  - file://prompts/classifier.txt
providers:
  - id: anthropic:claude-opus-4-7
  - id: openai:gpt-5
tests:
  - description: refund intent
    vars:
      input: "I want my money back."
    assert:
      - type: javascript
        value: JSON.parse(output).category === "REFUND"
      - type: cost
        threshold: 0.001
  - description: ambiguous
    vars:
      input: "Hello?"
    assert:
      - type: javascript
        value: ["INQUIRY","OTHER"].includes(JSON.parse(output).category)
```

## Quick Reference (xUnit golden tests)

```csharp
[Theory]
[ClassData(typeof(GoldenDataset))]
public async Task Classifier_matches_golden(string input, string expectedCategory)
{
    var result = await _classifier.ClassifyAsync(input);
    Assert.Equal(expectedCategory, result.Category);
}
```

## RAG eval (Ragas, Python)

```python
from ragas import evaluate
from ragas.metrics import faithfulness, answer_relevancy, context_recall

results = evaluate(
    dataset,                                  # questions, contexts, answers, ground_truth
    metrics=[faithfulness, answer_relevancy, context_recall],
)
print(results)
```

## Common Pitfalls

- "Eval set is the prod logs" — biased toward what you already do well.
- Letting the same model judge itself — biased high.
- Ignoring cost/latency in scoring — you'll ship a $$$ "best" model.
- No CI gate on regressions — quality slowly rots.
- Evaluating on the prompt you trained the prompt with.

## Examples in this folder

- [`promptfoo.yaml`](promptfoo.yaml)
- [`golden-tests.cs`](golden-tests.cs)
- [`ragas-eval.py`](ragas-eval.py)

## See also

- [../PromptEngineering](../PromptEngineering/) · [../RAG](../RAG/) · [../../Testing](../../Testing/)
