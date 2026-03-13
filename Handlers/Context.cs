using System.Text.Json.Serialization;

namespace Api.Handlers;


[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
