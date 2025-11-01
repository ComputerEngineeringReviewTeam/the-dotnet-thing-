using MongoDB.Driver;
using Models;
using Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;


namespace Services
{
    public class MongoDbService
    {
        private MongoDbSettings _mongoDbSettings;
        private readonly IMongoCollection<WaterMeasurement> _collection;

        internal static string logPrefix = "[MongoDb]";

        internal void logMessage(string msg)
        {
            Console.WriteLine($"{logPrefix} {msg}");
        }

        public MongoDbService(IOptions<MongoDbSettings> options)
        {
            _mongoDbSettings = options.Value;
            var client = new MongoClient(_mongoDbSettings.ConnectionString);
            var database = client.GetDatabase(_mongoDbSettings.DatabaseName);
            _collection = database.GetCollection<WaterMeasurement>(_mongoDbSettings.CollectionName);

            try
            {
                database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                logMessage("MongoDB connection successful!");
            }
            catch (Exception ex)
            {
                logMessage("MongoDB connection failed: " + ex.Message);
            }

            logMessage("Connection: " + _mongoDbSettings.ConnectionString);
            logMessage("Db name: " + _mongoDbSettings.DatabaseName);
            logMessage("Collection: " + _mongoDbSettings.CollectionName);
        }

        public async Task<List<WaterMeasurement>> GetAllAsync()
            => await _collection.Find(_ => true).ToListAsync();

        public async Task<WaterMeasurement?> GetByIdAsync(string id)
            => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task AddAsync(WaterMeasurement measurement)
            => await _collection.InsertOneAsync(measurement);

        public async Task UpdateAsync(string id, WaterMeasurement updated)
            => await _collection.ReplaceOneAsync(x => x.Id == id, updated);

        public async Task DeleteAsync(string id)
            => await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
