namespace Api.Handlers;

using Hello;
using Todo;
public static class Register
{
    public static void MediatorHandlers(IMediator mediator)
    {
        mediator.Register<HelloRequest, HelloResponse, HelloHandler>(AppJsonSerializerContext.Default);
        mediator.Register<TodoRequest, TodoEntity, TodoHandler>(AppJsonSerializerContext.Default);        
    }
}