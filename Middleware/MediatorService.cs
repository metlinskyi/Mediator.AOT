using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Api.Middleware;

public sealed class MediatorService : 
    Dictionary<string, Func<IServiceProvider, HttpContext, CancellationToken, Task>>, 
    IMediator
{
    private readonly IServiceCollection services;

    public List<JsonSerializerContext> JsonSerializerContexts { get; } = new List<JsonSerializerContext>();    

    public MediatorService(IServiceCollection services) : base(StringComparer.OrdinalIgnoreCase)
    {
        this.services = services;
    }

    public void Register<
        TRequest, 
        TResponse, 
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
        JsonSerializerContext context)
        where TRequest : IRequest
        where TResponse : IResponse
        where THandler : IRequestHandler<TRequest, TResponse>
    {
        if(!JsonSerializerContexts.Contains(context))
            JsonSerializerContexts.Add(context);

        services.AddScoped(typeof(IRequestHandler<TRequest, TResponse>), typeof(THandler));

        var requestTypeInfo = context.GetTypeInfo(typeof(TRequest)) as JsonTypeInfo<TRequest> 
            ?? throw new InvalidOperationException($"Unable to get JsonTypeInfo for {typeof(TRequest).FullName}.");
        var responseTypeInfo = context.GetTypeInfo(typeof(TResponse)) as JsonTypeInfo<TResponse>
            ?? throw new InvalidOperationException($"Unable to get JsonTypeInfo for {typeof(TResponse).FullName}.");

        var className = typeof(TRequest).Name;
        this[className] = async (sp, ctx, ct) =>
        {
            var handler = sp.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
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
