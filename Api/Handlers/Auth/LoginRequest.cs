using System.Security;

namespace Api.Handlers.Auth;

public record LoginRequest(string Username, SecureString Password) : IRequest;
