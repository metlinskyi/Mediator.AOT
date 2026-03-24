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
```bash
docker build -t api -f ".dockerfile" .
docker run -p 8080:8080 api
```

## Mediator Endpoint

`MapMediator()` uses this default route pattern:

- `POST /mediator/send/{className}`

`{className}` is matched against registered request type names (case-insensitive).

If no handler is registered, the API returns:
- `404 Not Found`

If request body deserialization returns `null`, the API returns:
- `400 Bad Request`

## Registered Sample Handlers

Handlers are wired in `Api/Handlers/Register.cs`:
- `LoginRequest -> LoginResponse` (`LoginHandler`)
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

## Testing

Run tests:

```bash
dotnet test Tests/Tests.csproj
```

Current status:
- `1` test passing.

## Notes

- The package targets `net10.0` and currently includes:
  - `<FrameworkReference Include="Microsoft.AspNetCore.App" />`
- API JSON serialization uses source-generated metadata via `AppJsonSerializerContext` and a custom `SecureString` converter.
