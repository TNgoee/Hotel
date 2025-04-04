using KhachSan.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace KhachSan.Services;

public class MongoDBService
{
    private readonly IMongoDatabase _database;

    public MongoDBService(IOptions<MongoDBSetting> mongoDBSettings)
    {
        Console.WriteLine($"MongoDB Connection String: {mongoDBSettings.Value.ConnectionURI}");

        var settings = MongoClientSettings.FromConnectionString(mongoDBSettings.Value.ConnectionURI);
        var client = new MongoClient(settings);
        _database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}
