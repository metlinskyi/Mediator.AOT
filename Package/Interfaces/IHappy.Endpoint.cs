namespace Happy.Endpoint.Interfaces;

public partial interface IHappy
{
    /// <summary>
    /// Defines a handler for a request
    /// </summary>
    public interface Endpoint<in TRequest, TResponse> : IHappy
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
    public interface Endpoint<in TRequest> : IHappy
    {
        /// <summary>
        /// Handles a request
        /// </summary>
        Task Handle(TRequest request, CancellationToken cancellationToken);
    }
}

