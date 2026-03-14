global using Api.Middleware;

using Api.Handlers;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddMediator(Register.MediatorHandlers);

var app = builder.Build();
app.MapMediator(mediator =>
{

});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
