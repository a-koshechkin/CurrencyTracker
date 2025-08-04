using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.DTOs;
using System.Reflection;

namespace Shared.Infrastructure.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class HealthControllerBase : ControllerBase
{
    protected abstract string ServiceName { get; }

    [HttpGet]
    public virtual IActionResult GetHealth()
    {
        var healthStatus = new HealthResponse
        {
            Service = new ServiceInfo
            {
                Name = ServiceName,
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0"
            }
        };

        return Ok(healthStatus);
    }
} 