using Microsoft.AspNetCore.Mvc;
using Services;
using Microsoft.AspNetCore.Cors;

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
