// Domain event: something the business cares about, named in past tense.
// Lives inside the aggregate's bounded context; integration events are derived from these.

namespace Architecture.DDD;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}

public sealed record OrderPlacedEvent(
    OrderId OrderId,
    CustomerId CustomerId,
    Money Total,
    DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record OrderLineAddedEvent(
    OrderId OrderId,
    string Sku,
    int Quantity,
    Money LineTotal,
    DateTimeOffset OccurredAt) : IDomainEvent;
