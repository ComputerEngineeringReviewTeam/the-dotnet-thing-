using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("api")]
    public class MeasurementController : ControllerBase
    {
        [HttpGet("measurement")]
        public IActionResult Get() => Ok(new[] { "Measurement" });

        [HttpGet("measurement/json")]
        public IActionResult GetJson() => Ok(new[] { "Measurement Json" });

        [HttpGet("measurement/csv")]
        public IActionResult GetCsv() => Ok(new[] { "Measurement Csv" });
    }
}

