using Microsoft.AspNetCore.Http;

namespace Mediator.Middleware;

public interface IMediator 
{
   Task Send(string className, HttpContext ctx, CancellationToken ct);
}