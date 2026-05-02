// ArrayPoolExample.cs
// Rent buffers; ALWAYS return them in finally / using.

using System.Buffers;

namespace BackEnd.CSharp.Performance;

public sealed class StreamCopier
{
    public async Task<long> CopyAsync(Stream src, Stream dst, CancellationToken ct)
    {
        var pool = ArrayPool<byte>.Shared;
        var buf = pool.Rent(81_920);
        long total = 0;
        try
        {
            int read;
            while ((read = await src.ReadAsync(buf.AsMemory(0, buf.Length), ct).ConfigureAwait(false)) > 0)
            {
                await dst.WriteAsync(buf.AsMemory(0, read), ct).ConfigureAwait(false);
                total += read;
            }
            return total;
        }
        finally
        {
            pool.Return(buf, clearArray: false);
        }
    }
}
