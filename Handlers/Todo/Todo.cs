using Api.Middleware;

namespace Api.Handlers;

public record Todo(
    int Id, 
    string? Title, 
    DateOnly? DueBy = null, 
    bool IsComplete = false) : IResponse;