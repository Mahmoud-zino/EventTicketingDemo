using Domain.Enums;

namespace Domain.Entities;

public class Ticket
{
    public required string Id { get; init; }
    public required Event Event { get; init; }
    
    public required TicketType TicketType { get; set; }
    public string? Description { get; set; }
    public required decimal Price { get; set; }
    
    public required int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }

    public required DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    
    public int Version { get; set; }
    
    public bool CanReserve(int quantity) => AvailableQuantity >= quantity;

    public void Reserve(int quantity)
    {
        if (!CanReserve(quantity))
        {
            throw new InvalidOperationException("Not enough available tickets to reserve.");
        }

        AvailableQuantity -= quantity;
        ReservedQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReleaseReservation(int quantity)
    {
        AvailableQuantity += quantity;
        ReservedQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}