# Design — News Feed (Twitter/X-style)

## Core decision: fan-out

| Strategy | When |
|---|---|
| **Fan-out on write (push)** | Most users have few followers; reads dominate |
| **Fan-out on read (pull)** | Some users have millions of followers (celebrities) |
| **Hybrid** | Push for normal users, pull for celebrities; merge on read |

## Architecture

```
Tweet POST → Tweet Service → SQL (canonical store)
                            ↘ Pub/Sub
                                ├─ Fan-out worker → Redis sorted-set per follower (timeline cache)
                                ├─ Search indexer → Elasticsearch
                                └─ Analytics

GET /home → Timeline Service → Redis (hot)  → fall back to: query celebrity feeds + merge
                            ↘ Cache miss   → rebuild from per-user feeds + persist
```

## Data model

```
tweets(id, author_id, content, created_at, ...)            -- canonical
followers(user_id, followed_id)                            -- edge list
timeline:<user_id> (Redis sorted set)                      -- tweet_id → score=created_at
```

## Hot paths

- `POST /tweets` writes once, fan-out async via queue (don't block the writer)
- `GET /home` returns top N from Redis sorted set; if cold, rebuild
- Celebrity tweets bypass fan-out — read-side merge via per-author cache

## Bottlenecks

- **Celebrity fan-out** — 100M followers × write → unacceptable. Hybrid solves.
- **Timeline cache cold-start** — exponential decay TTL; refill on first hit
- **Hot key** for celebrity reads — read-replica fanout / CDN

## Tradeoffs

- Push: fast reads, expensive writes
- Pull: cheap writes, expensive reads
- Eventual consistency on the timeline is acceptable; users tolerate 1-2s lag

## See also

- [../../Architecture/Messaging](../../Architecture/Messaging/) · [../../Database/Caching](../../Database/Caching/) · [../../Database/Redis](../../Database/Redis/)
