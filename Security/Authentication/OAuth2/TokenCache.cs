// Trivial in-process token cache. In real systems use MSAL token cache or a
// distributed cache (Redis) with encryption (Data Protection).

public sealed class TokenCache
{
    private readonly Lock _gate = new();
    private string? _token;
    private DateTimeOffset _expiresAtUtc;

    public bool TryGet(out string? token)
    {
        lock (_gate)
        {
            if (_token is not null && DateTimeOffset.UtcNow < _expiresAtUtc)
            {
                token = _token;
                return true;
            }

            token = null;
            return false;
        }
    }

    public void Set(string token, TimeSpan ttl)
    {
        lock (_gate)
        {
            _token = token;
            _expiresAtUtc = DateTimeOffset.UtcNow + ttl;
        }
    }
}
