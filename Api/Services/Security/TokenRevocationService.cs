using System.Collections.Concurrent;

namespace Api.Services.Security;

public sealed class TokenRevocationService : ITokenRevocationService
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> revokedTokens = new();

    public void Revoke(string jti, DateTimeOffset expiresAtUtc)
    {
        CleanupExpired();
        revokedTokens[jti] = expiresAtUtc;
    }

    public bool IsRevoked(string jti)
    {
        CleanupExpired();
        return revokedTokens.ContainsKey(jti);
    }

    private void CleanupExpired()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var token in revokedTokens)
        {
            if (token.Value <= now)
                revokedTokens.TryRemove(token.Key, out _);
        }
    }
}
