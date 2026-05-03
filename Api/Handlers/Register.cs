namespace Api.Handlers;

    using Application;

public static class Register
{
    public static void EndpointsCollection(this IHappy.Middleware middleware)
    {
        middleware.Register<LoginRequest, LoginResponse, LoginEndpoint>(AppJsonSerializerContext.Default);
        middleware.Register<LogoutRequest, LogoutEndpoint>(AppJsonSerializerContext.Default);
    }
}