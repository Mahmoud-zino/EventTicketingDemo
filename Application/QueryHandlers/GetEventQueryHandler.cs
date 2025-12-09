using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using Domain.Enums;
using Domain.Exceptions;
using Mediator;

namespace Application.QueryHandlers;

public sealed class GetEventQueryHandler(IEventRepository eventRepository): IRequestHandler<GetEventQuery, EventDetailsDto>
{
    public async ValueTask<EventDetailsDto> Handle(GetEventQuery request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (@event is null)
        {
            throw new EventNotFoundException(request.EventId);
        }

        var now = DateTime.UtcNow;
        return new EventDetailsDto
        {
            Id = @event.Id,
            Name = @event.Name,
            Description = @event.Description,
            Venue = @event.Venue,
            EventDate = @event.EventDate,
            SalesStartDate = @event.SalesStartDate,
            SalesEndDate = @event.SalesEndDate,
            Tickets = @event.Tickets.Select(t => new TicketDto
            {
                Id = t.Id,
                EventId = t.Event.Id,
                Description = t.Description,
                Price = t.Price,
                TotalQuantity = t.TotalQuantity,
                AvailableQuantity = t.AvailableQuantity,
                ReservedQuantity = t.ReservedQuantity,
                IsSoldOut = t.AvailableQuantity == 0
            }).ToList(),
            TotalTicketsAvailable = @event.Tickets.Sum(t => t.AvailableQuantity),
            TotalTicketsReserved = @event.Tickets.Sum(t => t.ReservedQuantity),
            Status = @event.Status,
            OrganizerId = @event.OrganizerId,
            IsSoldOut = @event.IsSoldOut(),
            IsAvailableForPurchase = @event.Status == EventStatus.Published
                                     && now >= @event.SalesStartDate
                                     && now <= @event.SalesEndDate,
            HasSalesStarted = now >= @event.SalesStartDate,
            HasSalesEnded = now > @event.SalesEndDate,
            CreatedAt = @event.CreatedAt,
            UpdatedAt = @event.UpdatedAt
        };
    }
}