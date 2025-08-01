namespace Shared.Identity.Services;

public interface ISessionService
{
    string CreateSession(int userId, string userName);
    bool ValidateSession(string sessionToken);
    void InvalidateSession(string sessionToken);
    bool HasActiveSession(int userId);
} 