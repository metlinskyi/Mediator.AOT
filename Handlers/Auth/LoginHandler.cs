namespace Api.Handlers.Auth;

internal class LoginHandler(ILogger<LoginHandler> logger) : IRequestHandler<LoginRequest, LoginResponse>
{
    public Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling LoginRequest");
        return Task.FromResult(new LoginResponse("Login successful!"));
    }
}