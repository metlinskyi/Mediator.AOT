using Api.Services.Security;
using Microsoft.AspNetCore.Mvc;

namespace Api.Handlers.Application;

public class LoginHandler(
    ILogger<LoginHandler> logger,
    ISecurityService securityService
    ) : IRequestHandler<LoginRequest, LoginResponse>
{
    public Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling LoginRequest");
        var secureToken = securityService.ConvertToSecureString("TEST".ToCharArray());
        secureToken.MakeReadOnly();
        return Task.FromResult(new LoginResponse(secureToken));
    }
}