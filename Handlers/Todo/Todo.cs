namespace Api.Handlers.Todo;

public record Todo(
    int Id, 
    string? Title, 
    DateOnly? DueBy = null, 
    bool IsComplete = false) : IResponse;