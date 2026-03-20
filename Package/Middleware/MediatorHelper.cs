using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Mediator.Middleware;

public static class MediatorHelper
{
    public static IServiceCollection AddMediator(this IServiceCollection services, 
        Action<IMediatorRegister>? configure = null)
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

    public static void MapMediator(this WebApplication app, 
        Action<MediatorOptions>? configure = null)
    {
        var mediator = app.Services.GetRequiredService<IMediator>();
        var options = new MediatorOptions();
        configure?.Invoke(options);    

        app.MapPost(options.SendPattern, async ctx =>
        {
            var className = ctx.Request.RouteValues["className"]?.ToString();
            if (string.IsNullOrWhiteSpace(className))
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                await ctx.Response.WriteAsync("Route parameter 'className' is required.", ctx.RequestAborted);
                return;
            }

            await mediator.Send(className, ctx, ctx.RequestAborted);
        });
    }

    public static async Task AddSchema(this IMediatorRegister register, 
        Func<MediatorOptions, IEnumerable<IMediatorHandlerInfo>, Task> configure)
    {
        await configure(register.Options, register.HandlerInfos);
    }       
}