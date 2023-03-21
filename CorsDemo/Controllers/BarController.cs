using Microsoft.AspNetCore.Mvc;

namespace CorsDemo.Controllers;

[ApiController]
[Route("Bar")]
public class BarController : ControllerBase
{
    private readonly ILogger<BarController> _logger;
    public BarController(ILogger<BarController> logger)
    {
        _logger = logger;
    }

    [HttpPut(Name = "Money")]
    public IActionResult PutMoneyOnBar()
    {
        _logger.LogInformation("X-Money");
        var found = Request.Headers.TryGetValue("X-Money", out var headerValue);
        if (found)
        {
            return Ok(headerValue);
        }

        return NotFound();
    }
}