namespace Mediator.Handlers;

/// <summary>
/// Defines a handler for a request
/// </summary>
public interface IRequestHandler<in TRequest, TResponse>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a handler for a request with a void response.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
public interface IRequestHandler<in TRequest>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    Task Handle(TRequest request, CancellationToken cancellationToken);
}