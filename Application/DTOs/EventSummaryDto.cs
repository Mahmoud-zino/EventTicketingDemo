using Domain.Enums;

namespace Application.DTOs;

public record EventSummaryDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Venue { get; init; }
    public required DateTime EventDate { get; init; }
    public required EventStatus Status { get; init; }
    
    public int TotalTicketsAvailable { get; init; }
    public decimal MinTicketPrice { get; init; }
    public decimal MaxTicketPrice { get; init; }
    
    public bool IsSoldOut { get; init; }
    public bool IsAvailableForPurchase { get; init; }
}