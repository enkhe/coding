"""
RAG eval with Ragas — faithfulness, relevance, recall.
pip install ragas datasets
"""
from datasets import Dataset
from ragas import evaluate
from ragas.metrics import (
    faithfulness,
    answer_relevancy,
    context_recall,
    context_precision,
)

# Each row: question, the contexts your retriever returned, your generated answer,
# and the ground-truth answer (for recall/precision).
samples = {
    "question": [
        "What is the .NET 10 LTS support window?",
        "When does .NET 8 reach end of support?",
    ],
    "answer": [
        "Through November 10, 2028.",
        ".NET 8 reaches end of support on November 10, 2026.",
    ],
    "contexts": [
        [".NET 10 (released Nov 11, 2025) is supported through Nov 10, 2028."],
        ["Both .NET 8 and .NET 9 reach end-of-support on Nov 10, 2026."],
    ],
    "ground_truth": [
        "November 10, 2028",
        "November 10, 2026",
    ],
}

ds = Dataset.from_dict(samples)
result = evaluate(
    ds,
    metrics=[faithfulness, answer_relevancy, context_recall, context_precision],
)
print(result)
result.to_pandas().to_csv("rag-eval-results.csv", index=False)
