namespace Api.Services;
    using Security;

public static class Register
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISecurityService, SecurityService>();

        return services;
    }
}