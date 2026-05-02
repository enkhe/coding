// ChannelExample.cs
// Bounded Channel<T> for producer/consumer with backpressure.

using System.Threading.Channels;

namespace BackEnd.CSharp.AsyncPatterns;

public sealed class JobPipeline
{
    private readonly Channel<Job> _channel = Channel.CreateBounded<Job>(
        new BoundedChannelOptions(capacity: 100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false,
        });

    public ChannelWriter<Job> Writer => _channel.Writer;

    public async Task RunWorkersAsync(int workers, CancellationToken ct)
    {
        var tasks = Enumerable.Range(0, workers)
            .Select(_ => Task.Run(() => ConsumeAsync(ct), ct));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private async Task ConsumeAsync(CancellationToken ct)
    {
        await foreach (var job in _channel.Reader.ReadAllAsync(ct).ConfigureAwait(false))
        {
            await Process(job, ct).ConfigureAwait(false);
        }
    }

    private static Task Process(Job j, CancellationToken ct) => Task.Delay(10, ct);
}

public sealed record Job(int Id, string Payload);
