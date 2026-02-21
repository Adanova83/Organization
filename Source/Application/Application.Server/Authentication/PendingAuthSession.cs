using System;
using System.Security.Claims;

namespace Application.Server.Authentication;

// Authentication/PendingAuthSession.cs
public class PendingAuthSession
{
    private readonly Dictionary<string, ClaimsPrincipal> _pending = new();

    public string Store(ClaimsPrincipal principal)
    {
        var token = Guid.NewGuid().ToString();
        _pending[token] = principal;
        return token;
    }

    public ClaimsPrincipal? Retrieve(string token)
    {
        if (_pending.TryGetValue(token, out var principal))
        {
            _pending.Remove(token);
            return principal;
        }
        return null;
    }
}