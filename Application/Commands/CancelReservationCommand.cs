using Mediator;

namespace Application.Commands;

public record CancelReservationCommand: IRequest
{
    public required string ReservationId { get; init; }
}