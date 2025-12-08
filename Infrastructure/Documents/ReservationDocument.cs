using Domain.Enums;

namespace Infrastructure.Documents;

public class ReservationDocument
{
    public string Id { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string TicketId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal PricePerTicket { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public int Version { get; set; }}