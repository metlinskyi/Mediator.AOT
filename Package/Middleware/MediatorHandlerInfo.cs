using Microsoft.AspNetCore.Authorization;

namespace Mediator.Middleware;

internal record MediatorHandlerInfo(
    Type RequestType,
    Type? ResponseType = null
) : IMediatorHandlerInfo
{
    public IAuthorizeData[] AuthorizeData {get; init;} = Array.Empty<IAuthorizeData>();
}