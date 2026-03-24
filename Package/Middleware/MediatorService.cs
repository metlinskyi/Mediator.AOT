using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Mediator.Middleware;

internal sealed partial class MediatorService : 
    Dictionary<(HttpMethod, string), Func<IServiceProvider, HttpContext, CancellationToken, Task>>, 
    IMediator
{
    private readonly IServiceCollection services;
    private readonly List<IMediatorHandlerInfo> handlerInfos = new List<IMediatorHandlerInfo>();
    public List<JsonSerializerContext> JsonSerializerContexts { get; } = new List<JsonSerializerContext>();
    public IEnumerable<IMediatorHandlerInfo> HandlerInfos => handlerInfos;
    public MediatorOptions Options { get; } = new MediatorOptions();
    public MediatorService(IServiceCollection services) : base()
    {
        this.services = services;
    }

    public async Task Send((HttpMethod, string) key, HttpContext ctx, CancellationToken ct)
    {
        if (!TryGetValue(key, out var handle))
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            await ctx.Response.WriteAsync($"No handler registered for '{key.Item2}'.", ct);
            return;
        }
        await handle(ctx.RequestServices, ctx, ct);
    }
}
