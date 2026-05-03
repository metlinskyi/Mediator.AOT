using System.IdentityModel.Tokens.Jwt;
using Api.Services.Security;

namespace Api.Handlers.Application;

public class LoginEndpoint(
    ILogger<LoginEndpoint> logger,
    IAuthService authService,
    ISecurityService securityService
    ) : IHappy.Endpoint<LoginRequest, LoginResponse>
{
    public Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to sign in user: {Username}", request.Username);

        try
        {
            if (!authService.IsCredentialValid(request.Username, request.Password))
            {
                logger.LogWarning("Failed sign in attempt for user: {Username}", request.Username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
        }
        finally
        {
            request.Password.Dispose();
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = securityService.CreateTokenDescriptor(request.Username);

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Task.FromResult(new LoginResponse(tokenHandler.WriteToken(token)));
    }
}