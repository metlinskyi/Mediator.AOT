namespace Api.Handlers.Auth;

public record LoginRequest(string Username, string Password) : IRequest;
