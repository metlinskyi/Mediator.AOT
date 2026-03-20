using System.Security;

namespace Api.Handlers.Auth;

public record LoginResponse(SecureString Token);