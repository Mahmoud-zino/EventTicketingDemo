using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;

namespace Domain.Entities;

public record Reservation
{
    public required string Id { get; init; }
    public required Event Event { get; init; }
    public required Ticket Ticket { get; init; }
    public required string UserId { get; init; }
    
    public int Quantity { get; set; }
    public required decimal PricePerTicket { get; set; }
    public decimal TotalPrice => PricePerTicket * Quantity;
    
    public ReservationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public int Version { get; set; }

    public bool IsExpired() => Status == ReservationStatus.Pending && DateTime.UtcNow > ExpiresAt;

    public void Confirm()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new InvalidReservationStatusException(Id, Status, ReservationStatus.Pending);
        }
        
        if (IsExpired())
        {
            throw new ReservationExpiredException(Id, ExpiresAt);
        }
        
        Status = ReservationStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        
        _domainEvents.Add(new ReservationConfirmedEvent
        {
            Reservation = this
        });
    }

    public void Cancel()
    {
        switch (Status)
        {
            case ReservationStatus.Confirmed:
                throw new CannotCancelConfirmedReservationException(Id);
            case ReservationStatus.Cancelled:
                throw new InvalidReservationStatusException(Id, Status, ReservationStatus.Pending);
            case ReservationStatus.Pending:
            case ReservationStatus.Expired:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Status = ReservationStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        
        Ticket.ReleaseReservation(Quantity, "Cancelled");
        
        _domainEvents.Add(new ReservationCancelledEvent
        {
            Reservation = this
        });
    }

    public void MarkAsExpired()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new InvalidReservationStatusException(Id, Status, ReservationStatus.Pending);
        }
        
        Status = ReservationStatus.Expired;
        Ticket.ReleaseReservation(Quantity, "Expired");
        _domainEvents.Add(new ReservationExpiredEvent
        {
            Reservation = this
        });
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}