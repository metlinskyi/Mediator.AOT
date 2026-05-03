using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Happy.Endpoint.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Happy.Endpoint.Interfaces;

namespace Tests;

public class MediatorServiceTests
{
    [Test]
    public async Task Send_WithRegisteredHandler_ReturnsSerializedResponse()
    {
        var services = new ServiceCollection();
        var mediator = new MiddlewareService(services);
        mediator.Register<PingRequest, PingResponse, PingHandler>(TestJsonSerializerContext.Default);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var httpContext = CreateHttpContext(scope.ServiceProvider, JsonSerializer.SerializeToUtf8Bytes(
            new PingRequest("hi"),
            TestJsonSerializerContext.Default.PingRequest));

        await mediator.Send((HttpMethod.Post, "PingRequest".ToUpper()), httpContext, CancellationToken.None);

        httpContext.Response.Body.Position = 0;
        var response = await JsonSerializer.DeserializeAsync(
            httpContext.Response.Body,
            TestJsonSerializerContext.Default.PingResponse,
            CancellationToken.None);

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Message, Is.EqualTo("Echo:hi"));
    }

    [Test]
    public async Task Send_ClassNameLookup_IsCaseInsensitive()
    {
        var services = new ServiceCollection();
        var mediator = new MiddlewareService(services);
        mediator.Register<PingRequest, PingResponse, PingHandler>(TestJsonSerializerContext.Default);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var httpContext = CreateHttpContext(scope.ServiceProvider, JsonSerializer.SerializeToUtf8Bytes(
            new PingRequest("world"),
            TestJsonSerializerContext.Default.PingRequest));

        await mediator.Send((HttpMethod.Post, "pingrequest".ToUpper()), httpContext, CancellationToken.None);

        httpContext.Response.Body.Position = 0;
        var response = await JsonSerializer.DeserializeAsync(
            httpContext.Response.Body,
            TestJsonSerializerContext.Default.PingResponse,
            CancellationToken.None);

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Message, Is.EqualTo("Echo:world"));
    }

    [Test]
    public async Task Send_WithUnknownClassName_Returns404AndMessage()
    {
        var services = new ServiceCollection();
        var mediator = new MiddlewareService(services);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var httpContext = CreateHttpContext(scope.ServiceProvider, "{}"u8.ToArray());

        await mediator.Send((HttpMethod.Post, "DoesNotExist".ToUpper()), httpContext, CancellationToken.None);

        httpContext.Response.Body.Position = 0;
        using var reader = new StreamReader(httpContext.Response.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(body, Is.EqualTo("No handler registered for 'DOESNOTEXIST'."));
    }

    [Test]
    public void Register_WithoutTypeMetadata_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();
        var mediator = new MiddlewareService(services);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            mediator.Register<PingRequest, PingResponse, PingHandler>(
                MissingRequestJsonSerializerContext.Default));

        Assert.That(ex!.Message, Does.Contain(typeof(PingRequest).FullName));
    }

    [Test]
    public async Task AddSchema_EnumeratesRegisteredHandlerInfos()
    {
        var services = new ServiceCollection();
        var mediator = new MiddlewareService(services);
        mediator.Register<PingRequest, PingResponse, PingHandler>(TestJsonSerializerContext.Default);

        var infos = new List<EndpointInfo>();
        await mediator.AddSchema((opt, info) =>
        {
            if(info.First() is EndpointInfo endpointInfo)
            {
                infos.Add(endpointInfo);
            }
            return Task.CompletedTask;
        });

        Assert.That(infos, Has.Count.EqualTo(1));
        Assert.That(infos[0].RequestType, Is.EqualTo(typeof(PingRequest)));
        Assert.That(infos[0].ResponseType, Is.EqualTo(typeof(PingResponse)));
    }

    [Test]
    public async Task Send_WithAuthorizeHandler_AnonymousUser_Returns401()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization();

        var mediator = new MiddlewareService(services);
        mediator.Register<PingRequest, PingResponse, ProtectedPingHandler>(TestJsonSerializerContext.Default);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var httpContext = CreateHttpContext(scope.ServiceProvider, JsonSerializer.SerializeToUtf8Bytes(
            new PingRequest("secret"),
            TestJsonSerializerContext.Default.PingRequest));

        await mediator.Send((HttpMethod.Post, "PINGREQUEST"), httpContext, CancellationToken.None);

        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
    }

    [Test]
    public async Task Send_WithRoleProtectedHandler_MissingRole_Returns403()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization();

        var mediator = new MiddlewareService(services);
        mediator.Register<PingRequest, PingResponse, AdminOnlyPingHandler>(TestJsonSerializerContext.Default);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var httpContext = CreateHttpContext(scope.ServiceProvider, JsonSerializer.SerializeToUtf8Bytes(
            new PingRequest("secret"),
            TestJsonSerializerContext.Default.PingRequest));
        httpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, "user")],
                authenticationType: "TestAuth"));

        await mediator.Send((HttpMethod.Post, "PINGREQUEST"), httpContext, CancellationToken.None);

        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
    }

    private static DefaultHttpContext CreateHttpContext(IServiceProvider requestServices, byte[] requestBody)
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = requestServices
        };

        httpContext.Request.ContentType = "application/json";
        httpContext.Request.Body = new MemoryStream(requestBody);
        httpContext.Response.Body = new MemoryStream();
        return httpContext;
    }

    public sealed record PingRequest(string Message);

    public sealed record PingResponse(string Message);

    private sealed class PingHandler : IHappy.Endpoint<PingRequest, PingResponse>
    {
        public Task<PingResponse> Handle(PingRequest request, CancellationToken cancellationToken)
            => Task.FromResult(new PingResponse($"Echo:{request.Message}"));
    }

    [Authorize]
    private sealed class ProtectedPingHandler : IHappy.Endpoint<PingRequest, PingResponse>
    {
        public Task<PingResponse> Handle(PingRequest request, CancellationToken cancellationToken)
            => Task.FromResult(new PingResponse($"Protected:{request.Message}"));
    }

    [Authorize(Roles = "Admin")]
    private sealed class AdminOnlyPingHandler : IHappy.Endpoint<PingRequest, PingResponse>
    {
        public Task<PingResponse> Handle(PingRequest request, CancellationToken cancellationToken)
            => Task.FromResult(new PingResponse($"Admin:{request.Message}"));
    }
}

[JsonSerializable(typeof(MediatorServiceTests.PingRequest))]
[JsonSerializable(typeof(MediatorServiceTests.PingResponse))]
internal partial class TestJsonSerializerContext : JsonSerializerContext
{
}

[JsonSerializable(typeof(MediatorServiceTests.PingResponse))]
internal partial class MissingRequestJsonSerializerContext : JsonSerializerContext
{
}
