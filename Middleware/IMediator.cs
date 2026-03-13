namespace Api.Middleware;

public interface IMediator : IMediatorRegister
{
   Task Send(string className, HttpContext ctx, CancellationToken ct);
}