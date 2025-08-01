using Shared.Identity.Models;
using System.Security.Cryptography;

namespace Shared.Identity.Services;

public class SimpleSessionService : ISessionService
{
    private readonly Dictionary<string, SessionInfo> _sessions = [];
    private readonly object _lock = new();

    public string CreateSession(int userId, string userName)
    {
        var sessionToken = GenerateSessionToken();
        var sessionInfo = new SessionInfo
        {
            UserId = userId,
            UserName = userName
        };

        lock (_lock)
        {
            _sessions[sessionToken] = sessionInfo;
        }

        return sessionToken;
    }

    public bool ValidateSession(string sessionToken)
    {
        if (string.IsNullOrEmpty(sessionToken))
            return false;

        lock (_lock)
        {
            return _sessions.ContainsKey(sessionToken);
        }
    }

    public void InvalidateSession(string sessionToken)
    {
        if (string.IsNullOrEmpty(sessionToken))
            return;

        lock (_lock)
        {
            _sessions.Remove(sessionToken);
        }
    }

    public bool HasActiveSession(int userId)
    {
        lock (_lock)
        {
            return _sessions.Values.Any(session => session.UserId == userId);
        }
    }

    private string GenerateSessionToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
} 