using Domain.Entities;

namespace Domain.Events;

public record ReservationCreatedEvent : DomainEvent
{
    public required Reservation Reservation { get; init; }
}

public record ReservationConfirmedEvent : DomainEvent
{
    public required Reservation Reservation { get; init; }
}

public record ReservationCancelledEvent : DomainEvent
{
    public required Reservation Reservation { get; init; }
}

public record ReservationExpiredEvent : DomainEvent
{
    public required Reservation Reservation { get; init; }
}