using Application.DTOs;
using Mediator;

namespace Application.Queries;

public record GetAvailableEventsQuery: IRequest<List<EventSummaryDto>>
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}