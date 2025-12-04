using Microsoft.AspNetCore.Mvc;
using Services;
using Microsoft.AspNetCore.Cors;

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

namespace Controllers
{
    [ApiController]
    [Route("api")]
    public class MeasurementController : ControllerBase
    {
        private readonly MongoDbService _mongo;

        public MeasurementController(MongoDbService mongo)
        {
            _mongo = mongo;
        }

        // [HttpGet("measurement")]
        // public IActionResult Get()
        // {
        //     var list = _mongo.GetAll();
        //     return Ok(list);
        // }

		// ! crypto ! //
        public static async Task<decimal> GetTokenBalance(
			string accountAddress,
			string tokenContractAddress
		)
		{
			var web3 = new Web3("http://geth-rpc:8545");

			var abi = "[{\"constant\":true,\"inputs\":[{\"name\":\"_owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"name\":\"balance\",\"type\":\"uint256\"}],\"type\":\"function\"}]";

			var contract = web3.Eth.GetContract(abi, tokenContractAddress);
			var balanceFunction = contract.GetFunction("balanceOf");

			var balanceWei = await balanceFunction.CallAsync<BigInteger>(accountAddress);
			var balanceTokens = Web3.Convert.FromWei(balanceWei);

			return balanceTokens;
		}

        [EnableCors("permissive")]
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance(string id)
        {

        	if (!MqttService.map.ContainsKey(id)) {
        		return Ok();
        	}

        	var acc = MqttService.map[id];

            var data = await GetTokenBalance(acc, System.IO.File.ReadAllLines("./contract.txt")[0]);
            return Ok(data);
        }

        [EnableCors("permissive")]
        [HttpGet("measurement/json")]
        public IActionResult GetJson([FromQuery] MeasurementQuery q)
        {
            var data = _mongo.Search(q);
            return Ok(data); // automatic JSON
        }

        [EnableCors("permissive")]
        [HttpGet("measurement/csv")]
        public IActionResult GetCsv([FromQuery] MeasurementQuery q)
        {
            var data = _mongo.Search(q);

            var lines = new List<string>
            {
                "Id,SensorId,SensorType,Value,Timestamp"
            };

            foreach (var m in data)
            {
                lines.Add($"{m.Id},{m.SensorId},{m.SensorType},{m.Value},{m.Timestamp:o}");
            }

            var csv = string.Join("\n", lines);

            return File(
                System.Text.Encoding.UTF8.GetBytes(csv),
                "text/csv",
                "measurements.csv"
            );
        }

        [EnableCors("permissive")]
        [HttpGet("measurement/{id}")]
        public IActionResult GetOne(string id)
        {
            var item = _mongo.GetById(id);
            return item == null ? NotFound() : Ok(item);
        }

        [EnableCors("permissive")]
        [HttpPost("measurement")]
        public IActionResult Add([FromBody] Models.WaterMeasurement m)
        {
            _mongo.Add(m);
            return Ok(m);
        }

        [EnableCors("permissive")]
        [HttpPut("measurement/{id}")]
        public IActionResult Update(string id, [FromBody] Models.WaterMeasurement m)
        {
            _mongo.Update(id, m);
            return Ok(m);
        }

        [EnableCors("permissive")]
        [HttpDelete("measurement/{id}")]
        public IActionResult Delete(string id)
        {
            _mongo.Delete(id);
            return NoContent();
        }

        [EnableCors("permissive")]
        [HttpDelete("measurement")]
        public IActionResult DeleteAll()
        {
            _mongo.DeleteAll();
            return NoContent();
        }

        [EnableCors("permissive")]
        [HttpGet("measurement")]
        public IActionResult Search([FromQuery] MeasurementQuery q)
        {
            var result = _mongo.Search(q);
            return Ok(result);
        }
    }
}
