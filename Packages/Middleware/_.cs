using Microsoft.AspNetCore.Builder;

namespace Happy.Endpoint.Middleware;

public static class _
{
    public static IServiceCollection AddHappyEndpoints(this IServiceCollection services, 
        Action<IHappy.Middleware>? configure = null)
    {
        var middleware = new MiddlewareService(services);
        services.AddSingleton<IHappy.MiddlewareService>(middleware);
        configure?.Invoke(middleware);
        
        return services.ConfigureHttpJsonOptions(options =>
        {
            middleware.JsonSerializerContexts.ForEach(context => 
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, context)); 
        });
    }

    public static void MapHappyEndpoints(this WebApplication app, 
        Action<EndpointMappingOptions>? configure = null)
    {
        var middleware = app.Services.GetRequiredService<IHappy.MiddlewareService>();
        var options = new EndpointMappingOptions();
        configure?.Invoke(options);    

        app.MapPost(options.SendPattern, async ctx =>
        {
            var className = await ctx.GetClassNameFromRoute(options);
            if (className is null) return;
            await middleware.Send((HttpMethod.Post, className.ToUpper()), ctx, ctx.RequestAborted);
        });

        app.MapPut(options.SendPattern, async ctx =>
        {
            var className = await ctx.GetClassNameFromRoute(options);
            if (className is null) return;
            await middleware.Send((HttpMethod.Put, className.ToUpper()), ctx, ctx.RequestAborted);
        });
    }

    public static async Task AddSchema(this IHappy.Middleware middleware, 
        Func<EndpointMappingOptions, IEnumerable<IHappy.EndpointInfo>, Task> configure)
    {
        await configure(middleware.Options, middleware);
    }  
}