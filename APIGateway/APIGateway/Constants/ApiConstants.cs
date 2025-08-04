namespace APIGateway.Constants;

public static class ApiConstants
{
    public static class Routes
    {
        public const string Base = "/api/v1";
        public const string Auth = "/auth";
        public const string Currencies = "/currencies";
        public const string Favorites = "/favorites";
        public const string Health = "/health";
        public const string Documentation = "/docs";
    }

    public static class Policies
    {
        public const string AuthenticatedUser = "AuthenticatedUser";
        public const string AllowAll = "AllowAll";
    }

    public static class Headers
    {
        public const string Authorization = "Authorization";
        public const string UserId = "X-User-Id";
        public const string UserName = "X-User-Name";
    }

    public static class Authentication
    {
        public const string Bearer = "Bearer";
        public const string JwtBearer = "Bearer";
        public const string TokenType = "Bearer";
    }

    public static class Status
    {
        public const string Healthy = "Healthy";
        public const string Unhealthy = "Unhealthy";
        public const string Running = "Running";
    }

    public static class Messages
    {
        public const string GatewayRunning = "API Gateway is running";
        public const string ApiGatewayMessage = "Currency Tracker API Gateway";
    }
} 