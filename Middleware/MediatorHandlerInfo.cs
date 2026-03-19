namespace Api.Middleware;

internal record MediatorHandlerInfo (
    Type RequestType,
    Type ResponseType
) : IMediatorHandlerInfo;