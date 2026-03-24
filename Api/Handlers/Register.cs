namespace Api.Handlers;

    using Application;

public static class Register
{
    public static void RegisterHandlers(this  Mediator.Middleware.IMediatorRegister mediator)
    {
        mediator.Register<LoginRequest, LoginResponse, LoginHandler>(AppJsonSerializerContext.Default);
        mediator.Register<LogoutRequest, LogoutHandler>(AppJsonSerializerContext.Default);
    }
}