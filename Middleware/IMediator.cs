namespace Api.Middleware;

public interface IMediator 
{
   Task Send(string className, HttpContext ctx, CancellationToken ct);
}