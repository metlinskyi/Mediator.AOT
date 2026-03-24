using Microsoft.AspNetCore.Authorization;

namespace Mediator.Middleware;

public interface IMediatorHandlerInfo 
{
   Type RequestType { get; }
   Type? ResponseType { get; }
   IAuthorizeData[] AuthorizeData { get; }
}