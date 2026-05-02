// JsonContext.cs
// JsonSerializerContext + [JsonSerializable] -> all metadata generated at build.
// Required for NativeAOT; recommended even for JIT scenarios.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackEnd.CSharp.SourceGenerators;

public sealed record Order(int Id, string Customer, decimal Total, DateTimeOffset PlacedAt);

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(Order[]))]
[JsonSerializable(typeof(IReadOnlyList<Order>))]
public partial class OrdersJsonContext : JsonSerializerContext;

public static class OrderJson
{
    public static string ToJson(Order o) =>
        JsonSerializer.Serialize(o, OrdersJsonContext.Default.Order);

    public static Order? FromJson(string s) =>
        JsonSerializer.Deserialize(s, OrdersJsonContext.Default.Order);
}
