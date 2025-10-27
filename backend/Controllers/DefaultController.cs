using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("api/demo")]
    public class DefaultController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new[] { "Hello World" });
    }
}

