using Domain.Enums;

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
    
    public int Version { get; set; }

    public bool IsExpired() => Status == ReservationStatus.Pending && DateTime.UtcNow > ExpiresAt;

    public void Confirm()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new InvalidOperationException("Only pending reservations can be confirmed.");
        }
        
        if (IsExpired())
        {
            throw new InvalidOperationException("Reservation has expired and cannot be confirmed.");
        }
        
        Status = ReservationStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == ReservationStatus.Confirmed)
        {
            throw new InvalidOperationException("Confirmed reservations cannot be cancelled.");
        }
        
        Status = ReservationStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        
        Ticket.ReleaseReservation(Quantity);
    }
}