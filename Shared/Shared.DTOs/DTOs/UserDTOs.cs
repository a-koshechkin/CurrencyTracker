namespace Shared.DTOs.DTOs;

public record UserRegistrationRequest
{
    public string Name { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record UserLoginRequest
{
    public string Name { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record UserLoginResponse
{
    public int UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public int ExpiresIn { get; init; } = 3600; // 1 hour in seconds
}

public record UserLogoutRequest
{
    public string AccessToken { get; init; } = string.Empty;
}

public record ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public List<string> Errors { get; init; } = new();
} 