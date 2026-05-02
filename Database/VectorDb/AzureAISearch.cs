// Azure AI Search — hybrid vector + lexical + semantic ranker.
// Package: Azure.Search.Documents
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace VectorDb;

public sealed class AzureAISearchExample
{
    public static async Task EnsureIndexAsync(SearchIndexClient indexClient, string name, CancellationToken ct = default)
    {
        var index = new SearchIndex(name)
        {
            Fields =
            {
                new SimpleField("id", SearchFieldDataType.String) { IsKey = true, IsFilterable = true },
                new SearchableField("title")  { IsFilterable = false },
                new SearchableField("body")   { IsFilterable = false },
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
                Profiles   = { new VectorSearchProfile("hnsw", "hnsw-cfg") },
            },
            SemanticSearch = new()
            {
                Configurations =
                {
                    new SemanticConfiguration("default-semantic",
                        new SemanticPrioritizedFields
                        {
                            TitleField = new SemanticField("title"),
                            ContentFields = { new SemanticField("body") },
                        })
                }
            }
        };

        await indexClient.CreateOrUpdateIndexAsync(index, cancellationToken: ct);
    }

    public static async Task<IReadOnlyList<SearchResult<Doc>>> HybridAsync(
        SearchClient client,
        string tenantId,
        string queryText,
        ReadOnlyMemory<float> queryVector,
        int k = 10,
        CancellationToken ct = default)
    {
        var options = new SearchOptions
        {
            Size = k,
            Filter = $"tenant_id eq '{tenantId}'",
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
                        Fields = { "embedding" }
                    }
                }
            }
        };

        var response = await client.SearchAsync<Doc>(queryText, options, ct);
        var list = new List<SearchResult<Doc>>();
        await foreach (var r in response.Value.GetResultsAsync()) list.Add(r);
        return list;
    }
}

public sealed record Doc(string Id, string Title, string Body, string Tenant_Id);
