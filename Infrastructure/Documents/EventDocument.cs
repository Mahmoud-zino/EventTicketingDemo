using Domain.Enums;

namespace Infrastructure.Documents;

public class EventDocument
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Venue { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public DateTime SalesStartDate { get; set; }
    public DateTime SalesEndDate { get; set; }
    public string OrganizerId { get; set; } = null!;
    public EventStatus Status { get; set; }
    public List<TicketDocument> Tickets { get; set; } = [];
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}