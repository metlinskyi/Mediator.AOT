using System.Text.Json.Serialization;

namespace Api.Handlers;

[JsonSerializable(typeof(Hello.HelloRequest))]
[JsonSerializable(typeof(Hello.HelloResponse))]
[JsonSerializable(typeof(Todo.Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
