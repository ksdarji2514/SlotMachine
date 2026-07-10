using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace SlotMachine.Database.Mongo;

public interface IMongoContext
{
    IMongoCollection<T> GetCollection<T>(string name);
}

/// <summary>
/// Owns the Mongo connection. Repositories depend on this abstraction rather than
/// creating their own client. Registered as a singleton (the driver is thread-safe
/// and pools connections internally).
/// </summary>
public class MongoContext : IMongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IOptions<MongoSettings> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.Database);
    }

    public IMongoCollection<T> GetCollection<T>(string name) =>
        _database.GetCollection<T>(name);
}
