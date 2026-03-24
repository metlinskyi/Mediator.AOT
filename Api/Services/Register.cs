namespace Api.Services;
    using Security;

public static class Register
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISecurityService, SecurityService>();
        services.AddSingleton<ITokenRevocationService, TokenRevocationService>();
        services.AddHttpContextAccessor();

        return services;
    }
}