// OAuth2 Client Credentials grant - acquire a token, then call a protected API.
// In production prefer Microsoft.Identity.Client (MSAL) or Microsoft.Identity.Web's
// ITokenAcquirer; this is the raw HTTP version for clarity.

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

public sealed class ClientCredentialsExample(IHttpClientFactory httpFactory, TokenCache cache)
{
    private const string TokenEndpoint =
        "https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token";

    public async Task<string> CallOrdersApiAsync(string ordersUrl, CancellationToken ct = default)
    {
        var token = await GetAccessTokenAsync(ct);

        using var http = httpFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, ordersUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct);
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken ct)
    {
        if (cache.TryGet(out var cached)) return cached!;

        using var http = httpFactory.CreateClient();
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID")!,
            ["client_secret"] = Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET")!,
            ["scope"] = "api://orders/.default"
        });

        using var response = await http.PostAsync(TokenEndpoint, form, ct);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct)
            ?? throw new InvalidOperationException("empty token response");

        // Refresh ~60s before expiry.
        cache.Set(payload.AccessToken, TimeSpan.FromSeconds(payload.ExpiresIn - 60));
        return payload.AccessToken;
    }

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn,
        [property: JsonPropertyName("token_type")] string TokenType);
}
