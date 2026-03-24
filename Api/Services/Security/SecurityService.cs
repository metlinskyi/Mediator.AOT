using System.Security;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services.Security;

public class SecurityService(
    ISecurityConfiguration configuration
    ) : ISecurityService
{

    public byte[] GetSecurityKey()
    {
        return configuration.SecurityKey.Key;
    }

    public SecureString ConvertToSecureString(IEnumerable<char> password)
    {
        var securePassword = new SecureString();
        foreach (char c in password)
        {
            securePassword.AppendChar(c);
        }
        securePassword.MakeReadOnly();
        return securePassword;
    }

    public SecurityTokenDescriptor CreateTokenDescriptor(string username)
    {
        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] 
            { 
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            }),
            Expires = DateTime.UtcNow.AddHours(12),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(GetSecurityKey()), SecurityAlgorithms.HmacSha256Signature)
        };
    }

}

