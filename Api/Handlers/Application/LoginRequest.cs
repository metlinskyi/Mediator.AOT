using System.Security;

namespace Api.Handlers.Application;

public record LoginRequest(string Username, SecureString Password);
