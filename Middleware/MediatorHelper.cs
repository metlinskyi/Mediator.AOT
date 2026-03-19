
namespace Api.Middleware;

    using Schema;

public static class MediatorHelper
{
    public static void AddMediator(this IServiceCollection services, Action<IMediator>? configure = null)
    {
        var mediator = new MediatorService(services);
        services.AddSingleton<IMediator>(mediator);
        configure?.Invoke(mediator);
        
        services.ConfigureHttpJsonOptions(options =>
        {
            mediator.JsonSerializerContexts.ForEach(context => 
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, context)); 
        });
    }

    public static void MapMediator(this WebApplication app, Action<MediatorOptions>? configure = null)
    {
        var mediator = app.Services.GetRequiredService<IMediator>();
        var options = new MediatorOptions();
        configure?.Invoke(options);    

        app.MapPost(options.SendPattern, async (string className, HttpContext ctx, CancellationToken ct) =>
            await mediator.Send(className, ctx, ct));      
    }
}