using Microsoft.AspNetCore.Authorization;

namespace Api.Handlers.Application;

[Authorize]
public class LogoutHandler(
    ILogger<LogoutHandler> logger
    ) : IRequestHandler<LogoutRequest>
{
    public Task Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling LogoutRequest");

        return Task.CompletedTask;
    }
}