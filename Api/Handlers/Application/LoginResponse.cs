using System.Security;

namespace Api.Handlers.Application;

public record LoginResponse(SecureString Token);