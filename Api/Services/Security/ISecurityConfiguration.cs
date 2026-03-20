using Microsoft.IdentityModel.Tokens;
namespace Api.Services.Security;
public interface ISecurityConfiguration
{
    SymmetricSecurityKey SecurityKey { get;  }
    int Expiration { get;  }
}