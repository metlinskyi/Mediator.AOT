# Mediator.AOT

A lightweight, AOT-friendly mediator pattern implementation for ASP.NET Core minimal APIs.

This repository contains:
- `Package/`: the reusable `Mediator.AOT` library.
- `Api/`: a sample ASP.NET Core app that registers handlers and exposes a mediator endpoint.
- `Tests/`: NUnit tests for the library/solution.

## Features

- Handler registration by request/response type.
- Runtime dispatch by request class name (`{className}`).
- AOT-oriented JSON serialization via source-generated `JsonSerializerContext`.
- Minimal API integration with simple extension methods:
  - `services.AddMediator(...)`
  - `app.MapMediator(...)`
- OpenAPI schema enrichment for registered mediator request/response models.
- Handler-level authorization via ASP.NET Core `[Authorize]` and `[AllowAnonymous]` attributes.

## Requirements

- .NET SDK `10.0`

## Project Structure

```text
Api/        Sample web app (minimal API + JWT + OpenAPI + handler registrations)
Package/    Mediator.AOT library
Tests/      NUnit test project
```

## Quick Start

Build the API:

```bash
dotnet build Api/Api.csproj
```

Run the API:

```bash
dotnet run --project Api/Api.csproj
```

By default (see `Api/Properties/launchSettings.json`), the app listens on:
- `http://localhost:8080`

## Docker

Build and run the sample API with the Alpine-based container image:

```bash
docker build -t api -f ".dockerfile" .
docker run -p 8080:8080 api
```

The Docker image uses:
- `mcr.microsoft.com/dotnet/sdk:10.0-alpine` for build
- `mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine` for runtime
- `linux-musl-x64` publish target for Alpine compatibility

## Mediator Endpoint

`MapMediator()` uses this default route pattern:

- `POST /mediator/send/{className}`
- `PUT /mediator/send/{className}`

`{className}` is matched against registered request type names (case-insensitive).

If no handler is registered, the API returns:
- `404 Not Found`

If request body deserialization returns `null`, the API returns:
- `400 Bad Request`

If a handler is decorated with `[Authorize]`, mediator evaluates authorization before handler execution and returns:
- `401 Unauthorized` for anonymous requests
- `403 Forbidden` for authenticated requests that do not satisfy the policy or role requirements

## Registered Sample Handlers

Handlers are wired in `Api/Handlers/Register.cs`:
- `LoginRequest -> LoginResponse` (`LoginHandler`)
- `LogoutRequest` (`LogoutHandler`, protected with `[Authorize]`)
- `HelloRequest -> HelloResponse` (`HelloHandler`)
- `TodoRequest -> TodoEntity` (`TodoHandler`)

### Example: Hello

Request:

```http
POST /mediator/send/HelloRequest
Content-Type: application/json

{}
```

Response:

```json
{
  "message": "Hello World!"
}
```

### Example: Todo

Request:

```http
POST /mediator/send/TodoRequest
Content-Type: application/json

{
  "id": 2
}
```

Response (sample):

```json
{
  "id": 2,
  "title": "Do the dishes",
  "dueBy": "2026-03-20",
  "isComplete": false
}
```

### Example: Login

Request:

```http
POST /mediator/send/LoginRequest
Content-Type: application/json

{
  "username": "demo",
  "password": "secret"
}
```

Response shape:

```json
{
  "token": "..."
}
```

### Example: Logout

Request:

```http
PUT /mediator/send/LogoutRequest
Authorization: Bearer <jwt>
Content-Type: application/json

{}
```

Response:

```http
204 No Content
```

## Using the Library in Your App

1. Reference `Mediator.AOT` (`Package/Mediator.csproj`) from your API project.
2. Register your handlers during service configuration:

```csharp
services.AddMediator(mediator =>
{
    mediator.Register<MyRequest, MyResponse, MyHandler>(MyJsonSerializerContext.Default);
});
```

3. Map mediator route(s):

```csharp
app.MapMediator();
```

4. Send `POST` requests to:

```text
/mediator/send/MyRequest
```

For handlers implementing `IRequestHandler<TRequest>` without a response body, send `PUT` requests instead.

To protect a handler, apply standard ASP.NET Core authorization attributes on the handler type or its `Handle` method:

```csharp
using Microsoft.AspNetCore.Authorization;

[Authorize]
public sealed class LogoutHandler : IRequestHandler<LogoutRequest>
{
  public Task Handle(LogoutRequest request, CancellationToken cancellationToken)
    => Task.CompletedTask;
}
```

## Notes

- The package targets `net10.0` and currently includes:
  - `<FrameworkReference Include="Microsoft.AspNetCore.App" />`
- API JSON serialization uses source-generated metadata via `AppJsonSerializerContext` and a custom `SecureString` converter.
- Sample JWT settings live in `Api/appsettings.json` under the `Jwt` section.
