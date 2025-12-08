using System.Data;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Mappers;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class ReservationRepository(MongoDbContext context, IEventRepository eventRepository): IReservationRepository
{
    public async Task<Reservation?> GetByIdAsync(string reservationId, CancellationToken cancellationToken = default)
    {
        var doc = await context.Reservations
            .Find(r => r.Id == reservationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (doc is null) return null;
        return await doc.ToDomainAsync(eventRepository, cancellationToken);
    }

    public async Task<List<Reservation>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var docs = await context.Reservations
            .Find(r => r.UserId == userId)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
        
        var reservations = new List<Reservation>();
        foreach (var doc in docs)
        {
            reservations.Add(await doc.ToDomainAsync(eventRepository, cancellationToken));
        }
        return reservations;
    }

    public async Task<string> AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        reservation.Version = 0;
        reservation.CreatedAt = DateTime.UtcNow;
        
        var doc = reservation.ToDocument();
        await context.Reservations.InsertOneAsync(doc, new InsertOneOptions(), cancellationToken);
        return doc.Id;
    }

    public async Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        var currentVersion = reservation.Version;
        reservation.Version++;
        
        var doc = reservation.ToDocument();
        
        var result = await context.Reservations.ReplaceOneAsync(
            r => r.Id == doc.Id &&  r.Version == currentVersion, doc, new ReplaceOptions(),  cancellationToken);

        if (result.MatchedCount == 0)
        {
            throw new DBConcurrencyException($"Event {reservation.Id} was modified by another process!");
        }
    }
}