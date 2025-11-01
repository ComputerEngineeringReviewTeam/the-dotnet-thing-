using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;


namespace Services
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = null!;
        public string QueueName { get; set; } = null!;
    }

    public class MqttService : BackgroundService
    {
        private readonly RabbitMqSettings _rabbitSettings;
        private IMqttClient? _client;
        private MqttClientOptions? _options;
        internal static string logPrefix = "[MQTT]";

        internal void logMessage(string msg)
        {
            Console.WriteLine($"{logPrefix} {msg}");
        }

        public MqttService(IOptions<RabbitMqSettings> options)
        {
            _rabbitSettings = options.Value;
            logMessage("HostName: " + _rabbitSettings.HostName);
            logMessage("QueueName: " + _rabbitSettings.QueueName);
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
    }
}