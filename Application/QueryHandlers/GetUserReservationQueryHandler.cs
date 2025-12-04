using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using Domain.Enums;
using MediatR;

namespace Application.QueryHandlers;

public class GetUserReservationQueryHandler(IReservationRepository reservationRepository): IRequestHandler<GetUserReservationQuery, List<ReservationDto>>
{
    public async Task<List<ReservationDto>> Handle(GetUserReservationQuery request, CancellationToken cancellationToken)
    {
        var reservations = await reservationRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        var now = DateTime.UtcNow;
        
        return reservations
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                EventId = r.Event.Id,
                EventName = r.Event.Name,
                EventVenue = r.Event.Venue,
                EventDate = r.Event.EventDate,
                TicketId = r.Ticket.Id,
                Quantity = r.Quantity,
                PricePerTicket = r.PricePerTicket,
                TotalPrice = r.TotalPrice,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                ExpiresAt = r.Status == ReservationStatus.Pending ? r.ExpiresAt : null,
                ConfirmedAt = r.ConfirmedAt,
                CancelledAt = r.CancelledAt,
                IsExpired = r.IsExpired(),
                MinutesUntilExpiry = r.Status == ReservationStatus.Pending 
                    ? Math.Max(0, (int)(r.ExpiresAt - now).TotalMinutes)
                    : null,
                CanBeCancelled = r.Status == ReservationStatus.Pending && !r.IsExpired(),
                CanBeConfirmed = r.Status == ReservationStatus.Pending && !r.IsExpired()
            })
            .ToList();
    }
}