using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;

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
    
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public int Version { get; set; }
    
    public bool CanReserve(int quantity) => AvailableQuantity >= quantity;

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
        {
            throw new InvalidTicketQuantityException(quantity);
        }
        
        if (!CanReserve(quantity))
        {
            throw new InsufficientTicketsException(quantity, AvailableQuantity);
        }

        AvailableQuantity -= quantity;
        ReservedQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
        
        _domainEvents.Add(new TicketsReservedEvent
        {
            Ticket = this,
            Quantity = quantity
        });
        
        if (AvailableQuantity == 0)
        {
            _domainEvents.Add(new TicketSoldOutEvent
            {
                Ticket = this
            });
        }
    }

    public void ReleaseReservation(int quantity, string reason)
    {
        if (quantity <= 0)
        {
            throw new InvalidTicketQuantityException(quantity);
        }
        
        AvailableQuantity += quantity;
        ReservedQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
        
        _domainEvents.Add(new TicketsReleasedEvent
        {
            Ticket = this,
            Quantity = quantity,
            Reason = reason
        });
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}