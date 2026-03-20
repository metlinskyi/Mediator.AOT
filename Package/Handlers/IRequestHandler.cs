namespace Mediator.Handlers;
/// <summary>
/// Marker interface for request handlers. 
/// This is used to register all request handlers in the DI container.
/// </summary>
public interface IRequestHandler
{
}
/// <summary>
/// Interface for handling requests.
/// </summary>
public interface IRequestHandler<in TRequest, TResponse> : IRequestHandler
    where TRequest : IRequest
    where TResponse : IResponse
{   
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
/// <summary>
/// Interface for handling requests that do not return a response.
/// </summary>
public interface IRequestHandler<in TRequest> : IRequestHandler
    where TRequest : IRequest
{   
    Task Handle(TRequest request, CancellationToken cancellationToken);
}