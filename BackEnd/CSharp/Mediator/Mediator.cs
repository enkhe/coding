// Minimal in-process mediator — no MediatR dependency.
// ~100 LOC, supports: requests with response, notifications (multi-handler), pipeline behaviors.
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public interface IRequest<TResponse> { }
public interface INotification { }

public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct);
}

public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    Task HandleAsync(TNotification notification, CancellationToken ct);
}

public interface IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct);
}

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

public interface IMediator
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken ct = default)
        where TNotification : INotification;
}

public sealed class Mediator(IServiceProvider sp) : IMediator
{
    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = sp.GetRequiredService(handlerType);
        var behaviors = ((IEnumerable<object>)sp.GetServices(behaviorType)).Reverse().ToArray();

        RequestHandlerDelegate<TResponse> pipeline = () => (Task<TResponse>)handlerType
            .GetMethod("HandleAsync")!.Invoke(handler, [request, ct])!;

        foreach (var behavior in behaviors)
        {
            var current = pipeline;
            pipeline = () => (Task<TResponse>)behaviorType
                .GetMethod("HandleAsync")!.Invoke(behavior, [request, current, ct])!;
        }

        return await pipeline();
    }

    public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken ct = default)
        where TNotification : INotification
    {
        var handlers = sp.GetServices<INotificationHandler<TNotification>>();
        // Fan-out, await all (so failures surface).
        await Task.WhenAll(handlers.Select(h => h.HandleAsync(notification, ct)));
    }
}

public static class MediatorRegistration
{
    public static IServiceCollection AddMediator(this IServiceCollection services, params Type[] markers)
    {
        services.AddScoped<IMediator, Mediator>();

        foreach (var marker in markers)
        {
            var asm = marker.Assembly;
            // Register all IRequestHandler<,> and INotificationHandler<>
            foreach (var type in asm.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface))
            {
                foreach (var iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType) continue;
                    var def = iface.GetGenericTypeDefinition();
                    if (def == typeof(IRequestHandler<,>) || def == typeof(INotificationHandler<>))
                    {
                        services.AddScoped(iface, type);
                    }
                }
            }
        }
        return services;
    }
}
