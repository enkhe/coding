// Azure AI Search — hybrid search with semantic ranker.
// Package: Azure.Search.Documents
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace VectorSearch;

public sealed class AzureAISearchExample(SearchClient client)
{
    public async Task<IReadOnlyList<SearchResult<Doc>>> HybridAsync(
        ReadOnlyMemory<float> queryVector, string queryText, int k = 10, CancellationToken ct = default)
    {
        var options = new SearchOptions
        {
            Size = k,
            QueryType = SearchQueryType.Semantic,
            SemanticSearch = new SemanticSearchOptions
            {
                SemanticConfigurationName = "default-semantic",
                QueryCaption = new QueryCaption(QueryCaptionType.Extractive),
                QueryAnswer = new QueryAnswer(QueryAnswerType.Extractive),
            },
            VectorSearch = new VectorSearchOptions
            {
                Queries =
                {
                    new VectorizedQuery(queryVector)
                    {
                        KNearestNeighborsCount = 50,
                        Fields = { "embedding" },
                    }
                }
            }
        };

        var results = await client.SearchAsync<Doc>(queryText, options, ct);
        var list = new List<SearchResult<Doc>>();
        await foreach (var r in results.Value.GetResultsAsync()) list.Add(r);
        return list;
    }

    // Index creation (one-shot setup).
    public static async Task EnsureIndexAsync(SearchIndexClient indexClient, string name, CancellationToken ct = default)
    {
        var index = new SearchIndex(name)
        {
            Fields =
            {
                new SimpleField("id", SearchFieldDataType.String) { IsKey = true, IsFilterable = true },
                new SearchableField("content") { IsFilterable = false },
                new SearchField("embedding", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                {
                    VectorSearchDimensions = 1536,
                    VectorSearchProfileName = "hnsw",
                },
                new SimpleField("tenant_id", SearchFieldDataType.String) { IsFilterable = true },
            },
            VectorSearch = new()
            {
                Algorithms = { new HnswAlgorithmConfiguration("hnsw-cfg") },
                Profiles = { new VectorSearchProfile("hnsw", "hnsw-cfg") },
            },
            SemanticSearch = new()
            {
                Configurations =
                {
                    new SemanticConfiguration("default-semantic",
                        new SemanticPrioritizedFields { ContentFields = { new("content") } })
                }
            },
        };
        await indexClient.CreateOrUpdateIndexAsync(index, cancellationToken: ct);
    }
}

public sealed record Doc(string Id, string Content, string Tenant_Id);
