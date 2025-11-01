using MongoDB.Driver;
using Models;


namespace Services
{
    public class WaterMeasurementService
    {
        private readonly IMongoCollection<WaterMeasurement> _collection;

        public WaterMeasurementService(string connectionString, string dbName, string collectionName = "WaterMeasurements")
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            _collection = database.GetCollection<WaterMeasurement>(collectionName);
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
