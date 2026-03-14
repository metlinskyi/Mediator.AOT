namespace Api.Middleware.Schema;

public class MediatorSchemaHandler : IRequestHandler<MediatorSchemaRequest, MediatorSchema>
{
    public Task<MediatorSchema> Handle(MediatorSchemaRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new MediatorSchema
        {


        });
    }
}