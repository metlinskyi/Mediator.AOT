namespace Api.Handlers;

    using Application;

public static class Register
{
    public static void EndpointsCollection(this IHappy.Middleware middleware)
    {
        middleware.Register<LoginRequest, LoginResponse, LoginHandler>(AppJsonSerializerContext.Default);
        middleware.Register<LogoutRequest, LogoutHandler>(AppJsonSerializerContext.Default);
    }
}