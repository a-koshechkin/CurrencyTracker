namespace Shared.DTOs.DTOs;

public record HealthResponse
{
    public ServiceInfo Service { get; init; } = new();
}

public record ServiceInfo
{
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string Version { get; init; } = string.Empty;
}

public record GatewayHealthResponse
{
    public GatewayInfo Gateway { get; init; } = new();
    public Dictionary<string, ServiceHealthInfo> Services { get; init; } = new();
}

public record GatewayInfo
{
    public string Status { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string Message { get; init; } = string.Empty;
}

public record ServiceHealthInfo
{
    public string Status { get; init; } = string.Empty;
    public object? Response { get; init; }
    public int? HttpStatus { get; init; }
    public string? Error { get; init; }
} 