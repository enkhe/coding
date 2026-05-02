// Cancellation.cs
// Discipline:
//   1. Accept CancellationToken on every public async method.
//   2. Pass it to every awaited call.
//   3. Combine per-request and per-call deadlines via linked sources.

namespace BackEnd.CSharp.AsyncPatterns;

public sealed class TimeoutDemo(HttpClient http)
{
    public async Task<string> GetWithDeadlineAsync(
        string url, TimeSpan timeout, CancellationToken requestCt)
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            requestCt, timeoutCts.Token);

        try
        {
            using var resp = await http.GetAsync(url, linked.Token).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync(linked.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            // distinguish caller cancel vs our timeout
            throw new TimeoutException($"GET {url} timed out after {timeout}.");
        }
    }
}
