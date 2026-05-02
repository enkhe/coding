// AsyncStreams.cs
// IAsyncEnumerable<T> for paged or streamed results.
// Always mark the CancellationToken parameter with [EnumeratorCancellation]
// so callers can pass it via WithCancellation(...).

using System.Runtime.CompilerServices;

namespace BackEnd.CSharp.AsyncPatterns;

public sealed class PagedFeed
{
    public async IAsyncEnumerable<Item> ReadAllAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var cursor = 0;
        while (true)
        {
            var page = await FetchPageAsync(cursor, ct).ConfigureAwait(false);
            foreach (var item in page.Items)
            {
                ct.ThrowIfCancellationRequested();
                yield return item;
            }

            if (page.NextCursor is null) yield break;
            cursor = page.NextCursor.Value;
        }
    }

    private static Task<Page> FetchPageAsync(int cursor, CancellationToken ct)
    {
        // pretend network call
        var done = cursor >= 30;
        var page = new Page(
            Items: done ? [] : Enumerable.Range(cursor, 10).Select(i => new Item(i)).ToList(),
            NextCursor: done ? null : cursor + 10);
        return Task.FromResult(page);
    }
}

public sealed record Item(int Id);
public sealed record Page(IReadOnlyList<Item> Items, int? NextCursor);

internal static class StreamConsumer
{
    public static async Task RunAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var feed = new PagedFeed();
        await foreach (var item in feed.ReadAllAsync().WithCancellation(cts.Token))
            Console.WriteLine(item.Id);
    }
}
