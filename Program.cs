global using Api.Middleware;

using Api.Handlers;


var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddMediator(Register.MediatorHandlers);
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(async (document, context, ct) =>
    {
        var mediator = context.ApplicationServices.GetRequiredService<IMediator>();
        foreach (var requestType in mediator.RequestTypes
            .Union(mediator.ResponseTypes)
            .OrderBy(t => t.Name))
        {
            var schema = await context.GetOrCreateSchemaAsync(requestType, null, ct);
            document.AddComponent(requestType.Name, schema);
        }
    });
});


var app = builder.Build();
app.MapMediator();
app.MapOpenApi();
app.Run();
