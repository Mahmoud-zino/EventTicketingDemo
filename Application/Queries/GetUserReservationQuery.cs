using Application.DTOs;
using MediatR;

namespace Application.Queries;

public record GetUserReservationQuery: IRequest<IList<ReservationDto>>, IRequest<List<ReservationDto>>
{
    public required string UserId { get; init; }
}