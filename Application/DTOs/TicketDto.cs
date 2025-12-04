namespace Application.DTOs;

public record TicketDto
{
    public required string Id { get; init; }
    public required string EventId { get; init; }
    public string? Description { get; init; }
    public required decimal Price { get; init; }
    
    public required int TotalQuantity { get; init; }
    public required int AvailableQuantity { get; init; }
    public required int ReservedQuantity { get; init; }
    
    public bool IsSoldOut { get; init; }
    public int SoldQuantity => TotalQuantity - AvailableQuantity - ReservedQuantity;
}