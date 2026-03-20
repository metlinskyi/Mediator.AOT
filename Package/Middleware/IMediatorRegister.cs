using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Mediator.Middleware;

public interface IMediatorRegister
{
      MediatorOptions Options { get; }
      
      IEnumerable<IMediatorHandlerInfo> HandlerInfos { get; }

      void Register<TRequest, TResponse,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
            JsonSerializerContext context)
            where THandler : IRequestHandler<TRequest, TResponse>;
}