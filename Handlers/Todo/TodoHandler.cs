using Api.Middleware;

namespace  Api.Handlers;
/// <summary>
/// A sample handler for the Todo API. 
/// </summary>
internal class TodoHandler : IRequestHandler<TodoRequest, Todo>
{
    public Task<Todo> Handle(TodoRequest request, CancellationToken cancellationToken)
    {
        Todo[] sampleTodos =
        [
            new(1, "Walk the dog"),
            new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
            new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
            new(4, "Clean the bathroom"),
            new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
        ];

        return Task.FromResult(sampleTodos.FirstOrDefault(a => a.Id == request.Id) ?? throw new KeyNotFoundException($"Todo with id {request.Id} not found"));
    }
}