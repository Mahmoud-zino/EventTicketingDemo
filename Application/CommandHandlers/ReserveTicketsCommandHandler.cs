using Application.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Mediator;

namespace Application.CommandHandlers;

public sealed class ReserveTicketsCommandHandler(IEventRepository eventRepository, IReservationRepository reservationRepository)
    : IRequestHandler<ReserveTicketCommand, string>
{
    private const int ReservationExpiryMinutes = 15;
    
    public async ValueTask<string> Handle(ReserveTicketCommand request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (@event is null)
        {
            throw new EventNotFoundException(request.EventId);
        }
        
        var ticket = @event.Tickets.FirstOrDefault(t => t.Id == request.TicketId);
        if (ticket is null)
        {
            throw new TicketNotFoundException(request.TicketId);
        }
        
        @event.ValidateCanPurchaseTickets();
        ticket.Reserve(request.Quantity);

        var reservation = new Reservation
        {
            Id = Guid.NewGuid().ToString(),
            Event = @event,
            Ticket = ticket,
            UserId = request.UserId,
            Quantity = request.Quantity,
            PricePerTicket = ticket.Price,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(ReservationExpiryMinutes)
        };
        
        await eventRepository.UpdateAsync(@event, cancellationToken);
        await reservationRepository.AddAsync(reservation, cancellationToken);
        
        return reservation.Id;
    }
}