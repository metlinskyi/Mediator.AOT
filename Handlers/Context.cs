using System.Text.Json.Serialization;

namespace Api.Handlers;

[JsonSerializable(typeof(Hello.HelloRequest))]
[JsonSerializable(typeof(Hello.HelloResponse))]
[JsonSerializable(typeof(Todo.TodoRequest))]
[JsonSerializable(typeof(Todo.TodoEntity[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}