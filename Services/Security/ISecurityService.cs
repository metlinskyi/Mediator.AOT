using System.Security;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services.Security;

public interface ISecurityService : IService
{
    byte[] GetSecurityKey();
    SecureString ConvertToSecureString(IEnumerable<char> password);
    SecurityTokenDescriptor CreateTokenDescriptor(string username);
}

