// BFF (Backend-for-Frontend) — aggregating endpoint per UI.
// One BFF per client (web app, mobile app, partner). Avoids chatty calls from the UI.
using System.Security.Claims;

public static class DashboardBff
{
    public static void MapDashboard(this IEndpointRouteBuilder app)
    {
        app.MapGet("/me/dashboard", async (
            ClaimsPrincipal user,
            IOrdersClient orders,
            INotificationsClient notifs,
            IBillingClient billing,
            CancellationToken ct) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Fan-out in parallel; BFF tolerates partial failure with sensible defaults.
            var ordersTask = SafeAsync(() => orders.RecentForAsync(userId, ct), Array.Empty<OrderSummary>());
            var notifsTask = SafeAsync(() => notifs.UnreadForAsync(userId, ct), 0);
            var balanceTask = SafeAsync(() => billing.BalanceForAsync(userId, ct), 0m);

            await Task.WhenAll(ordersTask, notifsTask, balanceTask);

            return Results.Ok(new
            {
                orders = ordersTask.Result,
                unreadNotifications = notifsTask.Result,
                accountBalance = balanceTask.Result,
            });
        }).RequireAuthorization("default");
    }

    private static async Task<T> SafeAsync<T>(Func<Task<T>> op, T fallback)
    {
        try { return await op(); }
        catch { return fallback; }
    }
}

public interface IOrdersClient { Task<OrderSummary[]> RecentForAsync(Guid userId, CancellationToken ct); }
public interface INotificationsClient { Task<int> UnreadForAsync(Guid userId, CancellationToken ct); }
public interface IBillingClient { Task<decimal> BalanceForAsync(Guid userId, CancellationToken ct); }
public sealed record OrderSummary(Guid Id, decimal Total, DateTimeOffset PlacedAt);
