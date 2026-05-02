// EF Core 10 + pgvector — embed a query, retrieve top-K by cosine distance.
// Packages:
//   Pgvector
//   Pgvector.EntityFrameworkCore
//   Microsoft.EntityFrameworkCore
//   Microsoft.Extensions.AI
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace VectorSearch;

public sealed class DocChunk
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid DocId { get; set; }
    public string Content { get; set; } = "";
    public Vector Embedding { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class RagDb(DbContextOptions<RagDb> o) : DbContext(o)
{
    public DbSet<DocChunk> Chunks => Set<DocChunk>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasPostgresExtension("vector");
        b.Entity<DocChunk>(e =>
        {
            e.ToTable("doc_chunks");
            e.Property(x => x.Embedding).HasColumnType("vector(1536)");
        });
    }
}

public sealed record SearchHit(Guid Id, Guid DocId, string Content, double Similarity);

public sealed class VectorRetriever(RagDb db, IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    public async Task<IReadOnlyList<SearchHit>> SearchAsync(
        Guid tenantId, string query, int k = 10, CancellationToken ct = default)
    {
        var queryEmb = await embedder.GenerateAsync(query, cancellationToken: ct);
        var qVec = new Vector(queryEmb.Vector.ToArray());

        return await db.Chunks
            .Where(c => c.TenantId == tenantId)
            .OrderBy(c => c.Embedding.CosineDistance(qVec))
            .Take(k)
            .Select(c => new SearchHit(c.Id, c.DocId, c.Content, 1.0 - c.Embedding.CosineDistance(qVec)))
            .ToListAsync(ct);
    }
}
