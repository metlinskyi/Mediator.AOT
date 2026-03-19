namespace Api.Middleware;

public interface IMediatorHandlerInfo 
{
   Type RequestType { get; }
   Type ResponseType { get; }
}