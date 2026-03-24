namespace Mediator.Middleware;

public class MediatorOptions
{
    public string SendPattern { get;set;} = "mediator/send/{className}";

    public string RouteTag { get;set;} =  "className";

}