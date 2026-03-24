
using System.Text;
using Api.Handlers;
using Api.Services;
using Api.Services.Security;
using Mediator.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateSlimBuilder(args);
var config = builder.Configuration;
var services = builder.Services;
var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["Jwt:Key"]!));

services.AddMediator(mediator =>
        {
            mediator.RegisterHandlers();
        })
        .AddOpenApi(options =>
        {
            options.AddDocumentTransformer(async (document, context, ct) =>
            {
                 
                await context.ApplicationServices
                    .GetRequiredService<IMediatorRegister>()
                    .AddSchema(async (options, infos) =>
                    {   
                        var path = new Microsoft.OpenApi.OpenApiPathItem();
                        path.AddOperation(HttpMethod.Post, new Microsoft.OpenApi.OpenApiOperation
                        {
                        });
                        document.Paths.TryAdd(options.SendPattern, path);

                        foreach (var info in infos)
                        {        
                            var request = await context.GetOrCreateSchemaAsync(info.RequestType, null, ct);
                            request.DependentRequired = new Dictionary<string, HashSet<string>>{
                                {info.ResponseType.Name, new HashSet<string>()}
                            };   
                            var response = await context.GetOrCreateSchemaAsync(info.ResponseType, null, ct);
                            document.AddComponent(info.RequestType.Name, request); 
                            document.AddComponent(info.ResponseType.Name, response); 
                        }
                    });
            });
        })
        .AddSingleton<ISecurityConfiguration>(new SecurityConfiguration(
            key,
            int.TryParse(config["Jwt:Expiration"], out var expiration) ? expiration : 0
        ))
        .AddJsonOptions()
        .AddServices()
        .AddCors()
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
services.AddAuthorization();

var app = builder.Build();

app
    .UseCors()
    .UseAuthentication()
    .UseAuthorization();
    
app.MapMediator();
app.MapOpenApi();
app.Run();
