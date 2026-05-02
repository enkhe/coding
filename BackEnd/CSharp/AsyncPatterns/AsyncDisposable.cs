// AsyncDisposable.cs
// Implement IAsyncDisposable when cleanup itself does I/O.
// Consumers use `await using`.

namespace BackEnd.CSharp.AsyncPatterns;

public sealed class BufferedWriter : IAsyncDisposable
{
    private readonly Stream _stream;
    private readonly List<byte> _buffer = new();

    public BufferedWriter(Stream stream) => _stream = stream;

    public void Write(ReadOnlySpan<byte> data) => _buffer.AddRange(data.ToArray());

    public async ValueTask DisposeAsync()
    {
        if (_buffer.Count > 0)
        {
            await _stream.WriteAsync(_buffer.ToArray()).ConfigureAwait(false);
            await _stream.FlushAsync().ConfigureAwait(false);
        }
        await _stream.DisposeAsync().ConfigureAwait(false);
    }
}

internal static class WriterDemo
{
    public static async Task RunAsync()
    {
        await using var w = new BufferedWriter(File.Create("out.bin"));
        w.Write([1, 2, 3]);
        // disposed asynchronously here -> flush + close
    }
}
