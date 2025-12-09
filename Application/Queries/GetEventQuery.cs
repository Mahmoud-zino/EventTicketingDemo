using Application.DTOs;
using Mediator;

namespace Application.Queries;

public record GetEventQuery: IRequest<EventDetailsDto>
{
    public required string EventId { get; init; }
}