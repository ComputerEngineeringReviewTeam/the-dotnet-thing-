using Microsoft.AspNetCore.Mvc;
using Services;

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

        [HttpGet("measurement")]
        public IActionResult Get()
        {
            var list = _mongo.GetAll();
            return Ok(list);
        }

        [HttpGet("measurement/{id}")]
        public IActionResult GetOne(string id)
        {
            var item = _mongo.GetById(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("measurement")]
        public IActionResult Add([FromBody] Models.WaterMeasurement m)
        {
            _mongo.Add(m);
            return Ok(m);
        }

        [HttpPut("measurement/{id}")]
        public IActionResult Update(string id, [FromBody] Models.WaterMeasurement m)
        {
            _mongo.Update(id, m);
            return Ok(m);
        }

        [HttpDelete("measurement/{id}")]
        public IActionResult Delete(string id)
        {
            _mongo.Delete(id);
            return NoContent();
        }

        [HttpDelete("measurement")]
        public IActionResult DeleteAll()
        {
            _mongo.DeleteAll();
            return NoContent();
        }

        [HttpGet("measurement/json")]
        public IActionResult GetJson() => Ok(new[] { "Measurement Json" });

        [HttpGet("measurement/csv")]
        public IActionResult GetCsv() => Ok(new[] { "Measurement Csv" });
    }
}
