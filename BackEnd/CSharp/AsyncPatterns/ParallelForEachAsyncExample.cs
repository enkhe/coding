// ParallelForEachAsyncExample.cs
// Bounded fan-out across a sequence with a single CancellationToken.

namespace BackEnd.CSharp.AsyncPatterns;

public sealed class FanOut(HttpClient http)
{
    public async Task<IReadOnlyList<int>> FetchSizesAsync(
        IEnumerable<string> urls, CancellationToken ct)
    {
        var sizes = new System.Collections.Concurrent.ConcurrentBag<int>();

        await Parallel.ForEachAsync(
            urls,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = 8,
                CancellationToken = ct,
            },
            async (url, token) =>
            {
                using var resp = await http.GetAsync(url, token).ConfigureAwait(false);
                var bytes = await resp.Content.ReadAsByteArrayAsync(token).ConfigureAwait(false);
                sizes.Add(bytes.Length);
            }).ConfigureAwait(false);

        return [.. sizes];
    }
}
