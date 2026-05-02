// Source-generated logger pattern (zero-alloc, compile-time-validated).
// Why: faster than ILogger.LogXxx, structured fields preserved, no boxing of values.
using Microsoft.Extensions.Logging;

namespace Orders.Api;

internal static partial class Log
{
    [LoggerMessage(EventId = 1001, Level = LogLevel.Information,
        Message = "Order {OrderId} placed by {UserId} for {Amount:C}")]
    public static partial void OrderPlaced(ILogger logger, Guid orderId, Guid userId, decimal amount);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Warning,
        Message = "Payment retry {Attempt}/{Max} for {OrderId}")]
    public static partial void PaymentRetry(ILogger logger, int attempt, int max, Guid orderId);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Error,
        Message = "Order {OrderId} failed at stage {Stage}")]
    public static partial void OrderFailed(ILogger logger, Exception ex, Guid orderId, string stage);

    // SkipEnabledCheck = true for very hot paths where you have already checked IsEnabled.
    [LoggerMessage(EventId = 9999, Level = LogLevel.Trace,
        Message = "Cache hit for {Key}", SkipEnabledCheck = true)]
    public static partial void CacheHit(ILogger logger, string key);
}
