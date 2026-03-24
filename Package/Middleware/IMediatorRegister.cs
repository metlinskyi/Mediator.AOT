using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Mediator.Middleware;

public interface IMediatorRegister : IEnumerable<IMediatorHandlerInfo>
{
      MediatorOptions Options { get; }
      
      void Register<TRequest, TResponse,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
            JsonSerializerContext context)
            where THandler : IRequestHandler<TRequest, TResponse>;

      void Register<TRequest,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
            JsonSerializerContext context)
            where THandler : IRequestHandler<TRequest>;
}