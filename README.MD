# Happy.Endpoint.AOT

Happy.Endpoint.AOT is a lightweight .NET library that implements the Mediator pattern, designed with Ahead-of-Time (AOT) compilation in mind. It provides a clean architecture for handling requests and responses through mediators, decoupling senders and receivers.

## Features

- Mediator pattern implementation for .NET
- Full support for Ahead-of-Time (AOT) compilation
- Built-in authentication and security services
- Middleware pipeline support
- Flexible endpoint mapping
- Comprehensive unit tests

## Project Structure

- **Api/**: An example ASP.NET Core API project demonstrating the mediator usage, including login/logout endpoints and security services.
- **Packages/**: The core library containing interfaces (e.g., IHappy.Endpoint, IHappy.Middleware) and implementations for mediators, middleware, and endpoint handling.
- **Tests/**: Unit tests for validating the library functionality.

## Getting Started

1. Clone the repository.
2. Build the solution: `dotnet build`
3. Run tests: `dotnet test`

## Usage

Refer to the Api project for example implementations of mediators and endpoints.

### Docker
```bash
docker build -t api -f ".dockerfile" .
docker run -p 8080:8080 api
```
