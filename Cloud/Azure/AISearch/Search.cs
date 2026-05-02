// Azure AI Search — hybrid query with semantic ranker.
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace Cloud.Azure.AISearch;

public sealed class HybridSearch
{
    private readonly SearchClient _client;

    public HybridSearch(string endpoint, string indexName) =>
        _client = new SearchClient(new Uri(endpoint), indexName, new DefaultAzureCredential());

    public async Task<IReadOnlyList<SearchResult<Doc>>> SearchAsync(
        string tenantId,
        string queryText,
        ReadOnlyMemory<float> queryEmbedding,
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
                QueryAnswer = new QueryAnswer(QueryAnswerType.Extractive),
            },
            VectorSearch = new VectorSearchOptions
            {
                Queries =
                {
                    new VectorizedQuery(queryEmbedding)
                    {
                        KNearestNeighborsCount = 50,
                        Fields = { "embedding" }
                    }
                }
            }
        };

        var response = await _client.SearchAsync<Doc>(queryText, options, ct);
        var list = new List<SearchResult<Doc>>();
        await foreach (var r in response.Value.GetResultsAsync()) list.Add(r);
        return list;
    }
}

public sealed record Doc(string Id, string Title, string Body, string Tenant_Id);
