using Microsoft.AspNetCore.Http;

namespace Mediator.Middleware;

public interface IMediator 
{
   Task Send((HttpMethod, string) key, HttpContext ctx, CancellationToken ct);
}