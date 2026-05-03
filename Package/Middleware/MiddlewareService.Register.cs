using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Happy.Endpoint.Middleware;

internal sealed partial class MiddlewareService : IHappy.Middleware
{
    private static async Task<bool> EnsureAuthorizedAsync<THandler>(HttpContext ctx, IHappy.EndpointInfo info)
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
        where THandler : IHappy.Endpoint<TRequest, TResponse>
    {
        if(!JsonSerializerContexts.Contains(context))
            JsonSerializerContexts.Add(context);
        
        services.AddScoped(typeof(IHappy.Endpoint<TRequest, TResponse>), typeof(THandler));
        
        var requestTypeInfo = context.GetTypeInfo(typeof(TRequest)) as JsonTypeInfo<TRequest> 
            ?? throw new InvalidOperationException($"Unable to get JsonTypeInfo for {typeof(TRequest).FullName}.");
        var responseTypeInfo = context.GetTypeInfo(typeof(TResponse)) as JsonTypeInfo<TResponse>
            ?? throw new InvalidOperationException($"Unable to get JsonTypeInfo for {typeof(TResponse).FullName}.");
            
        var info = new EndpointInfo(HttpMethod.Post, typeof(TRequest), typeof(TResponse))
        {
            AuthorizeData = typeof(THandler)
                .GetCustomAttributes(inherit: true)
                .OfType<IAuthorizeData>()
                .ToArray()
        };
        endpoints.Add(info);
               
        var key = info.CreateKey();
        this[key] = async (sp, ctx, ct) =>
        {
            if (!await EnsureAuthorizedAsync<THandler>(ctx, info))
                return;

            var handler = sp.GetRequiredService<IHappy.Endpoint<TRequest, TResponse>>();
            var request = await ctx.Request.ReadFromJsonAsync(requestTypeInfo, ct);
            if (request is null)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var response = await handler.Handle(request, ct);
            ctx.Response.StatusCode = StatusCodes.Status200OK;
            ctx.Response.Headers.TryAdd("Response-Type", typeof(TResponse).Name);
            await ctx.Response.WriteAsJsonAsync(response, responseTypeInfo, cancellationToken: ct);
        };
    }

    public void Register<TRequest, THandler>(JsonSerializerContext context) 
        where THandler : IHappy.Endpoint<TRequest>
    {
        if(!JsonSerializerContexts.Contains(context))
            JsonSerializerContexts.Add(context);
        
        services.AddScoped(typeof(IHappy.Endpoint<TRequest>), typeof(THandler));
        
        var requestTypeInfo = context.GetTypeInfo(typeof(TRequest)) as JsonTypeInfo<TRequest> 
            ?? throw new InvalidOperationException($"Unable to get JsonTypeInfo for {typeof(TRequest).FullName}.");
 
        var info = new EndpointInfo(HttpMethod.Put, typeof(TRequest))
        {
            AuthorizeData = typeof(THandler)
                .GetCustomAttributes(inherit: true)
                .OfType<IAuthorizeData>()
                .ToArray()
        };
        endpoints.Add(info);

        var key = info.CreateKey();
        this[key] = async (sp, ctx, ct) =>
        {
            if (!await EnsureAuthorizedAsync<THandler>(ctx, info))
                return;

            var handler = sp.GetRequiredService<IHappy.Endpoint<TRequest>>();
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

    IEnumerator<IHappy.EndpointInfo> IEnumerable<IHappy.EndpointInfo>.GetEnumerator()
    {
        return endpoints.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return endpoints.GetEnumerator();
    }
}