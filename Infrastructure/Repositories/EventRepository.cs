using System.Data;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Mappers;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class EventRepository(MongoDbContext context): IEventRepository
{
    public async Task<Event?> GetByIdAsync(string eventId, CancellationToken cancellationToken = default)
    {
        var result = await context.Events
            .Find(e => e.Id == eventId)
            .FirstOrDefaultAsync(cancellationToken);

        return result?.ToDomain();
    }

    public async Task<List<Event>> GetAllAsync(CancellationToken cancellationToken = default)
    {
       var result = await context.Events
           .Find(_ => true)
           .ToListAsync(cancellationToken);
       
       return result.Select(e => e.ToDomain()).ToList();
    }

    public async Task<string> AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        @event.Version = 0;
        @event.UpdatedAt = DateTime.UtcNow;
        
        var doc = @event.ToDocument();
        await context.Events.InsertOneAsync(doc, new InsertOneOptions(), cancellationToken);
        return doc.Id;
    }

    public async Task UpdateAsync(Event @event, CancellationToken cancellationToken = default)
    {
        var currentVersion = @event.Version;

        @event.Version++;
        @event.UpdatedAt = DateTime.UtcNow;
        
        var doc = @event.ToDocument();

        var result = await context.Events.ReplaceOneAsync(
            e => e.Id == doc.Id && e.Version == currentVersion, doc, new ReplaceOptions(), cancellationToken);

        if (result.MatchedCount == 0)
        {
            throw new DBConcurrencyException($"Event {doc.Id} was modified by another process!");
        }
    }
}