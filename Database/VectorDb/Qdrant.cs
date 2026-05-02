// Qdrant in C# — collection, upsert, search with payload filter.
// Package: Qdrant.Client
using Qdrant.Client;
using Qdrant.Client.Grpc;
using static Qdrant.Client.Grpc.Conditions;

namespace VectorDb;

public sealed class QdrantExample
{
    private readonly QdrantClient _client;

    public QdrantExample(string host = "localhost", int port = 6334) =>
        _client = new QdrantClient(host, port);

    public async Task EnsureAsync(CancellationToken ct = default)
    {
        var collections = await _client.ListCollectionsAsync(ct);
        if (!collections.Contains("docs"))
        {
            await _client.CreateCollectionAsync("docs",
                new VectorParams { Size = 1536, Distance = Distance.Cosine },
                cancellationToken: ct);
        }
    }

    public async Task UpsertAsync(IEnumerable<(Guid Id, string TenantId, ReadOnlyMemory<float> Embedding, string Content)> rows, CancellationToken ct = default)
    {
        var points = rows.Select(r => new PointStruct
        {
            Id = new PointId { Uuid = r.Id.ToString() },
            Vectors = r.Embedding.ToArray(),
            Payload =
            {
                ["tenant_id"] = r.TenantId,
                ["content"] = r.Content,
            }
        }).ToList();

        await _client.UpsertAsync("docs", points, cancellationToken: ct);
    }

    public async Task<IReadOnlyList<(Guid Id, float Score, string Content)>> SearchAsync(
        string tenantId, ReadOnlyMemory<float> queryEmbedding, int k = 10, CancellationToken ct = default)
    {
        var hits = await _client.SearchAsync(
            "docs",
            queryEmbedding.ToArray(),
            filter: MatchKeyword("tenant_id", tenantId),
            limit: (ulong)k,
            cancellationToken: ct);

        return hits.Select(h =>
            (Guid.Parse(h.Id.Uuid), h.Score, h.Payload["content"].StringValue)).ToList();
    }
}
