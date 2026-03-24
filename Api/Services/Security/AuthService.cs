using System.Security;

namespace Api.Services.Security;

public class AuthService() : IAuthService
{
    public bool IsCredentialValid(string username, SecureString password)
    {
        // In a real application, you would validate the credentials against a user store (e.g., database)
        // Here, we simply check if the username is "JohnDoe" and the password is "P@ssw0rd" for demonstration purposes
        var validUsername = "JohnDoe";
        var validPassword = "P@ssw0rd";

        var passwordBSTR = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(password);
        var passwordString = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(passwordBSTR);

        return username == validUsername && passwordString == validPassword;
    }
}