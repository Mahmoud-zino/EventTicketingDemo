namespace Domain.Events;

public abstract record DomainEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}