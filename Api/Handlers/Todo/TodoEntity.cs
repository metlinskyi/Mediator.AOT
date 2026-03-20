namespace Api.Handlers.Todo;

public record TodoEntity(
    int Id, 
    string? Title, 
    DateOnly? DueBy = null, 
    bool IsComplete = false) : IResponse;