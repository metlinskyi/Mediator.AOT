using System.Text.Json.Serialization.Metadata;

namespace Api.Middleware;

public sealed class MediatorService : 
    Dictionary<string, Func<IServiceProvider, HttpContext, CancellationToken, Task>>, 
    IMediator
{
    public MediatorService() : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public void Register<TRequest, TResponse, THandler>(
        JsonTypeInfo<TRequest> requestTypeInfo,
        JsonTypeInfo<TResponse> responseTypeInfo)
        where TRequest : IRequest
        where TResponse : IResponse
        where THandler : IRequestHandler<TRequest, TResponse>
    {
        var className = typeof(TRequest).Name;
        this[className] = async (sp, ctx, ct) =>
        {
            var handler = sp.GetRequiredService<THandler>();
            var request = await ctx.Request.ReadFromJsonAsync(requestTypeInfo, ct);
            if (request is null)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            var response = await handler.Handle(request, ct);
            await ctx.Response.WriteAsJsonAsync(response, responseTypeInfo, cancellationToken: ct);
        };
    }

    public async Task Send(string className, HttpContext ctx, CancellationToken ct)
    {
        if (!TryGetValue(className, out var handle))
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            await ctx.Response.WriteAsync($"No handler registered for '{className}'.", ct);
            return;
        }
        await handle(ctx.RequestServices, ctx, ct);
    }
}
