using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;

public class MqttService : BackgroundService
{
    private IMqttClient? _client;
    private MqttClientOptions? _options;
    internal static string logPrefix = "[MQTT]";

    internal void logMessage(string msg)
    {
        Console.WriteLine(logPrefix + " " + msg);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logMessage("Service starting...");

        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();

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

        _options = new MqttClientOptionsBuilder()
            .WithTcpServer("rabbitmq-mqtt", 1883)
            .WithCredentials("guest", "guest")
            .Build();

        try
        {
            await _client.ConnectAsync(_options, stoppingToken);
            logMessage("Connected.");

            await _client.SubscribeAsync("test", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce, stoppingToken);
            logMessage("Subscribed to 'test'.");
        }
        catch (Exception ex)
        {
            logMessage($"Connection failed: {ex.Message}");
        }

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }
}