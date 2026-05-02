"""Hybrid retrieval: vector + BM25 fused with Reciprocal Rank Fusion (RRF).

pip install rank_bm25 numpy
"""
from __future__ import annotations
import numpy as np
from rank_bm25 import BM25Okapi


def rrf(rankings: list[list[str]], k: int = 60) -> list[tuple[str, float]]:
    """Reciprocal Rank Fusion. rankings: list of ranked doc-id lists, best first."""
    scores: dict[str, float] = {}
    for rank_list in rankings:
        for rank, doc_id in enumerate(rank_list):
            scores[doc_id] = scores.get(doc_id, 0.0) + 1.0 / (k + rank + 1)
    return sorted(scores.items(), key=lambda x: -x[1])


def cosine_search(
    query_vec: np.ndarray,
    doc_vecs: dict[str, np.ndarray],
    top_k: int = 50,
) -> list[str]:
    qn = query_vec / np.linalg.norm(query_vec)
    scored = []
    for doc_id, v in doc_vecs.items():
        scored.append((doc_id, float(np.dot(qn, v / np.linalg.norm(v)))))
    scored.sort(key=lambda x: -x[1])
    return [d for d, _ in scored[:top_k]]


def bm25_search(
    query: str,
    docs: dict[str, str],
    top_k: int = 50,
) -> list[str]:
    ids = list(docs.keys())
    tokenized = [docs[i].lower().split() for i in ids]
    bm25 = BM25Okapi(tokenized)
    scores = bm25.get_scores(query.lower().split())
    order = np.argsort(-scores)[:top_k]
    return [ids[i] for i in order]


def hybrid_retrieve(
    query: str,
    query_vec: np.ndarray,
    docs: dict[str, str],
    doc_vecs: dict[str, np.ndarray],
    top_k: int = 10,
) -> list[str]:
    v = cosine_search(query_vec, doc_vecs, top_k=50)
    b = bm25_search(query, docs, top_k=50)
    fused = rrf([v, b])
    return [doc_id for doc_id, _ in fused[:top_k]]


if __name__ == "__main__":
    docs = {"d1": "pgvector hnsw cosine", "d2": "ML.NET pipelines", "d3": "Claude tool use"}
    qvec = np.random.rand(8)
    dvecs = {k: np.random.rand(8) for k in docs}
    print(hybrid_retrieve("vector index", qvec, docs, dvecs, top_k=3))
