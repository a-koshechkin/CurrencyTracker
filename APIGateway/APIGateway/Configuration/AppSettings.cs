namespace APIGateway.Configuration;

public class AppSettings
{
    public JwtSettings Jwt { get; set; } = new();
    public ServiceSettings Services { get; set; } = new();
    public ApiSettings Api { get; set; } = new();
    public CorsSettings Cors { get; set; } = new();
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}

public class ServiceSettings
{
    public string UserService { get; set; } = string.Empty;
    public string FinanceService { get; set; } = string.Empty;
}

public class ApiSettings
{
    public string Name { get; set; } = "Currency Tracker API";
    public string Version { get; set; } = "1.0";
    public string Description { get; set; } = "RESTful API for currency tracking and user management";
    public string BasePath { get; set; } = "/api/v1";
    public string DocumentationPath { get; set; } = "/api/v1/docs";
    public string HealthPath { get; set; } = "/api/health";
}

public class CorsSettings
{
    public string PolicyName { get; set; } = "AllowAll";
    public bool AllowAnyOrigin { get; set; } = true;
    public bool AllowAnyMethod { get; set; } = true;
    public bool AllowAnyHeader { get; set; } = true;
} 