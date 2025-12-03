using Domain.Entities;

namespace Domain.Events;

public record EventCreatedEvent : DomainEvent
{
    public required Event Event { get; init; }
}

public record EventPublishedEvent : DomainEvent
{
    public required Event Event { get; init; }
}

public record EventCancelledEvent : DomainEvent
{
    public required Event Event { get; init; }
    public required string Reason { get; init; }
}