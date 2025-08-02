using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace FinanceService.API.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class HealthController(IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    [HttpGet]
    public IActionResult GetHealth()
    {
        var healthStatus = new
        {
            Service = new
            {
                Name = "Finance Service",
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0"
            }
        };

        return Ok(healthStatus);
    }
} 