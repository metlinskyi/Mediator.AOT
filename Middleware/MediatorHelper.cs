using Api.Middleware.Schema;

namespace Api.Middleware;

public static class MediatorHelper
{
    public static void AddMediator(this IServiceCollection services, Action<IMediator>? configure = null)
    {
        var mediator = new MediatorService(services);
        services.AddSingleton<IMediator>(mediator);
        configure?.Invoke(mediator);
        
        mediator.Register<MediatorSchemaRequest, MediatorSchema, MediatorSchemaHandler>(MediatorContext.Default);

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

        app.MapGet(options.SchemaPattern, async (HttpContext ctx, CancellationToken ct) =>
            await mediator.Send(typeof(MediatorSchemaRequest).Name, ctx, ct));           
    }
}