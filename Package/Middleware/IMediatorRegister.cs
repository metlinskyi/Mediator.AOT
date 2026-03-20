using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Mediator.Middleware;

public interface IMediatorRegister
{
      IEnumerable<IMediatorHandlerInfo> HandlerInfos { get; }

      void Register<TRequest, TResponse, 
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
            JsonSerializerContext context)
            where TRequest : IRequest
            where TResponse : IResponse
            where THandler : IRequestHandler<TRequest, TResponse>;
}