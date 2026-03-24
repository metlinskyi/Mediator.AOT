namespace Mediator.Middleware;

internal record MediatorHandlerInfo (
    Type RequestType,
    Type? ResponseType = null
) : IMediatorHandlerInfo;