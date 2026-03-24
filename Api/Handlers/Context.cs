using System.Text.Json.Serialization;
using Api.Services.Security;

namespace Api.Handlers;

[JsonSerializable(typeof(Application.LoginRequest))]
[JsonSerializable(typeof(Application.LoginResponse))]
[JsonSerializable(typeof(Application.LogoutRequest))]
[JsonSerializable(typeof(Hello.HelloRequest))]
[JsonSerializable(typeof(Hello.HelloResponse))]
[JsonSerializable(typeof(Todo.TodoRequest))]
[JsonSerializable(typeof(Todo.TodoEntity))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

public static class Extensions
{
    public static IServiceCollection AddJsonOptions(this IServiceCollection services) 
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new SecureStringConverter());
        });

        return services;
    }
}