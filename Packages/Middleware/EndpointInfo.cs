using Microsoft.AspNetCore.Authorization;

namespace Happy.Endpoint.Middleware;

internal record EndpointInfo(
    HttpMethod Method,
    Type RequestType,
    Type? ResponseType = null
) : IHappy.EndpointInfo
{
    public IAuthorizeData[] AuthorizeData {get; init;} = Array.Empty<IAuthorizeData>();
}