-- pgvector — DDL + HNSW index + hybrid search example.
-- Postgres 16+, pgvector 0.7+
CREATE EXTENSION IF NOT EXISTS vector;
CREATE EXTENSION IF NOT EXISTS pg_trgm;        -- for trigram lexical fallback

CREATE TABLE doc_chunks (
    id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id   uuid NOT NULL,
    doc_id      uuid NOT NULL,
    content     text NOT NULL,
    embedding   vector(1536) NOT NULL,        -- OpenAI text-embedding-3-small dims
    tsv         tsvector GENERATED ALWAYS AS (to_tsvector('english', content)) STORED,
    created_at  timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX doc_chunks_tenant_idx ON doc_chunks (tenant_id);
CREATE INDEX doc_chunks_tsv_idx    ON doc_chunks USING gin (tsv);

-- HNSW for cosine distance. Tune m and ef_construction for recall vs build time.
CREATE INDEX doc_chunks_embedding_idx
    ON doc_chunks USING hnsw (embedding vector_cosine_ops)
    WITH (m = 16, ef_construction = 64);

-- Per-session search-time recall knob.
SET hnsw.ef_search = 100;

-- Pure vector retrieval, top 10.
-- :q is the query embedding; :tid is the tenant.
SELECT id, doc_id, content, 1 - (embedding <=> :q) AS similarity
FROM doc_chunks
WHERE tenant_id = :tid
ORDER BY embedding <=> :q
LIMIT 10;

-- Hybrid: lexical (BM25-ish via tsvector) + vector, fused with reciprocal rank.
WITH lex AS (
    SELECT id, ROW_NUMBER() OVER (ORDER BY ts_rank_cd(tsv, plainto_tsquery('english', :q_text)) DESC) AS r
    FROM doc_chunks
    WHERE tenant_id = :tid AND tsv @@ plainto_tsquery('english', :q_text)
    LIMIT 50
),
vec AS (
    SELECT id, ROW_NUMBER() OVER (ORDER BY embedding <=> :q_vec) AS r
    FROM doc_chunks
    WHERE tenant_id = :tid
    LIMIT 50
)
SELECT c.id, c.content, COALESCE(1.0/(60+l.r), 0) + COALESCE(1.0/(60+v.r), 0) AS score
FROM doc_chunks c
LEFT JOIN lex l ON l.id = c.id
LEFT JOIN vec v ON v.id = c.id
WHERE l.id IS NOT NULL OR v.id IS NOT NULL
ORDER BY score DESC
LIMIT 10;
