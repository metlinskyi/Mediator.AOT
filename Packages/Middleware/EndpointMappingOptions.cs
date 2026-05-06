namespace Happy.Endpoint.Middleware;

public sealed class EndpointMappingOptions
{
    public string SendPattern { get; set;} = "mediator/send/{className}";
    public string RouteTag { get; set;} =  "className";
}