// Cross-cutting pipeline behaviors — logging, validation, transactions.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Mediator.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> log)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var name = typeof(TRequest).Name;
        log.LogInformation("Handling {Request}", name);
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            return await next();
        }
        finally
        {
            sw.Stop();
            log.LogInformation("Handled {Request} in {Elapsed:N0}ms", name, sw.ElapsedMilliseconds);
        }
    }
}

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!validators.Any()) return await next();

        var ctx = new ValidationContext<TRequest>(request);
        var failures = (await Task.WhenAll(validators.Select(v => v.ValidateAsync(ctx, ct))))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next();
    }
}

// Wraps the handler in an EF Core transaction for write commands.
public sealed class TransactionBehavior<TRequest, TResponse>(DbContext db)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        // Skip queries (convention: queries end in "Query").
        if (typeof(TRequest).Name.EndsWith("Query")) return await next();

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var response = await next();
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return response;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
