# Design — URL Shortener (bit.ly-style)

## Requirements

**Functional:**
- POST `/shorten` { long_url } → { short_code }
- GET `/<short_code>` → 301 redirect
- Optional: custom alias, analytics, expiration

**Non-functional:**
- 100M new URLs/day, 10:1 read:write
- < 100ms p99 redirect
- Highly available (redirect outage = brand outage)

## API

```
POST /api/shorten
Body: { "url": "https://...", "ttlDays": 365 }
→ 201 { "code": "aZ4kQ", "short": "https://co.ns/aZ4kQ", "expiresAt": "..." }

GET /<code>
→ 301 Location: <long-url>
```

## Data model

```
Table: links
  code        VARCHAR(10) PRIMARY KEY
  long_url    TEXT NOT NULL
  user_id     UUID NULL
  created_at  TIMESTAMPTZ NOT NULL
  expires_at  TIMESTAMPTZ NULL
  click_count BIGINT NOT NULL DEFAULT 0     -- updated async
```

## Code generation

- **Hash + suffix on collision**: `base62(SHA1(long_url))[0:7]`; if taken, try next chunk
- **Counter + base62**: distributed counter (e.g., DB sequence per shard); `base62(counter)`
- **Custom alias**: validate uniqueness in `links`

## Architecture

```
Client → CDN/edge cache (PURGE on update)
             │
             ▼
         API Gateway → Shorten Service → SQL primary
                                       ↘ Redis (write-through cache)

Client → CDN/edge cache → Redirect Service → Redis (hot) → SQL (cold)
                                          ↘ Async event → ClickStream → Analytics DB
```

## Deep dives

- **Redirect path**: avoid SQL on hot path. Cache `code → long_url` in Redis with no TTL (LRU-evicted). Cache miss → SQL → fill.
- **Click counting**: emit event to Kafka; aggregate in batch (avoid hot row contention on SQL).
- **Custom domains**: domain → tenant mapping at the gateway.
- **Abuse**: phishing detection on creation; rate limit per user/IP; allowlist auth for high-volume customers.

## Bottlenecks & mitigations

- **DB write hotspot on counter** → per-shard counters; or hash-based codes
- **Cache stampede** on a viral link → single-flight in Redis
- **Long-tail traffic** → tiered cache (CDN edge → regional Redis → DB)

## Tradeoffs

- Hash codes: deterministic, but expose URL similarity through structure
- Counter codes: shorter for early entries; reveals scale; needs coordination

## Cost back-of-envelope

- 100M/day × 365 = ~36B URLs / year. At 200 bytes/row = ~7 TB. Use Postgres + cold-store for old TTL'd rows.
- 1B redirects/day → 11.5K rps avg, ~50K rps peak. Cache hit rate >95% means SQL handles 0.5K-2.5K rps.

## See also

- [../../Architecture/Microservices](../../Architecture/Microservices/) · [../../Database/Caching](../../Database/Caching/)
