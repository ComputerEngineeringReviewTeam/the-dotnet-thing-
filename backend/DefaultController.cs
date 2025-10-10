using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class DefaultController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new[] { "Hello World" });
}