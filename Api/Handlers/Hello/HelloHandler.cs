namespace Api.Handlers.Hello;

internal class HelloHandler(ILogger<HelloHandler> logger) : IRequestHandler<HelloRequest, HelloResponse>
{
    public Task<HelloResponse> Handle(HelloRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling HelloRequest");
        return Task.FromResult(new HelloResponse("Hello World!"));
    }
}