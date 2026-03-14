using System.Text.Json.Serialization;
using Api.Middleware.Schema;

namespace Api.Middleware;

[JsonSerializable(typeof(MediatorSchemaRequest))]
[JsonSerializable(typeof(MediatorSchema))]
internal partial class MediatorContext : JsonSerializerContext
{
}