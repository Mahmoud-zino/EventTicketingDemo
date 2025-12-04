using Domain.Enums;

namespace Application.DTOs;

public record ReservationDto
{
    public required string Id { get; init; }
    
    public required string EventId { get; init; }
    public required string EventName { get; init; }
    public required string EventVenue { get; init; }
    public required DateTime EventDate { get; init; }
    
    public required string TicketId { get; init; }
    public required int Quantity { get; init; }
    public required decimal PricePerTicket { get; init; }
    public required decimal TotalPrice { get; init; }
    
    public required ReservationStatus Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    
    public DateTime? ExpiresAt { get; init; }
    public DateTime? ConfirmedAt { get; init; } 
    public DateTime? CancelledAt { get; init; }
    
    public bool IsExpired { get; init; }
    public int? MinutesUntilExpiry { get; init; }
    public bool CanBeCancelled { get; init; }
    public bool CanBeConfirmed { get; init; }
}