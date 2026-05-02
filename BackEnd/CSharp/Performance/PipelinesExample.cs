// PipelinesExample.cs
// System.IO.Pipelines: read newline-delimited records from a Stream
// without intermediate buffer copies.

using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace BackEnd.CSharp.Performance;

public sealed class LineReader
{
    public async Task<int> CountLinesAsync(Stream input, CancellationToken ct)
    {
        var reader = PipeReader.Create(input);
        var count = 0;

        while (true)
        {
            var result = await reader.ReadAsync(ct).ConfigureAwait(false);
            var buffer = result.Buffer;

            while (TryReadLine(ref buffer, out _))
                count++;

            reader.AdvanceTo(buffer.Start, buffer.End);
            if (result.IsCompleted) break;
        }

        await reader.CompleteAsync().ConfigureAwait(false);
        return count;
    }

    private static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
        var pos = buffer.PositionOf((byte)'\n');
        if (pos is null) { line = default; return false; }
        line = buffer.Slice(0, pos.Value);
        buffer = buffer.Slice(buffer.GetPosition(1, pos.Value));
        return true;
    }
}
