using Domain.Entities;
using Infrastructure.Documents;

namespace Infrastructure.Mappers;

public static class TicketMapper
{
    public static Ticket ToDomain(this TicketDocument doc, Event @event)
    {
        return new Ticket
        {
            Id = doc.Id,
            Event = @event,
            Description = doc.Description,
            Price = doc.Price,
            TotalQuantity = doc.TotalQuantity,
            AvailableQuantity = doc.AvailableQuantity,
            ReservedQuantity = doc.ReservedQuantity,
            Version = doc.Version,
            CreatedAt = doc.CreatedAt,
            UpdatedAt = doc.UpdatedAt
        };
    }

    public static TicketDocument ToDocument(this Ticket ticket)
    {
        return new TicketDocument
        {
            Id = ticket.Id,
            EventId = ticket.Event.Id,
            Description = ticket.Description,
            Price = ticket.Price,
            TotalQuantity = ticket.TotalQuantity,
            AvailableQuantity = ticket.AvailableQuantity,
            ReservedQuantity = ticket.ReservedQuantity,
            Version = ticket.Version,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }
}