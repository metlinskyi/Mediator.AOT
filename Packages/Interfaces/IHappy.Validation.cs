namespace Happy.Endpoint.Interfaces;

public partial interface IHappy
{
        /// <summary>
    /// Defines a handler for a request
    /// </summary>
    public interface Validation<in TRequest> : IHappy
    {
        /// <summary>
        /// Handles a request
        /// </summary>
        Task<bool> Validate(TRequest request, CancellationToken cancellationToken);
    }
}