using APIGateway.DTOs;

namespace APIGateway.Configuration;

public static class ApiEndpoints
{
    public static class Authentication
    {
        public const string BasePath = "/auth";
        public const string Description = "User authentication and authorization";
        
        public static readonly EndpointInfo[] Endpoints =
        [
            new() { Method = "POST", Path = "/auth/register", Description = "Register a new user", Auth = "None" },
            new() { Method = "POST", Path = "/auth/login", Description = "Authenticate user and get token", Auth = "None" },
            new() { Method = "POST", Path = "/auth/logout", Description = "Logout user", Auth = "Bearer Token" }
        ];
    }

    public static class Currencies
    {
        public const string BasePath = "/currencies";
        public const string Description = "Currency information and data";
        
        public static readonly EndpointInfo[] Endpoints =
        [
            new() { Method = "GET", Path = "/currencies", Description = "Get all available currencies", Auth = "Bearer Token" }
        ];
    }

    public static class Favorites
    {
        public const string BasePath = "/favorites";
        public const string Description = "User favorite currencies management";
        
        public static readonly EndpointInfo[] Endpoints =
        [
            new() { Method = "GET", Path = "/favorites", Description = "Get user's favorite currencies", Auth = "Bearer Token" },
            new() { Method = "POST", Path = "/favorites", Description = "Add currency to favorites", Auth = "Bearer Token" },
            new() { Method = "DELETE", Path = "/favorites/{currencyCode}", Description = "Remove currency from favorites", Auth = "Bearer Token" }
        ];
    }

    public static class Health
    {
        public const string BasePath = "/health";
        public const string Description = "System health monitoring";
        
        public static readonly EndpointInfo[] Endpoints =
        [
            new() { Method = "GET", Path = "/health", Description = "Get comprehensive system health status", Auth = "None" }
        ];
    }

    public static class AuthenticationInfo
    {
        public const string Type = "Bearer Token";
        public const string Header = "Authorization: Bearer {token}";
        public const string Description = "Include the JWT token in the Authorization header for protected endpoints";
        public const string TokenFormat = "JWT";
        public const string Expiration = "1 hour";
    }

    public static class StatusCodes
    {
        public static readonly int[] Success = [200, 201, 204];
        public static readonly int[] ClientError = [400, 401, 403, 404, 422];
        public static readonly int[] ServerError = [500, 502, 503];
    }
} 