// BufferWriterExample.cs
// Write JSON straight into an IBufferWriter<byte>.
// ArrayBufferWriter<byte> is the simplest implementation.

using System.Buffers;
using System.Text.Json;

namespace BackEnd.CSharp.Performance;

public static class BufferWriterDemo
{
    public static byte[] WriteUser(int id, string name)
    {
        var sink = new ArrayBufferWriter<byte>(initialCapacity: 128);
        using (var writer = new Utf8JsonWriter(sink))
        {
            writer.WriteStartObject();
            writer.WriteNumber("id", id);
            writer.WriteString("name", name);
            writer.WriteEndObject();
        }
        return sink.WrittenSpan.ToArray();
    }
}
