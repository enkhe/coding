using System.Net.Http.Headers;

public interface ITokenStore
{
    string? AccessToken { get; set; }
}

public sealed class MemoryTokenStore : ITokenStore
{
    public string? AccessToken { get; set; }
}

/// <summary>
/// DelegatingHandler that attaches a bearer token and retries once on 401.
/// Registered with <c>AddHttpMessageHandler&lt;AuthHandler&gt;()</c>.
/// </summary>
public sealed class AuthHandler(ITokenStore tokens) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        if (tokens.AccessToken is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var response = await base.SendAsync(request, ct);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // hook: refresh token, then retry once
            // tokens.AccessToken = await RefreshAsync(ct);
            // return await base.SendAsync(request, ct);
        }
        return response;
    }
}
