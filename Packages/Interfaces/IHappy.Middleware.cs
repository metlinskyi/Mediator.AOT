using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Happy.Endpoint.Middleware;

namespace Happy.Endpoint.Interfaces;

public partial interface IHappy
{
      public interface Middleware: IEnumerable<EndpointInfo>
      {
            EndpointMappingOptions Options { get; }

            void Register<TRequest, TResponse,
                  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
                  JsonSerializerContext context)
                  where THandler : Endpoint<TRequest, TResponse>;

            void Register<TRequest,
                  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
                  JsonSerializerContext context)
                  where THandler : Endpoint<TRequest>;
      }
}