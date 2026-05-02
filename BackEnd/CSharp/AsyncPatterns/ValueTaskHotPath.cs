// ValueTaskHotPath.cs
// When should you return ValueTask<T>?
//   - The method completes synchronously most of the time.
//   - Callers will await exactly once.
// Otherwise prefer Task<T>.

using System.Collections.Concurrent;

namespace BackEnd.CSharp.AsyncPatterns;

public sealed class CachedUserService(IUserRepository repo)
{
    private readonly ConcurrentDictionary<int, User> _cache = new();

    public ValueTask<User?> GetAsync(int id, CancellationToken ct)
    {
        if (_cache.TryGetValue(id, out var hit))
            return ValueTask.FromResult<User?>(hit);   // sync completion, no alloc

        return new ValueTask<User?>(LoadAndCacheAsync(id, ct));
    }

    private async Task<User?> LoadAndCacheAsync(int id, CancellationToken ct)
    {
        var u = await repo.FindAsync(id, ct).ConfigureAwait(false);
        if (u is not null) _cache[id] = u;
        return u;
    }
}

public interface IUserRepository
{
    Task<User?> FindAsync(int id, CancellationToken ct);
}

public sealed record User(int Id, string Email);
