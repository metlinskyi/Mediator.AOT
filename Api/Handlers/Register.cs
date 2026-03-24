namespace Api.Handlers;

    using Application;
    using Hello;
    using Todo;

public static class Register
{
    public static void RegisterHandlers(this  Mediator.Middleware.IMediatorRegister mediator)
    {
        mediator.Register<LoginRequest, LoginResponse, LoginHandler>(AppJsonSerializerContext.Default);
        mediator.Register<LogoutRequest, LogoutHandler>(AppJsonSerializerContext.Default);
        mediator.Register<HelloRequest, HelloResponse, HelloHandler>(AppJsonSerializerContext.Default);
        mediator.Register<TodoRequest, TodoEntity, TodoHandler>(AppJsonSerializerContext.Default);        
    }
}