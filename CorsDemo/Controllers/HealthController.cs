using Microsoft.AspNetCore.Mvc;

namespace CorsDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var result = Environment.GetEnvironmentVariable("LOCALCORS");
        _logger.LogInformation($"Result is : {result}");
        
        return Ok(result ?? "I'm Alive!!!!!");
    }
}