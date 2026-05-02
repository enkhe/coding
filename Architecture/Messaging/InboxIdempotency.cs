// Manual inbox table — when you don't have framework outbox/inbox.
// Idea: persist (MessageId, ConsumerName) before processing; PK violation = dedupe.
using Microsoft.EntityFrameworkCore;

namespace Messaging.Inbox;

public sealed class InboxRecord
{
    public required Guid MessageId { get; init; }
    public required string Consumer { get; init; }
    public DateTimeOffset ReceivedAt { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class IdempotencyDbContext(DbContextOptions<IdempotencyDbContext> o) : DbContext(o)
{
    public DbSet<InboxRecord> Inbox => Set<InboxRecord>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<InboxRecord>().HasKey(x => new { x.MessageId, x.Consumer });
        b.Entity<InboxRecord>().Property(x => x.Consumer).HasMaxLength(200);
    }
}

public sealed class IdempotentConsumer(IdempotencyDbContext db, ILogger<IdempotentConsumer> log)
{
    public async Task<bool> TryHandleAsync(
        Guid messageId,
        string consumerName,
        Func<Task> handler,
        CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            db.Inbox.Add(new InboxRecord { MessageId = messageId, Consumer = consumerName });
            await db.SaveChangesAsync(ct);    // PK violation → dupe
        }
        catch (DbUpdateException) when (IsUniqueViolation(db))
        {
            log.LogDebug("Duplicate message {MessageId} for {Consumer}", messageId, consumerName);
            await tx.RollbackAsync(ct);
            return false;
        }

        await handler();
        await tx.CommitAsync(ct);
        return true;
    }

    private static bool IsUniqueViolation(DbContext db) =>
        true; // narrow this per-provider in real code (e.g., SqlException.Number == 2627)
}
