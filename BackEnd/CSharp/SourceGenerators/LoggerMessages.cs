// LoggerMessages.cs
// [LoggerMessage] generates an allocation-free, structured logger method.
// Format placeholders become structured event properties automatically.

using Microsoft.Extensions.Logging;

namespace BackEnd.CSharp.SourceGenerators;

public static partial class Log
{
    [LoggerMessage(EventId = 1000, Level = LogLevel.Information,
        Message = "Order {OrderId} accepted in {Elapsed}ms")]
    public static partial void OrderAccepted(ILogger logger, int orderId, long elapsed);

    [LoggerMessage(EventId = 1001, Level = LogLevel.Warning,
        Message = "Order {OrderId} rejected: {Reason}")]
    public static partial void OrderRejected(ILogger logger, int orderId, string reason);

    // Exception parameter conventionally named `ex` and last positional.
    [LoggerMessage(EventId = 1002, Level = LogLevel.Error,
        Message = "Order {OrderId} processing failed")]
    public static partial void OrderFailed(ILogger logger, int orderId, Exception ex);
}

// Instance-style: works when the surrounding class has an ILogger field.
public sealed partial class OrderProcessor(ILogger<OrderProcessor> logger)
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Processing order {OrderId}")]
    private partial void LogStart(int orderId);

    public void Process(int orderId)
    {
        LogStart(orderId);
        Log.OrderAccepted(logger, orderId, 12);
    }
}
