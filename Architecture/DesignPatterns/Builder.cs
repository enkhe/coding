// Builder: stepwise construction with validation, ideal for many optional parameters.
// In modern C#, records + `with` cover the easy case; use a builder when there are
// invariants to enforce or when the build process itself has steps/branches.

namespace Architecture.DesignPatterns.Builder;

public sealed record HttpRequestSpec(
    Uri Url,
    string Method,
    IReadOnlyDictionary<string, string> Headers,
    string? Body,
    TimeSpan Timeout);

public sealed class HttpRequestBuilder
{
    private Uri? _url;
    private string _method = "GET";
    private readonly Dictionary<string, string> _headers = new();
    private string? _body;
    private TimeSpan _timeout = TimeSpan.FromSeconds(30);

    public HttpRequestBuilder Url(string url) { _url = new Uri(url); return this; }
    public HttpRequestBuilder Method(string method) { _method = method; return this; }
    public HttpRequestBuilder Header(string k, string v) { _headers[k] = v; return this; }
    public HttpRequestBuilder Body(string body) { _body = body; return this; }
    public HttpRequestBuilder Timeout(TimeSpan t) { _timeout = t; return this; }

    public HttpRequestSpec Build()
    {
        if (_url is null) throw new InvalidOperationException("Url required");
        return new HttpRequestSpec(_url, _method, _headers, _body, _timeout);
    }
}

// Records-with (preferred when no invariant logic is needed):
// var req = new HttpRequestSpec(new("https://x"), "GET", new Dictionary<string,string>(), null, TimeSpan.FromSeconds(30));
// var withAuth = req with { Headers = new Dictionary<string,string> { ["Authorization"] = "Bearer …" } };
