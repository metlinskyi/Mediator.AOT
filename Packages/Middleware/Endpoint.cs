using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Happy.Endpoint.Middleware;

internal class Endpoint<TRequest, TResponse> 
{
    protected readonly EndpointInfo info;
    protected readonly JsonTypeInfo<TRequest> requestTypeInfo;
    protected readonly JsonTypeInfo<TResponse> responseTypeInfo;

    public Endpoint(EndpointInfo info, JsonTypeInfo<TRequest> requestTypeInfo, JsonTypeInfo<TResponse> responseTypeInfo)
    {
        this.info = info;
        this.requestTypeInfo = requestTypeInfo;
        this.responseTypeInfo = responseTypeInfo;
    }

    public virtual async Task Handle(IServiceProvider sp, HttpContext ctx, CancellationToken ct)
    {
        if (!await EnsureAuthorizedAsync(ctx, info))
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

    }

    protected async Task<bool> EnsureAuthorizedAsync(HttpContext ctx, IHappy.EndpointInfo info)
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

}

internal class Endpoint<TRequest> : Endpoint<TRequest, object?>
{
    public Endpoint(EndpointInfo info, JsonTypeInfo<TRequest> requestTypeInfo) 
        : base(info, requestTypeInfo, null!)
    {
    }
    
    public override async Task Handle(IServiceProvider sp, HttpContext ctx, CancellationToken ct)
    {
        if (!await EnsureAuthorizedAsync(ctx, info))
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
    }
}
