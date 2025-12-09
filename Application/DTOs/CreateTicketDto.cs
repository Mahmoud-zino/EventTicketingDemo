namespace Application.DTOs;

public record CreateTicketDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required decimal Price { get; init; }
    public required int TotalQuantity { get; init; }
}