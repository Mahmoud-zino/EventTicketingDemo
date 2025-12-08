using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Documents;

namespace Infrastructure.Mappers;

public static class ReservationMapper
{
    public static async Task<Reservation> ToDomainAsync(
        this ReservationDocument doc, 
        IEventRepository eventRepository,
        CancellationToken ct)
    {
        var @event = await eventRepository.GetByIdAsync(doc.EventId, ct);
        if (@event is null)
            throw new EventNotFoundException(doc.EventId);

        var ticket = @event.Tickets.FirstOrDefault(t => t.Id == doc.TicketId);
        if (ticket is null)
            throw new TicketNotFoundException(doc.TicketId);

        return new Reservation
        {
            Id = doc.Id,
            Event = @event,
            Ticket = ticket,
            UserId = doc.UserId,
            Quantity = doc.Quantity,
            PricePerTicket = doc.PricePerTicket,
            Status = doc.Status,
            CreatedAt = doc.CreatedAt,
            ExpiresAt = doc.ExpiresAt,
            ConfirmedAt = doc.ConfirmedAt,
            CancelledAt = doc.CancelledAt,
            Version = doc.Version
        };
    }

    public static ReservationDocument ToDocument(this Reservation reservation)
    {
        return new ReservationDocument
        {
            Id = reservation.Id,
            EventId = reservation.Event.Id,
            TicketId = reservation.Ticket.Id,
            UserId = reservation.UserId,
            Quantity = reservation.Quantity,
            PricePerTicket = reservation.PricePerTicket,
            Status = reservation.Status,
            CreatedAt = reservation.CreatedAt,
            ExpiresAt = reservation.ExpiresAt,
            ConfirmedAt = reservation.ConfirmedAt,
            CancelledAt = reservation.CancelledAt,
            Version = reservation.Version
        };
    }
}