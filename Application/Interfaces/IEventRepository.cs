using Domain.Entities;

namespace Application.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(string eventId, CancellationToken cancellationToken = default);
    Task<List<Event>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<string> AddAsync(Event @event, CancellationToken cancellationToken = default);
    Task UpdateAsync(Event @event, CancellationToken cancellationToken = default);
}