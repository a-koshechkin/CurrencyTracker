using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.DTOs;
using System.Reflection;

namespace UserService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController(IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    [HttpGet]
    public IActionResult GetHealth()
    {
        var healthStatus = new HealthResponse
        {
            Service = new ServiceInfo
            {
                Name = "User Service",
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0"
            }
        };

        return Ok(healthStatus);
    }
} 