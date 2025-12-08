using Infrastructure.Documents;
using MongoDB.Driver;

namespace Infrastructure;

public static class MongoDbIndexConfiguration
{
    public static async Task ConfigureIndexesAsync(MongoDbContext context)
    {
        await ConfigureEventIndexes(context);
        await ConfigureReservationIndexes(context);
    }

    private static async Task ConfigureEventIndexes(MongoDbContext context)
    {
        var eventCollection = context.Events;

        var statusIndex = Builders<EventDocument>.IndexKeys.Ascending(e => e.Status);
        await eventCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<EventDocument>(statusIndex));

        var eventDateIndex = Builders<EventDocument>.IndexKeys.Ascending(e => e.EventDate);
        await eventCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<EventDocument>(eventDateIndex));

        var statusDateIndex = Builders<EventDocument>.IndexKeys
            .Ascending(e => e.Status)
            .Ascending(e => e.EventDate);
        await eventCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<EventDocument>(statusDateIndex));

        var organizerIndex = Builders<EventDocument>.IndexKeys.Ascending(e => e.OrganizerId);
        await eventCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<EventDocument>(organizerIndex));
    }

    private static async Task ConfigureReservationIndexes(MongoDbContext context)
    {
        var reservationCollection = context.Reservations;

        var userIdIndex = Builders<ReservationDocument>.IndexKeys.Ascending(r => r.UserId);
        await reservationCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<ReservationDocument>(userIdIndex));

        var eventIdIndex = Builders<ReservationDocument>.IndexKeys.Ascending(r => r.EventId);
        await reservationCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<ReservationDocument>(eventIdIndex));

        var expirationIndex = Builders<ReservationDocument>.IndexKeys
            .Ascending(r => r.Status)
            .Ascending(r => r.ExpiresAt);
        await reservationCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<ReservationDocument>(expirationIndex));

        var createdAtIndex = Builders<ReservationDocument>.IndexKeys.Descending(r => r.CreatedAt);
        await reservationCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<ReservationDocument>(createdAtIndex));
    }
}