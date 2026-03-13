namespace Api.Handlers.Hello;

internal class HelloHandler : IRequestHandler<HelloRequest, HelloResponse>
{
    public Task<HelloResponse> Handle(HelloRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HelloResponse("Hello World!"));
    }
}