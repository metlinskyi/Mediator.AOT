using Api.Middleware;

namespace Api.Handlers;

public record TodoRequest(
    int Id) : IRequest;