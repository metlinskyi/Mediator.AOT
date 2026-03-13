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

var mediator = new MediatorService();
mediator.Register(new HelloHandler(), AppJsonSerializerContext.Default.HelloRequest, AppJsonSerializerContext.Default.HelloResponse);
mediator.Register(new TodoHandler(), AppJsonSerializerContext.Default.TodoRequest, AppJsonSerializerContext.Default.Todo);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/mediator/send/{className}", async (string className, HttpContext ctx, CancellationToken ct) =>
    await mediator.Send(className, ctx, ct));

app.Run();
