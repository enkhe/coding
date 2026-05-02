// In-memory vector store with Microsoft.Extensions.VectorData abstractions.
//
// dotnet add package Microsoft.SemanticKernel.Connectors.InMemory
// dotnet add package Microsoft.Extensions.VectorData.Abstractions

using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Embeddings;

public sealed class DocChunk
{
    [VectorStoreKey] public required string Id { get; set; }
    [VectorStoreData] public required string Text { get; set; }
    [VectorStoreData] public required string Source { get; set; }
    [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}

class Program
{
    static async Task Main()
    {
        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-5", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!)
            .AddOpenAIEmbeddingGenerator("text-embedding-3-small",
                Environment.GetEnvironmentVariable("OPENAI_API_KEY")!)
            .Build();

        var embedder = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

        var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, DocChunk>("docs");
        await collection.EnsureCollectionExistsAsync();

        string[] docs =
        {
            "Mongolia is bordered by Russia and China.",
            "The Gobi Desert spans southern Mongolia.",
            "Genghis Khan founded the Mongol Empire in 1206.",
        };

        foreach (var (text, i) in docs.Select((t, i) => (t, i)))
        {
            var emb = await embedder.GenerateEmbeddingAsync(text);
            await collection.UpsertAsync(new DocChunk
            {
                Id = i.ToString(),
                Text = text,
                Source = $"doc{i}.md",
                Embedding = emb,
            });
        }

        var queryEmb = await embedder.GenerateEmbeddingAsync("Where is Gobi?");
        await foreach (var hit in collection.SearchAsync(queryEmb, top: 3))
            Console.WriteLine($"{hit.Score:F3}  {hit.Record.Text}");
    }
}
