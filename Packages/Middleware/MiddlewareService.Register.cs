using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Authorization;

namespace Happy.Endpoint.Middleware;

internal sealed partial class MiddlewareService : IHappy.Middleware
{
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

        var endpoint = new Endpoint<TRequest, TResponse>(info, requestTypeInfo, responseTypeInfo);
        this[endpoint.CreateKey()] = endpoint.Handle;
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

        var endpoint = new Endpoint<TRequest>(info, requestTypeInfo);
        this[endpoint.CreateKey()] = endpoint.Handle;
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