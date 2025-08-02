using System.Security.Claims;

namespace FinanceService.API.Middleware;

public class UserContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
        var userName = context.Request.Headers["X-User-Name"].FirstOrDefault();

        if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out var userIdInt))
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
                new("UserId", userId)
            };

            if (!string.IsNullOrEmpty(userName))
            {
                claims.Add(new Claim(ClaimTypes.Name, userName));
            }

            var identity = new ClaimsIdentity(claims, "API Gateway");
            var principal = new ClaimsPrincipal(identity);
            
            context.User = principal;
        }

        await _next(context);
    }
}

public static class UserContextMiddlewareExtensions
{
    public static IApplicationBuilder UseUserContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserContextMiddleware>();
    }
} 