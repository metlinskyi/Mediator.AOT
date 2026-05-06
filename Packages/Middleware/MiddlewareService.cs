using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Happy.Endpoint.Middleware;

internal sealed partial class MiddlewareService : 
    Dictionary<(HttpMethod, string), Func<IServiceProvider, HttpContext, CancellationToken, Task>>, 
    IHappy.MiddlewareService
{
    private readonly IServiceCollection services;
    private readonly List<EndpointInfo> endpoints = [];
    public List<JsonSerializerContext> JsonSerializerContexts { get; } = new List<JsonSerializerContext>();
    public EndpointMappingOptions Options { get; } = new();
    
    public MiddlewareService(IServiceCollection services) : base()
    {
        this.services = services;
    }

    public async Task Send((HttpMethod, string) key, HttpContext ctx, CancellationToken ct)
    {
        if (!TryGetValue(key, out var handle))
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            await ctx.Response.WriteAsync($"Endpoint '{key.Item2}' not found.", ct);
            return;
        }
        await handle(ctx.RequestServices, ctx, ct);
    }
}
