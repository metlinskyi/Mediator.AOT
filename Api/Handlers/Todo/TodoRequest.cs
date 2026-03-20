namespace Api.Handlers.Todo;

public record TodoRequest(
    int Id) : IRequest;