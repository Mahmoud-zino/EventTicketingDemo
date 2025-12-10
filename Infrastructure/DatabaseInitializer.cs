using Domain.Enums;
using Infrastructure.Documents;
using MongoDB.Driver;

namespace Infrastructure;

public class DatabaseInitializer(MongoDbContext context)
{
    public async Task InitializeAsync()
    {
        await MongoDbIndexConfiguration.ConfigureIndexesAsync(context);
        await SeedDataAsync();
    }

    private async Task SeedDataAsync()
    {
        var eventCount = await context.Events.CountDocumentsAsync(FilterDefinition<EventDocument>.Empty);
        if (eventCount > 0)
            return; 

        // Seed sample events
        var events = new List<EventDocument>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Rock Concert 2026",
                Description = "Amazing rock concert featuring top bands",
                Venue = "Vienna Arena",
                EventDate = DateTime.UtcNow.AddMonths(2),
                SalesStartDate = DateTime.UtcNow,
                SalesEndDate = DateTime.UtcNow.AddMonths(2).AddDays(-1),
                OrganizerId = "organizer-1",
                Status = EventStatus.Published,
                Tickets =
                [
                    new TicketDocument
                    {
                        Id = Guid.NewGuid().ToString(),
                        EventId = Guid.NewGuid().ToString(),
                        Description = "VIP seating with backstage access",
                        Price = 150.00m,
                        TotalQuantity = 50,
                        AvailableQuantity = 50,
                        ReservedQuantity = 0,
                        Version = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    new TicketDocument
                    {
                        Id = Guid.NewGuid().ToString(),
                        EventId = Guid.NewGuid().ToString(),
                        Description = "Standard seating",
                        Price = 50.00m,
                        TotalQuantity = 200,
                        AvailableQuantity = 200,
                        ReservedQuantity = 0,
                        Version = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                ],
                Version = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Events.InsertManyAsync(events);
    }
}