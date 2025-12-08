using Domain.Entities;
using Infrastructure.Documents;

namespace Infrastructure.Mappers;

public static class EventMapper
{
    public static Event ToDomain(this EventDocument doc)
    {
        var @event = new Event
        {
            Id = doc.Id,
            Name = doc.Name,
            Description = doc.Description,
            Venue = doc.Venue,
            EventDate = doc.EventDate,
            SalesStartDate = doc.SalesStartDate,
            SalesEndDate = doc.SalesEndDate,
            OrganizerId = doc.OrganizerId,
            Status = doc.Status,
            Version = doc.Version,
            CreatedAt = doc.CreatedAt,
            UpdatedAt = doc.UpdatedAt
        };

        @event.Tickets = doc.Tickets.Select(t => t.ToDomain(@event)).ToList();
        return @event;
    }

    public static EventDocument ToDocument(this Event @event)
    {
        return new EventDocument
        {
            Id = @event.Id,
            Name = @event.Name,
            Description = @event.Description,
            Venue = @event.Venue,
            EventDate = @event.EventDate,
            SalesStartDate = @event.SalesStartDate,
            SalesEndDate = @event.SalesEndDate,
            OrganizerId = @event.OrganizerId,
            Status = @event.Status,
            Version = @event.Version,
            CreatedAt = @event.CreatedAt,
            UpdatedAt = @event.UpdatedAt,
            Tickets = @event.Tickets.Select(t => t.ToDocument()).ToList()
        };
    }
}