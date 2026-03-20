namespace Mediator.Middleware;

public class MediatorOptions
{
    public string SendPattern { get;set;} = "mediator/send/{className}";
    public string SchemaPattern { get;set;} = "mediator";
}