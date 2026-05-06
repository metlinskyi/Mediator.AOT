using Microsoft.AspNetCore.Http;

namespace Happy.Endpoint.Middleware;
        
internal static class MiddlewareHelper
{
    public static async Task<string?> GetClassNameFromRoute(this HttpContext ctx, EndpointMappingOptions options)
    {
        var className = ctx.Request.RouteValues[options.RouteTag]?.ToString();
        if (string.IsNullOrWhiteSpace(className))
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ctx.Response.WriteAsync($"Route parameter '{options.RouteTag}' is required.", ctx.RequestAborted);
            return null;
        }
        return className;
    } 
}