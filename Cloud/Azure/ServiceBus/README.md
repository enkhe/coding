# Service Bus

> Enterprise messaging: queues, topics/subscriptions, sessions, transactions, dead-letter, geo-DR.

## Core Concepts

- **Queue**: 1:1 producer/consumer; FIFO with sessions.
- **Topic + Subscriptions**: 1:N pub/sub with SQL filter rules per subscription.
- **Sessions**: ordered, single-consumer-per-session for FIFO and request/reply.
- **Peek-Lock** (default) vs **Receive-and-Delete**; complete / abandon / dead-letter / defer.
- **Dead-Letter Queue (DLQ)**: per queue or per subscription; reasons: max delivery count, TTL, filter eval errors.
- **Partitions**: scale beyond namespace limits (Premium); Premium also supports geo-DR via aliases.
- **Tiers**: Basic (queues only), Standard (topics, low cost), Premium (dedicated capacity, VNet, larger messages, geo-DR).

## "To Be Dangerous" Cheatsheet

- Always **idempotent receivers**: dedupe on `MessageId` or business key.
- Set **MaxDeliveryCount** to ~5; tune lock duration to 30-60s + auto-renew.
- Use **sessions** for per-key ordering (e.g. orders per customer).
- Use **scheduled enqueue** for retries with backoff instead of in-process sleeps.
- DLQ is a queue: monitor, alert on depth, build a replayer.
- Premium + **availability zones** + **paired-namespace geo-DR** for prod.
- Identity-based: `ServiceBus.Data.Sender` / `Receiver` data-plane RBAC.

## Quick Reference

| Concern | Setting |
|---|---|
| Order per key | sessions enabled, sessionId set on send |
| Exactly-once-ish | dedup window + idempotent handler |
| Big payloads (>256KB) | Premium 1MB-100MB or claim-check pattern |
| Pub/sub | Topic + SQL filter `properties.eventType = 'OrderCreated'` |
| Retry | abandon (immediate) or scheduled enqueue (delayed) |

## Common Pitfalls

- Catching exceptions and `Complete()`-ing poison messages -> silent loss.
- Using `ReceiveAndDelete` and crashing -> message lost.
- Holding lock too long; let auto-renew handle it.
- Picking Standard for a system that needs VNet -> upgrade to Premium.

## Examples in this folder

- [Sender.cs](./Sender.cs)
- [Processor.cs](./Processor.cs)

## See also

- [Functions](../Functions/README.md)
- [Storage queues](../Storage/README.md)
