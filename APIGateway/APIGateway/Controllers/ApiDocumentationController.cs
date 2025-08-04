using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers;

[ApiController]
[Route("api/v1/docs")]
public class ApiDocumentationController : ControllerBase
{
    private static readonly object AuthenticationResource = new
    {
        BasePath = "/auth",
        Description = "User authentication and authorization",
        Endpoints = new[]
        {
            new { Method = "POST", Path = "/auth/register", Description = "Register a new user", Auth = "None" },
            new { Method = "POST", Path = "/auth/login", Description = "Authenticate user and get token", Auth = "None" },
            new { Method = "POST", Path = "/auth/logout", Description = "Logout user", Auth = "Bearer Token" }
        }
    };

    private static readonly object CurrenciesResource = new
    {
        BasePath = "/currencies",
        Description = "Currency information and data",
        Endpoints = new[]
        {
            new { Method = "GET", Path = "/currencies", Description = "Get all available currencies", Auth = "Bearer Token" }
        }
    };

    private static readonly object FavoritesResource = new
    {
        BasePath = "/favorites",
        Description = "User favorite currencies management",
        Endpoints = new[]
        {
            new { Method = "GET", Path = "/favorites", Description = "Get user's favorite currencies", Auth = "Bearer Token" },
            new { Method = "POST", Path = "/favorites", Description = "Add currency to favorites", Auth = "Bearer Token" },
            new { Method = "DELETE", Path = "/favorites/{currencyCode}", Description = "Remove currency from favorites", Auth = "Bearer Token" }
        }
    };

    private static readonly object HealthResource = new
    {
        BasePath = "/health",
        Description = "System health monitoring",
        Endpoints = new[]
        {
            new { Method = "GET", Path = "/health", Description = "Get comprehensive system health status", Auth = "None" }
        }
    };

    private static readonly object AuthenticationInfo = new
    {
        Type = "Bearer Token",
        Header = "Authorization: Bearer {token}",
        Description = "Include the JWT token in the Authorization header for protected endpoints",
        TokenFormat = "JWT",
        Expiration = "1 hour"
    };

    private static readonly object StatusCodes = new
    {
        Success = new[] { 200, 201, 204 },
        ClientError = new[] { 400, 401, 403, 404, 422 },
        ServerError = new[] { 500, 502, 503 }
    };



    private static readonly object RegisterUserExample = new
    {
        Request = new
        {
            Method = "POST",
            Url = "/api/v1/auth/register",
            Body = new { Name = "john_doe", Password = "securePassword123" }
        },
        Response = new
        {
            Success = true,
            Message = "User registered successfully",
            Data = new
            {
                UserId = 1,
                Name = "john_doe",
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                TokenType = "Bearer",
                ExpiresIn = 3600
            }
        }
    };

    [HttpGet]
    public IActionResult GetApiDocumentation()
    {
        return Ok(new
        {
            Name = "Currency Tracker API",
            Version = "1.0",
            Description = "RESTful API for currency tracking and user management",
            BaseUrl = $"{Request.Scheme}://{Request.Host}/api/v1",
            Documentation = "Comprehensive API documentation for the Currency Tracker system",
            Resources = new
            {
                Authentication = AuthenticationResource,
                Currencies = CurrenciesResource,
                Favorites = FavoritesResource,
                Health = HealthResource
            },
            Authentication = AuthenticationInfo,
            StatusCodes,
            Examples = new
            {
                RegisterUser = RegisterUserExample
            }
        });
    }
} 