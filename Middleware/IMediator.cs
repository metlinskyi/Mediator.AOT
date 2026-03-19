namespace Api.Middleware;

public interface IMediator : IMediatorRegister
{
   ISet<Type> RequestTypes { get; }
   ISet<Type> ResponseTypes { get; }
   Task Send(string className, HttpContext ctx, CancellationToken ct);
}