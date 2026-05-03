using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Api.Handlers;
using Api.Services;
using Api.Services.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Happy.Endpoint.Middleware;

var builder = WebApplication.CreateSlimBuilder(args);
var config = builder.Configuration;
var services = builder.Services;
var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["Jwt:Key"]!));

services.AddHappyEndpoints(mediator =>
        {
            mediator.EndpointsCollection();
        })
        .AddOpenApi(options =>
        {
            options.AddDocumentTransformer(async (document, context, ct) =>
            {
                await context.ApplicationServices
                    .GetRequiredService<IHappy.Middleware>()
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
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
                        ?? context.Principal?.FindFirst("jti")?.Value;

                    if (string.IsNullOrWhiteSpace(jti))
                        return Task.CompletedTask;

                    var revocationService = context.HttpContext.RequestServices
                        .GetRequiredService<ITokenRevocationService>();

                    if (revocationService.IsRevoked(jti))
                        context.Fail("Token has been revoked.");

                    return Task.CompletedTask;
                }
            };
        });
services.AddAuthorization();

var app = builder.Build();

app
    .UseCors()
    .UseAuthentication()
    .UseAuthorization();
    
app.MapHappyEndpoints();
app.MapOpenApi();
app.Run();
