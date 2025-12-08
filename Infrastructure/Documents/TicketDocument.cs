namespace Infrastructure.Documents;

public class TicketDocument
{
    public string Id { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }}