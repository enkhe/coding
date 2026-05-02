# MongoDB

> Document database. Schema design = embed vs reference. Aggregation pipeline is your friend.

## Schema design

| Pattern | When |
|---|---|
| **Embed** | Child rarely accessed alone, bounded size, lifecycle = parent |
| **Reference** | Child accessed independently, unbounded size, different lifecycle |
| **Bucket** | Time-series — bucket many time points per doc |
| **Outlier** | 1% of docs are huge — separate collection |

## "To Be Dangerous" Cheatsheet

| Need | Operation |
|---|---|
| Insert | `db.orders.insertOne({...})` |
| Find | `db.orders.find({ userId: "u1" })` |
| Update | `db.orders.updateOne({_id}, { $set: { status: "paid" } })` |
| Aggregation | `db.orders.aggregate([{$match}, {$group}, {$project}])` |
| Index | `db.orders.createIndex({ userId: 1, placedAt: -1 })` |
| Text index | `createIndex({ description: "text" })` |
| Geo | `createIndex({ loc: "2dsphere" })` |
| Transactions | `session.startTransaction()` (replicaset/sharded only) |
| Change streams | `db.orders.watch([{$match: {operationType: "insert"}}])` |

## Quick Reference (.NET driver)

```csharp
// Package: MongoDB.Driver
public sealed class Order
{
    [BsonId] [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = "";
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PlacedAt { get; set; }
}

var client = new MongoClient(connectionString);
var db = client.GetDatabase("orders");
var col = db.GetCollection<Order>("orders");

await col.InsertOneAsync(new Order { UserId = uid, Amount = 9.99m, PlacedAt = DateTime.UtcNow });

// Filter builder
var filter = Builders<Order>.Filter.Eq(o => o.UserId, uid)
           & Builders<Order>.Filter.Gte(o => o.PlacedAt, since);
var orders = await col.Find(filter)
    .SortByDescending(o => o.PlacedAt)
    .Limit(10)
    .ToListAsync();
```

## Common Pitfalls

- Massive embedded arrays → 16MB doc limit + lock contention. Reference instead.
- No index on common filter — scans the whole collection
- Transactions across shards — slow; redesign to single shard
- Multi-document writes assumed atomic — only within one document by default

## See also

- [../EntityFramework](../EntityFramework/) (different paradigm) · [../VectorDb](../VectorDb/)
