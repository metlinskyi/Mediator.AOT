namespace Api.Handlers;

using Api.Handlers.Auth;
using Hello;
using Todo;
public static class Register
{
    public static void RegisterHandlers(this IMediatorRegister mediator)
    {
        mediator.Register<LoginRequest, LoginResponse, LoginHandler>(AppJsonSerializerContext.Default);
        mediator.Register<HelloRequest, HelloResponse, HelloHandler>(AppJsonSerializerContext.Default);
        mediator.Register<TodoRequest, TodoEntity, TodoHandler>(AppJsonSerializerContext.Default);        
    }
}