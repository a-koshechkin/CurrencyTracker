using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers;

[ApiController]
[Route("api/v1")]
public class ApiController : ControllerBase
{
    private static readonly string[] Resources =
    [
        "/auth/* - User authentication and authorization",
        "/currencies/* - Currency information and data", 
        "/favorites/* - User favorite currencies management"
    ];

    [HttpGet]
    public IActionResult GetApiInfo()
    {
        return Ok(new
        {
            Name = "Currency Tracker API",
            Version = "1.0",
            Description = "RESTful API for currency tracking and user management",
            BaseUrl = $"{Request.Scheme}://{Request.Host}/api/v1",
            Documentation = $"{Request.Scheme}://{Request.Host}/api/v1/docs",
            Health = $"{Request.Scheme}://{Request.Host}/api/health",
            Resources,
            Authentication = "Bearer Token required for protected endpoints"
        });
    }
} 