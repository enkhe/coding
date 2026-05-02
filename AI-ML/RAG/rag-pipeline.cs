// End-to-end RAG with Microsoft.Extensions.AI + EF Core 10 vector type (pgvector backend).
//
// dotnet add package Microsoft.Extensions.AI
// dotnet add package Microsoft.EntityFrameworkCore
// dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
// dotnet add package Pgvector.EntityFrameworkCore

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Pgvector;
using Pgvector.EntityFrameworkCore;

public class DocChunk
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public required string Source { get; set; }
    [Column(TypeName = "vector(1536)")]
    public Vector Embedding { get; set; } = new(new float[1536]);
}

public class RagDb(DbContextOptions<RagDb> opts) : DbContext(opts)
{
    public DbSet<DocChunk> Chunks => Set<DocChunk>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasPostgresExtension("vector");
        b.Entity<DocChunk>().HasIndex(c => c.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");
    }
}

public sealed class RagService(RagDb db, IChatClient chat,
    IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    public async Task IngestAsync(IEnumerable<(string text, string source)> docs)
    {
        foreach (var batch in docs.Chunk(64))
        {
            var embeddings = await embedder.GenerateAsync(batch.Select(d => d.text));
            for (int i = 0; i < batch.Length; i++)
                db.Chunks.Add(new DocChunk
                {
                    Text = batch[i].text,
                    Source = batch[i].source,
                    Embedding = new Vector(embeddings[i].Vector.ToArray()),
                });
        }
        await db.SaveChangesAsync();
    }

    public async Task<string> AskAsync(string query, int topK = 5)
    {
        var qEmb = new Vector((await embedder.GenerateAsync([query])).First().Vector.ToArray());

        var hits = await db.Chunks
            .OrderBy(c => c.Embedding.CosineDistance(qEmb))
            .Take(topK)
            .Select(c => new { c.Id, c.Text, c.Source })
            .ToListAsync();

        var context = string.Join("\n\n",
            hits.Select(h => $"[doc:{h.Id} src:{h.Source}]\n{h.Text}"));

        var resp = await chat.GetResponseAsync(
        [
            new(ChatRole.System,
                "Answer using ONLY the provided context. Cite sources as [doc:N]. "
              + "If the context does not contain the answer, say you don't know."),
            new(ChatRole.User, $"Context:\n{context}\n\nQuestion: {query}")
        ],
        new ChatOptions { Temperature = 0f, MaxOutputTokens = 512 });

        return resp.Text;
    }
}
