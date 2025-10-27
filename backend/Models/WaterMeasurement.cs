using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Models
{
    public class WaterMeasurement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("SensorId")]
        public string SensorId { get; set; } = null!;

        [BsonElement("SensorType")]
        [BsonRepresentation(BsonType.String)]
        public SensorType SensorType { get; set; }

        [BsonElement("Value")]
        public double Value { get; set; }

        [BsonElement("Timestamp")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Timestamp { get; set; }
    }

    public class SensorMessage
    {
        [JsonPropertyName("sensorId")]
        public string SensorId { get; set; } = null!;

        [JsonPropertyName("sensorType")]
        public string SensorType { get; set; } = null!;

        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}