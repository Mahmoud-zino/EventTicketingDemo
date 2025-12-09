using Application.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Mediator;

namespace Application.CommandHandlers;

public sealed class CreateEventCommandHandler(IEventRepository eventRepository): IRequestHandler<CreateEventCommand, string>
{
    public async ValueTask<string> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = new Event
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            Venue = request.Venue,
            EventDate = request.EventDate,
            SalesStartDate = request.SalesStartDate,
            SalesEndDate = request.SalesEndDate,
            OrganizerId = request.OrganizerId,
            Status = EventStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        @event.Tickets = request.Tickets.Select(t => new Ticket
        {
            Id = Guid.NewGuid().ToString(),
            Event = @event,
            Description = t.Description,
            Price = t.Price,
            TotalQuantity = t.TotalQuantity,
            AvailableQuantity = t.TotalQuantity,
            ReservedQuantity = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        }).ToList();

        @event.Status = EventStatus.Published;

        var eventId = await eventRepository.AddAsync(@event, cancellationToken);
        return eventId;
    }
}