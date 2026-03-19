namespace Api.Handlers.Auth;

public record LoginResponse(string Token) : IResponse;