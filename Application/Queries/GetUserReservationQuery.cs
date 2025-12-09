using Application.DTOs;
using Mediator;

namespace Application.Queries;

public record GetUserReservationQuery: IRequest<List<ReservationDto>>
{
    public required string UserId { get; init; }
}