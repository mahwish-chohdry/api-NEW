

using System.Security.Claims;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.ServiceLayer
{
    public interface IAuth
    {
        LoginDTO Login(UserDTO user);
        string GetToken(string email);
        UserResponseDTO ResetPassword(UserDTO user);
        User ForgotPassword(string email);
        bool ValidatePassword(string email, string currentPassword);
        string GenerateRefreshToken(string email);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Refreshtokens GetRefreshtokens(string refreshTokenFromUser);
        LoginDTO SuperAdminLogin(UserDTO UserDTO);

    }
}
