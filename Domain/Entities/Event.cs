using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

public record Event
{
    public required string Id { get; init; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Venue { get; set; }
    public required DateTime EventDate { get; set; }
    public required DateTime SalesStartDate { get; set; }
    public required DateTime SalesEndDate { get; set; }
    
    public required string OrganizerId { get; set; }
    public EventStatus Status { get; set; }
    
    public required DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    
    public List<Ticket> Tickets { get; set; } = [];
    
    public int Version { get; set; }
    
    public int GetTotalAvailableTickets() => Tickets.Sum(t => t.AvailableQuantity);
    public bool IsSoldOut() => Tickets.All(t => t.AvailableQuantity == 0);
}