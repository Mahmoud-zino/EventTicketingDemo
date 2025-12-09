using Application.DTOs;
using Mediator;

namespace Application.Queries;

public class GetReservationByIdQuery: IRequest<ReservationDto>
{
    public required string ReservationId { get; init; }
}