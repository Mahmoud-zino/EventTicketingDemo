using Application.Commands;
using Application.Interfaces;
using Domain.Exceptions;
using Mediator;

namespace Application.CommandHandlers;

public sealed class CancelReservationCommandHandler(IReservationRepository reservationRepository): IRequestHandler<CancelReservationCommand>
{
    public async ValueTask<Unit> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        if (reservation is null)
        {
            throw new ReservationNotFoundException(request.ReservationId);
        }
        
        reservation.Cancel();
        await reservationRepository.UpdateAsync(reservation, cancellationToken);
        return Unit.Value;
    }
}