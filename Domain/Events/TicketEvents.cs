using Domain.Entities;

namespace Domain.Events;

public record TicketsReservedEvent : DomainEvent
{
    public required Ticket Ticket { get; init; }
    public required int Quantity { get; init; }
}

public record TicketsReleasedEvent : DomainEvent
{
    public required Ticket Ticket { get; init; }
    public required int Quantity { get; init; }
    public required string Reason { get; init; }
}

public record TicketSoldOutEvent : DomainEvent
{
    public required Ticket Ticket { get; init; }
}