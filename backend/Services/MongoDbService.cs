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
                // block synchronously
                database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").GetAwaiter().GetResult();
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

        public List<WaterMeasurement> GetAll()
            => _collection.Find(_ => true).ToList();

        public WaterMeasurement? GetById(string id)
            => _collection.Find(x => x.Id == id).FirstOrDefault();

        public void Add(WaterMeasurement measurement)
            => _collection.InsertOne(measurement);

        public void Update(string id, WaterMeasurement updated)
            => _collection.ReplaceOne(x => x.Id == id, updated);

        public void Delete(string id)
            => _collection.DeleteOne(x => x.Id == id);

        public void DeleteAll()
            => _collection.DeleteMany(Builders<WaterMeasurement>.Filter.Empty);
            
        public List<WaterMeasurement> Search(MeasurementQuery q)
        {
            var builder = Builders<WaterMeasurement>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(q.SensorId))
                filter &= builder.Eq(x => x.SensorId, q.SensorId);

            if (q.SensorType != null)
                filter &= builder.Eq(x => x.SensorType, q.SensorType);

            if (q.MinValue != null)
                filter &= builder.Gte(x => x.Value, q.MinValue);

            if (q.MaxValue != null)
                filter &= builder.Lte(x => x.Value, q.MaxValue);

            if (q.From != null)
                filter &= builder.Gte(x => x.Timestamp, q.From);

            if (q.To != null)
                filter &= builder.Lte(x => x.Timestamp, q.To);

            var sort = q.SortBy switch
            {
                "value" => q.Desc ? Builders<WaterMeasurement>.Sort.Descending(x => x.Value)
                                : Builders<WaterMeasurement>.Sort.Ascending(x => x.Value),
                _ => q.Desc ? Builders<WaterMeasurement>.Sort.Descending(x => x.Timestamp)
                            : Builders<WaterMeasurement>.Sort.Ascending(x => x.Timestamp)
            };

            return _collection.Find(filter).Sort(sort).ToList();
        }
    }
}
