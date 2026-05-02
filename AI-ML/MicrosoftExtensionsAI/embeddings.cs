// Generate embeddings via IEmbeddingGenerator<string, Embedding<float>>.

using Microsoft.Extensions.AI;
using OpenAI;

var openAi = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
IEmbeddingGenerator<string, Embedding<float>> embedder =
    openAi.AsEmbeddingGenerator("text-embedding-3-small");

string[] docs =
[
    "The .NET runtime supports generic specialization.",
    "PostgreSQL pgvector enables HNSW indexes for ANN.",
    "Claude Sonnet 4.7 is Anthropic's mid-tier frontier model.",
];

GeneratedEmbeddings<Embedding<float>> result = await embedder.GenerateAsync(docs);

for (int i = 0; i < docs.Length; i++)
{
    ReadOnlyMemory<float> vec = result[i].Vector;
    Console.WriteLine($"[{i}] dim={vec.Length} preview={string.Join(',', vec.Span[..4].ToArray())}...");
}

// Cosine similarity helper
static float Cosine(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
{
    float dot = 0, na = 0, nb = 0;
    for (int i = 0; i < a.Length; i++)
    {
        dot += a[i] * b[i];
        na += a[i] * a[i];
        nb += b[i] * b[i];
    }
    return dot / (MathF.Sqrt(na) * MathF.Sqrt(nb));
}

float sim01 = Cosine(result[0].Vector.Span, result[1].Vector.Span);
Console.WriteLine($"sim(doc0, doc1) = {sim01:F4}");
