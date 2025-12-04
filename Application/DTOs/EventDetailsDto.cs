using Domain.Enums;

namespace Application.DTOs;

public record EventDetailsDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Venue { get; init; }
    public required DateTime EventDate { get; init; }
    
    public required DateTime SalesStartDate { get; init; }
    public required DateTime SalesEndDate { get; init; }
    
    public required List<TicketDto> Tickets { get; init; }
    public int TotalTicketsAvailable { get; init; }
    public int TotalTicketsReserved { get; init; }
    
    public required EventStatus Status { get; init; }
    public required string OrganizerId { get; init; }
    
    public bool IsSoldOut { get; init; }
    public bool IsAvailableForPurchase { get; init; }
    public bool HasSalesStarted { get; init; }
    public bool HasSalesEnded { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}