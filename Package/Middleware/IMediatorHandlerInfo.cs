using Microsoft.AspNetCore.Authorization;

namespace Mediator.Middleware;

public interface IMediatorHandlerInfo 
{
   HttpMethod Method { get; }
   Type RequestType { get; }
   Type? ResponseType { get; }
   IAuthorizeData[] AuthorizeData { get; }
}