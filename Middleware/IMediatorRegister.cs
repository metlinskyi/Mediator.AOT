using System.Text.Json.Serialization.Metadata;

namespace Api.Middleware;

public interface IMediatorRegister
{
      void Register<TRequest, TResponse, THandler>(
         JsonTypeInfo<TRequest> requestTypeInfo,
         JsonTypeInfo<TResponse> responseTypeInfo)
         where TRequest : IRequest
         where TResponse : IResponse
         where THandler : IRequestHandler<TRequest, TResponse>;
   }