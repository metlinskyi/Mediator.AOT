using Microsoft.IdentityModel.Tokens;
namespace Api.Services.Security;
internal record SecurityConfiguration(
    SymmetricSecurityKey SecurityKey,
    int Expiration) : ISecurityConfiguration;
