namespace Api.Services.Security;

public interface ITokenRevocationService : IService
{
    void Revoke(string jti, DateTimeOffset expiresAtUtc);
    bool IsRevoked(string jti);
}
