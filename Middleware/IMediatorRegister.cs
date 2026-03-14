using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Api.Middleware;

public interface IMediatorRegister
{
      void Register<TRequest, TResponse, 
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
            JsonSerializerContext context)
            where TRequest : IRequest
            where TResponse : IResponse
            where THandler : IRequestHandler<TRequest, TResponse>;
   }