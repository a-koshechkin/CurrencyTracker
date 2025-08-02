using System.Security.Claims;

namespace Shared.Identity.Services;

public interface IJwtService
{
    string GenerateToken(int userId, string userName, string role = "User");
    ClaimsPrincipal? ValidateToken(string token);
    int? GetUserIdFromToken(string token);
    string? GetUserNameFromToken(string token);
} 