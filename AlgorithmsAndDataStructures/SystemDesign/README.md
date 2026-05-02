# System Design

> Architectural primitives for "design X" interviews and real-world distributed systems.

## Approach (any prompt)

1. **Clarify** — functional + non-functional reqs, scale, read/write ratios
2. **API** — endpoints / message contracts
3. **Data model** — schema + access patterns
4. **High-level architecture** — components + data flow
5. **Deep dive** — pick the hardest part and design it
6. **Bottlenecks** — identify, propose mitigations
7. **Tradeoffs** — be explicit; no architecture is free

## Primitives

| Concern | Tools |
|---|---|
| Load balancing | L4 (TCP) vs L7 (HTTP); sticky vs round-robin |
| Sharding | By key (consistent hashing), range, geo |
| Replication | Leader-follower, multi-leader, leaderless (Dynamo) |
| Caching | Cache-aside, read-through, write-through, **CDN** |
| Pub/sub | Kafka, Pulsar, Service Bus, SNS+SQS |
| Workflow | Saga (orchestration / choreography), Step Functions, Temporal |
| Rate limiting | Token bucket, sliding window |
| Idempotency | Idempotency keys, dedup tables |
| Search | Elasticsearch / OpenSearch, Azure AI Search |
| Time-series | InfluxDB, TimescaleDB, Prometheus |

## CAP / PACELC

- **CAP**: under partition, choose between Consistency or Availability
- **PACELC**: extends to "no partition: latency vs consistency"
- Real systems pick per-feature, not per-system

## Catalog (worked designs)

- [`design-url-shortener.md`](design-url-shortener.md)
- [`design-rate-limiter.md`](design-rate-limiter.md)
- [`design-newsfeed.md`](design-newsfeed.md)
- [`design-chat-system.md`](design-chat-system.md)

## See also

- [../Algorithms](../Algorithms/) · [../DataStructures](../DataStructures/) · [../../Architecture](../../Architecture/)
