using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Mediator.Middleware;

internal sealed partial class MediatorService : IMediatorRegister
{
    private static async Task<bool> EnsureAuthorizedAsync<THandler>(HttpContext ctx, IMediatorHandlerInfo info)
    {
        if (info.AuthorizeData.Length == 0)
            return true;

        var policyProvider = ctx.RequestServices.GetRequiredService<IAuthorizationPolicyProvider>();
        var policy = await AuthorizationPolicy.CombineAsync(policyProvider, info.AuthorizeData);
        if (policy is null)
            return true;

        var authorizationService = ctx.RequestServices.GetRequiredService<IAuthorizationService>();
        var result = await authorizationService.AuthorizeAsync(ctx.User, resource: null, policy);
        if (result.Succeeded)
            return true;

        ctx.Response.StatusCode = (ctx.User.Identity?.IsAuthenticated ?? false)
            ? StatusCodes.Status403Forbidden
            : StatusCodes.Status401Unauthorized;
        return false;
    }

    public void Register<TRequest, TResponse, THandler>(JsonSerializerContext context)
        where THandler : IRequestHandler<TRequest, TResponse>
    {
        if(!JsonSerializerContexts.Contains(context))
            JsonSerializerContexts.Add(context);
        
        services.AddScoped(typeof(IRequestHandler<TRequest, TResponse>), typeof(THandler));
        
        var requestTypeInfo = context.GetTypeInfo(typeof(TRequest)) as JsonTypeInfo<TRequest> 
            ?? throw new InvalidOperationException($"Unable to get JsonTypeInfo for {typeof(TRequest).FullName}.");
        var responseTypeInfo = context.GetTypeInfo(typeof(TResponse)) as JsonTypeInfo<TResponse>
            ?? throw new InvalidOperationException($"Unable to get JsonTypeInfo for {typeof(TResponse).FullName}.");
            
        var info = new MediatorHandlerInfo(typeof(TRequest), typeof(TResponse))
        {
            AuthorizeData = typeof(THandler)
                .GetCustomAttributes(inherit: true)
                .OfType<IAuthorizeData>()
                .ToArray()
        };
        handlerInfos.Add(info);
               
        var key = (HttpMethod.Post, typeof(TRequest).Name.ToUpper());
        this[key] = async (sp, ctx, ct) =>
        {
            if (!await EnsureAuthorizedAsync<THandler>(ctx, info))
                return;

            var handler = sp.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            var request = await ctx.Request.ReadFromJsonAsync(requestTypeInfo, ct);
            if (request is null)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var response = await handler.Handle(request, ct);
            ctx.Response.Headers.TryAdd("Response-Type", typeof(TResponse).Name);
            await ctx.Response.WriteAsJsonAsync(response, responseTypeInfo, cancellationToken: ct);
        };
    }

    public void Register<TRequest, THandler>(JsonSerializerContext context) 
        where THandler : IRequestHandler<TRequest>
    {
        if(!JsonSerializerContexts.Contains(context))
            JsonSerializerContexts.Add(context);
        
        services.AddScoped(typeof(IRequestHandler<TRequest>), typeof(THandler));
        
        var requestTypeInfo = context.GetTypeInfo(typeof(TRequest)) as JsonTypeInfo<TRequest> 
            ?? throw new InvalidOperationException($"Unable to get JsonTypeInfo for {typeof(TRequest).FullName}.");
 
        var info = new MediatorHandlerInfo(typeof(TRequest))
        {
            AuthorizeData = typeof(THandler)
                .GetCustomAttributes(inherit: true)
                .OfType<IAuthorizeData>()
                .ToArray()
        };
        handlerInfos.Add(info);

        var key = (HttpMethod.Put, typeof(TRequest).Name.ToUpper());
        this[key] = async (sp, ctx, ct) =>
        {
            if (!await EnsureAuthorizedAsync<THandler>(ctx, info))
                return;

            var handler = sp.GetRequiredService<IRequestHandler<TRequest>>();
            var request = await ctx.Request.ReadFromJsonAsync(requestTypeInfo, ct);
            if (request is null)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
    
            await handler.Handle(request, ct);
            ctx.Response.StatusCode = StatusCodes.Status204NoContent;
        };
    } 
}