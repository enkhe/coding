-- pgvector — DDL + HNSW + hybrid query (cross-link to AI-ML/VectorSearch).
CREATE EXTENSION IF NOT EXISTS vector;
CREATE EXTENSION IF NOT EXISTS pg_trgm;

CREATE TABLE IF NOT EXISTS items (
    id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id   uuid NOT NULL,
    title       text NOT NULL,
    body        text NOT NULL,
    embedding   vector(1536) NOT NULL,
    tsv         tsvector GENERATED ALWAYS AS
                 (to_tsvector('english', coalesce(title,'') || ' ' || coalesce(body,''))) STORED,
    created_at  timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS items_tenant_idx ON items (tenant_id);
CREATE INDEX IF NOT EXISTS items_tsv_idx    ON items USING gin (tsv);

CREATE INDEX IF NOT EXISTS items_embedding_idx
    ON items USING hnsw (embedding vector_cosine_ops)
    WITH (m = 16, ef_construction = 64);

-- Per-session recall knob.
SET hnsw.ef_search = 100;

-- Hybrid query (lexical + vector, fused with RRF k=60).
-- :q_text = the user's text; :q_vec = the embedded query (vector(1536)); :tid = tenant.
WITH lex AS (
    SELECT id, ROW_NUMBER() OVER (ORDER BY ts_rank_cd(tsv, plainto_tsquery('english', :q_text)) DESC) AS r
    FROM items
    WHERE tenant_id = :tid AND tsv @@ plainto_tsquery('english', :q_text)
    LIMIT 50
),
vec AS (
    SELECT id, ROW_NUMBER() OVER (ORDER BY embedding <=> :q_vec) AS r
    FROM items
    WHERE tenant_id = :tid
    LIMIT 50
)
SELECT i.id, i.title,
       COALESCE(1.0/(60+l.r), 0) + COALESCE(1.0/(60+v.r), 0) AS rrf_score
FROM items i
LEFT JOIN lex l ON l.id = i.id
LEFT JOIN vec v ON v.id = i.id
WHERE l.id IS NOT NULL OR v.id IS NOT NULL
ORDER BY rrf_score DESC
LIMIT 10;
