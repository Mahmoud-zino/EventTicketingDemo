using Infrastructure.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure;

public class MongoDbContext
{
   private readonly IMongoDatabase _database;

   public MongoDbContext(IOptions<MongoDbSettings> settings)
   {
      var client = new MongoClient(settings.Value.ConnectionString);
      _database = client.GetDatabase(settings.Value.DatabaseName);
   }
   
   public IMongoCollection<EventDocument> Events => _database.GetCollection<EventDocument>("events");
   public IMongoCollection<ReservationDocument> Reservations => _database.GetCollection<ReservationDocument>("reservations");
}