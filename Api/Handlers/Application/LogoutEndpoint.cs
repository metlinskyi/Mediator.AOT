using System.IdentityModel.Tokens.Jwt;
using Api.Services.Security;
using Microsoft.AspNetCore.Authorization;

namespace Api.Handlers.Application;

[Authorize]
public class LogoutEndpoint(
    ILogger<LogoutEndpoint> logger,
    IHttpContextAccessor httpContextAccessor,
    ITokenRevocationService tokenRevocationService
    ) : IHappy.Endpoint<LogoutRequest>
{
    public Task Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var username = user?.Identity?.Name ?? "unknown";

        var jti = user?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
            ?? user?.FindFirst("jti")?.Value;
        var exp = user?.FindFirst(JwtRegisteredClaimNames.Exp)?.Value
            ?? user?.FindFirst("exp")?.Value;

        if (string.IsNullOrWhiteSpace(jti))
        {
            logger.LogWarning("Logout requested for user {Username} but JWT does not include jti claim.", username);
            return Task.CompletedTask;
        }

        var expiresAtUtc = DateTimeOffset.UtcNow.AddHours(1);
        if (long.TryParse(exp, out var expUnixSeconds))
            expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expUnixSeconds);

        tokenRevocationService.Revoke(jti, expiresAtUtc);
        logger.LogInformation("Logout completed for user {Username}; token id {Jti} revoked.", username, jti);

        return Task.CompletedTask;
    }
}