using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using Domain.Enums;
using Domain.Exceptions;
using Mediator;

namespace Application.QueryHandlers;

public sealed class GetReservationByIdQueryHandler(IReservationRepository reservationRepository): IRequestHandler<GetReservationByIdQuery, ReservationDto>
{
    public async ValueTask<ReservationDto> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        if (reservation is null)
            throw new ReservationNotFoundException(request.ReservationId);
        
        var now = DateTime.UtcNow;
        
        return new ReservationDto
        {
            Id = reservation.Id,
            EventId = reservation.Event.Id,
            EventName = reservation.Event.Name,
            EventVenue = reservation.Event.Venue,
            EventDate = reservation.Event.EventDate,
            TicketId = reservation.Ticket.Id,
            Quantity = reservation.Quantity,
            PricePerTicket = reservation.PricePerTicket,
            TotalPrice = reservation.TotalPrice,
            Status = reservation.Status,
            CreatedAt = reservation.CreatedAt,
            ExpiresAt = reservation.Status == ReservationStatus.Pending ? reservation.ExpiresAt : null,
            ConfirmedAt = reservation.ConfirmedAt,
            CancelledAt = reservation.CancelledAt,
            IsExpired = reservation.IsExpired(),
            MinutesUntilExpiry = reservation.Status == ReservationStatus.Pending 
                ? Math.Max(0, (int)(reservation.ExpiresAt - now).TotalMinutes)
                : null,
            CanBeCancelled = reservation.Status == ReservationStatus.Pending && !reservation.IsExpired(),
            CanBeConfirmed = reservation.Status == ReservationStatus.Pending && !reservation.IsExpired()
        };
    }
}