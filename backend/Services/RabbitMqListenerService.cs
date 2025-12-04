using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Settings;
using System.IO;
using System.Numerics;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;

namespace Services
{

    public class MqttService : BackgroundService
    {

		public static Dictionary<string, string> map = new Dictionary<string, string>();

    	public static async Task<string> TransferToken(
			string senderAddress,
			string tokenContractAddress,
			string recipientAddress,
			decimal amountTokens
		)
		{
			var web3 = new Web3("http://geth-rpc:8545");

//			var unlockResult = await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, "12", 60);
//
//			if (!unlockResult) {
//				throw new Exception("Account unlocking failed");
//			}

			var abi = "[{\"constant\":false,\"inputs\":[{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_value\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"type\":\"function\"}]";

			var contract = web3.Eth.GetContract(abi, tokenContractAddress);
			var transferFunction = contract.GetFunction("transfer");

			var amountWei = Web3.Convert.ToWei(amountTokens);

			var gas = await transferFunction.EstimateGasAsync(senderAddress, null, null, recipientAddress, amountWei);

			var txHash = await transferFunction.SendTransactionAsync(senderAddress, gas, null, recipientAddress, amountWei);

			return txHash;
		}

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

        private async Task HandleIncomingMessage(string message)
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

                map[sensorMsg.SensorId] = sensorMsg.Account;

				string sender = "0x59865b2aca42ae1c671d05a8cf5ebc42ca23f891";
				string tokenAddress = File.ReadAllLines("./contract.txt")[0];
				string recipient = sensorMsg.Account;
				decimal amount = 1m;

				string txHash = await TransferToken(sender, tokenAddress, recipient, amount);
				Console.WriteLine("Transaction sent. Hash: " + txHash);

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
