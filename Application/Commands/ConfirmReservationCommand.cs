using Mediator;

namespace Application.Commands;

public record ConfirmReservationCommand: IRequest
{
    public required string ReservationId { get; init; }
    public required string PaymentId { get; init; }
}