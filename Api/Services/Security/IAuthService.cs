using System.Security;

namespace Api.Services.Security;

public interface IAuthService : IService
{
    bool IsCredentialValid(string username, SecureString password);
}