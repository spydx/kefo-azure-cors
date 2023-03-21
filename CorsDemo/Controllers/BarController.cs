using Microsoft.AspNetCore.Mvc;

namespace CorsDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class BarController : ControllerBase
{
    private readonly ILogger<BarController> _logger;
    public BarController(ILogger<BarController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "Money")]
    public IActionResult PostMoneyOnBar()
    {
        _logger.LogInformation("X-Money");


        var found = Request.Headers.TryGetValue("X-Money", out var headerValue);
        if (!found) return NotFound();
        
        HttpContext.Response.Headers.Add("X-Beer", "IPA");
        return Ok(headerValue);

    }
}