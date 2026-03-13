using System.Text.Json.Serialization.Metadata;

namespace Api.Middleware;

public sealed class MediatorService
{
    private readonly Dictionary<string, Func<HttpContext, CancellationToken, Task>> _handlers =
        new(StringComparer.OrdinalIgnoreCase);

    public void Register<TRequest, TResponse>(
        IRequestHandler<TRequest, TResponse> handler,
        JsonTypeInfo<TRequest> requestTypeInfo,
        JsonTypeInfo<TResponse> responseTypeInfo)
        where TRequest : IRequest
        where TResponse : IResponse
    {
        var className = typeof(TRequest).Name;
        _handlers[className] = async (ctx, ct) =>
        {
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
        if (!_handlers.TryGetValue(className, out var handle))
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            await ctx.Response.WriteAsync($"No handler registered for '{className}'.", ct);
            return;
        }
        await handle(ctx, ct);
    }
}
