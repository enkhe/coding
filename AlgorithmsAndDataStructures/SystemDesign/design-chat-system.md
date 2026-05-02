# Design — Chat System

## Requirements

- 1:1 + group chats; presence; history; read receipts
- < 500ms message delivery
- Offline delivery (push notifications)

## Components

```
Client ── WebSocket ──> Edge Gateway (sticky) ──> Chat Service
                                                  ├─ presence (Redis: user → connected gateway)
                                                  ├─ Kafka topic: chat.messages
                                                  └─ Cassandra/HBase: messages by (channel_id, ts)
                                                       ↑
                                                       └─ Indexer for search (Elasticsearch)

Push Notifications ← Kafka consumer (when recipient offline)
```

## Data model

```
channels(id, type{1:1|group}, created_at)
channel_members(channel_id, user_id, last_read_ts)
messages(channel_id, ts ulid, sender_id, body, attachments_json)   -- partitioned by channel_id, sorted by ts
```

## Real-time delivery

- **WebSocket** for connected clients (or SSE for one-way)
- **Sticky sessions** — gateway routes new conn to assigned shard
- **Presence** in Redis: `user_id → gateway_node`; on send, look up recipient gateway and forward over internal RPC

## Storage

- Wide-column store (Cassandra) for time-ordered messages — append-only, easy horizontal scale
- ULID/snowflake IDs sort by time — primary key (channel_id, ulid)

## Tradeoffs

- WebSocket vs polling: WebSocket is harder to scale (connection state) but real-time-friendly
- Read receipts: per-receiver state (`last_read_ts` per channel member) is good enough; full delivery receipts can be expensive

## Bottlenecks

- **Group of 100k members** — fan-out write. Cap or use "supergroup" pull model (clients poll latest).
- **Hot channel** — partition by channel for storage; for delivery, dedicated workers per channel

## Common Pitfalls

- Synchronous DB writes on the message path → kills latency. Persist async after enqueue.
- Forgetting message ordering across reconnects — clients must dedupe by id
- Push notifications without batching → wakelocks / battery drain

## See also

- [../../Architecture/Messaging](../../Architecture/Messaging/) · [../../Database/Redis](../../Database/Redis/)
