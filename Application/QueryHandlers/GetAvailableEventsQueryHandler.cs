using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using Domain.Enums;
using Mediator;

namespace Application.QueryHandlers;

public sealed class GetAvailableEventsQueryHandler(IEventRepository eventRepository): IRequestHandler<GetAvailableEventsQuery, List<EventSummaryDto>>
{
    public async ValueTask<List<EventSummaryDto>> Handle(GetAvailableEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await eventRepository.GetAllAsync(cancellationToken);
        
        var now = DateTime.UtcNow;
        
        return events
            .Where(e => e.Status == EventStatus.Published)
            .Where(e => !request.FromDate.HasValue || e.EventDate >= request.FromDate.Value)
            .Where(e => !request.ToDate.HasValue || e.EventDate <= request.ToDate.Value)
            .Where(e => e.EventDate > now)
            .OrderBy(e => e.EventDate)
            .Select(e => new EventSummaryDto
            {
                Id = e.Id,
                Name = e.Name,
                Venue = e.Venue,
                EventDate = e.EventDate,
                Status = e.Status,
                TotalTicketsAvailable = e.Tickets.Sum(t => t.AvailableQuantity),
                MinTicketPrice = e.Tickets.Count != 0 ? e.Tickets.Min(t => t.Price) : 0,
                MaxTicketPrice = e.Tickets.Count != 0 ? e.Tickets.Max(t => t.Price) : 0,
                IsSoldOut = e.IsSoldOut(),
                IsAvailableForPurchase = now >= e.SalesStartDate && now <= e.SalesEndDate
            })
            .ToList();
    }
}