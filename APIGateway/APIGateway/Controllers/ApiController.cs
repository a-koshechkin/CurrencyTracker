using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers;

[ApiController]
[Route("api/v1")]
public class ApiController : ControllerBase
{
    [HttpGet]
    public IActionResult GetApiInfo()
    {
        return Ok(new
        {
            Name = "Currency Tracker API",
            Version = "v1.0",
            Description = "RESTful API for currency tracking and user management",
            BaseUrl = $"{Request.Scheme}://{Request.Host}/api/v1",
            Resources = new
            {
                Authentication = new
                {
                    BasePath = "/auth",
                    Description = "User authentication and authorization",
                    Endpoints = new[]
                    {
                        new { Method = "POST", Path = "/auth/register", Description = "Register a new user" },
                        new { Method = "POST", Path = "/auth/login", Description = "Authenticate user and get token" },
                        new { Method = "POST", Path = "/auth/logout", Description = "Logout user" }
                    }
                },
                Currencies = new
                {
                    BasePath = "/currencies",
                    Description = "Currency information and data",
                    Endpoints = new[]
                    {
                        new { Method = "GET", Path = "/currencies", Description = "Get all available currencies", Auth = "Required" }
                    }
                },
                Favorites = new
                {
                    BasePath = "/favorites",
                    Description = "User favorite currencies management",
                    Endpoints = new[]
                    {
                        new { Method = "GET", Path = "/favorites", Description = "Get user's favorite currencies", Auth = "Required" },
                        new { Method = "POST", Path = "/favorites", Description = "Add currency to favorites", Auth = "Required" },
                        new { Method = "DELETE", Path = "/favorites/{currencyCode}", Description = "Remove currency from favorites", Auth = "Required" }
                    }
                }
            },
            Authentication = new
            {
                Type = "Bearer Token",
                Header = "Authorization: Bearer {token}",
                Description = "Include the JWT token in the Authorization header for protected endpoints"
            },
            StatusCodes = new
            {
                Success = new[] { 200, 201, 204 },
                ClientError = new[] { 400, 401, 403, 404, 422 },
                ServerError = new[] { 500, 502, 503 }
            }
        });
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "v1.0",
            Services = new[]
            {
                "UserService",
                "FinanceService"
            }
        });
    }
} 