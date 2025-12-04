using Application.Commands;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.CommandHandlers;

public class ConfirmReservationCommandHandler(IReservationRepository reservationRepository): IRequestHandler<ConfirmReservationCommand>
{
    public async Task Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        if (reservation is null)
        {
            throw new ReservationNotFoundException(request.ReservationId);
        }
        
        // normally with something like PaymentId, we would verify the payment here
        reservation.Confirm();
        
        await reservationRepository.UpdateAsync(reservation, cancellationToken);
    }
}