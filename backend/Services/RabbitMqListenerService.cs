using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Settings;


namespace Services
{

    public class MqttService : BackgroundService
    {
        private readonly RabbitMqSettings _rabbitSettings;
        private IMqttClient? _client;
        private MqttClientOptions? _options;

        private MongoDbService mognoDbService;
        internal static string logPrefix = "[MQTT]";

        internal void logMessage(string msg)
        {
            Console.WriteLine($"{logPrefix} {msg}");
        }

        public MqttService(IOptions<RabbitMqSettings> options, MongoDbService service)
        {
            _rabbitSettings = options.Value;
            logMessage("HostName: " + _rabbitSettings.HostName);
            logMessage("QueueName: " + _rabbitSettings.QueueName);
            mognoDbService = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logMessage("Service starting...");

            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            _options = new MqttClientOptionsBuilder()
                .WithTcpServer(_rabbitSettings.HostName, 1883)
                .WithCredentials("guest", "guest")
                .Build();

            _client.ApplicationMessageReceivedAsync += e =>
            {
                var msg = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                logMessage($"Received: {msg}");
                HandleIncomingMessage(msg);
                return Task.CompletedTask;
            };

            _client.DisconnectedAsync += async e =>
            {
                logMessage("Disconnected. Trying to reconnect...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                try
                {
                    await _client!.ConnectAsync(_options!, stoppingToken);
                    logMessage("Reconnected.");
                }
                catch (Exception ex)
                {
                    logMessage($"Reconnect failed: {ex.Message}");
                }
            };

            try
            {
                await _client.ConnectAsync(_options, stoppingToken);
                logMessage("Connected.");

                await _client.SubscribeAsync(_rabbitSettings.QueueName, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce, stoppingToken);
                logMessage($"Subscribed to '{_rabbitSettings.QueueName}'.");
            }
            catch (Exception ex)
            {
                logMessage($"Connection failed: {ex.Message}");
            }

            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(1000, stoppingToken);
        }

        private void HandleIncomingMessage(string message)
        {
            try
            {
                // Deserialize JSON message to SensorMessage
                var sensorMsg = System.Text.Json.JsonSerializer.Deserialize<Models.SensorMessage>(message);
                if (sensorMsg == null) return;

                // Convert to WaterMeasurement
                var measurement = new Models.WaterMeasurement
                {
                    SensorId = sensorMsg.SensorId,
                    SensorType = Enum.Parse<Models.SensorType>(sensorMsg.SensorType),
                    Value = sensorMsg.Value,
                    Timestamp = sensorMsg.Timestamp
                };

                // Save to MongoDB
                mognoDbService.Add(measurement);
                logMessage($"Saved measurement from {measurement.SensorId}");
            }
            catch (Exception ex)
            {
                logMessage($"Failed to process message: {ex.Message}");
            }
        }
    }
}