
namespace Api.Middleware;

    using Schema;

public static class MediatorHelper
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Action<IMediatorRegister>? configure = null)
    {
        var mediator = new MediatorService(services);
        services.AddSingleton<IMediator>(mediator);
        services.AddSingleton<IMediatorRegister>(mediator);
        configure?.Invoke(mediator);
        
        services.ConfigureHttpJsonOptions(options =>
        {
            mediator.JsonSerializerContexts.ForEach(context => 
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, context)); 
        });

        return services;
    }

    public static void MapMediator(this WebApplication app, Action<MediatorOptions>? configure = null)
    {
        var mediator = app.Services.GetRequiredService<IMediator>();
        var options = new MediatorOptions();
        configure?.Invoke(options);    

        app.MapPost(options.SendPattern, async (string className, HttpContext ctx, CancellationToken ct) =>
            await mediator.Send(className, ctx, ct));      
    }

    public static async Task AddSchema(this IMediatorRegister register, Func<IMediatorHandlerInfo, Task> configure)
    {
        foreach (var info in register.HandlerInfos)
        {
            await configure(info);
        } 
    }

}