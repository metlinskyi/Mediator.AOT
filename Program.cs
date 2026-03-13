global using Api.Middleware;

using Api.Handlers;
using Api.Handlers.Hello;
using Api.Handlers.Todo;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddOpenApi();

builder.Services.AddScoped<HelloHandler>();
builder.Services.AddScoped<TodoHandler>();
builder.Services.AddSingleton<IMediator, MediatorService>();

var app = builder.Build();

var mediator = app.Services.GetRequiredService<IMediator>();
mediator.Register<HelloRequest, HelloResponse, HelloHandler>(AppJsonSerializerContext.Default.HelloRequest, AppJsonSerializerContext.Default.HelloResponse);
mediator.Register<TodoRequest, Todo, TodoHandler>(AppJsonSerializerContext.Default.TodoRequest, AppJsonSerializerContext.Default.Todo);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/mediator/send/{className}", async (string className, HttpContext ctx, CancellationToken ct) =>
    await mediator.Send(className, ctx, ct));

app.Run();
