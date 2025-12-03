using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;

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
    
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public int Version { get; set; }
    
    public int GetTotalAvailableTickets() => Tickets.Sum(t => t.AvailableQuantity);
    public bool IsSoldOut() => Tickets.All(t => t.AvailableQuantity == 0);

    public void Publish()
    {
        if (Status == EventStatus.Cancelled)
        {
            throw new EventAlreadyCancelledException(Id);
        }
        
        Status = EventStatus.Published;
        UpdatedAt = DateTime.UtcNow;
        
        _domainEvents.Add(new EventPublishedEvent
        {
            Event = this
        });
    }
    
    public void Cancel(string reason)
    {
        if (Status == EventStatus.Cancelled)
        {
            throw new EventAlreadyCancelledException(Id);
        }
        
        Status = EventStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        
        _domainEvents.Add(new EventCancelledEvent
        {
            Event = this,
            Reason = reason
        });
    }
    
    public void ValidateCanPurchaseTickets()
    {
        if (Status != EventStatus.Published)
        {
            throw new EventNotPublishedException(Id, Status);
        }
        
        var now = DateTime.UtcNow;
        if (now < SalesStartDate)
        {
            throw new EventSalesNotStartedException(Id, SalesStartDate);
        }
        if (now > SalesEndDate)
        {
            throw new EventSalesEndedException(Id, SalesEndDate);
        }
    }
}