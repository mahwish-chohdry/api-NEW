using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.WebAPI.ViewContracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Xavor.SD.ServiceLayer
{
    public class Auth : IAuth
    {
        private byte[] _key = Encoding.UTF8.GetBytes("09123678901236480");
        string ConnectionString;
        private IUserBL _user;
        private IUserroleBL _userRole;
        private IRoleBL _role;
        private ICustomerBL _customerBL;
        private readonly IRuleEngineBL _ruleEngineBL;
        private readonly IRefreshtokensBL _refreshTokensBL;

        public Auth(IUserBL user, IUserroleBL userRole, IRoleBL role, ICustomerBL customerBL, IRuleEngineBL ruleEngineBL, IRefreshtokensBL refreshTokensBL)
        {
            _user = user;
            _userRole = userRole;
            _role = role;
            _customerBL = customerBL;
            _ruleEngineBL = ruleEngineBL;
            _refreshTokensBL = refreshTokensBL;
        }

        public User ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "Invalid user",
                    Data = null
                });
            }
            var res = _user.ForgotPassword(email);
            return res;

        }

        public string GetToken(string Email)
        {
            var TokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name,Email)

                        }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256),


            };
            var TokenHandler = new JwtSecurityTokenHandler();
            var SecurityToken = TokenHandler.CreateToken(TokenDescription);
            var token = TokenHandler.WriteToken(SecurityToken);
            return token;
        }

        public LoginDTO Login(UserDTO UserDTO)
        {
            if (string.IsNullOrEmpty(UserDTO.email) || string.IsNullOrEmpty(UserDTO.password))
            {
                return null;
            }
            var encryptedPassword = UserDTO.password.Encrypt();
            var user = _user.GetUserByEmailandPassword(UserDTO.email.ToLower(), encryptedPassword);
            if (user == null)
            {
                return null;
            }

            var role = (from u in _user.QueryUsers()
                        join ur in _userRole.QueryUserrole() on u.Id equals ur.UserId
                        join r in _role.QueryRole() on ur.RoleId equals r.Id
                        where u.Id == user.Id
                        select r.Role1).FirstOrDefault();
            var customer = _customerBL.GetCustomer().Where(x => x.Id == user.CustomerId).FirstOrDefault();
            if (customer == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "Invalid user",
                    Data = null
                });
            }

            user.DeviceType = UserDTO.deviceType;
            user.DeviceIdentifier = UserDTO.deviceIdentifier;
            _user.UpdateUser(user);

            var login = new LoginDTO
            {
                Customer_Id = customer.Id,
                CustomerId = customer.CustomerId,
                Customertype = customer.CustomerType,
                Role = role,
                UserName = user.Username,
                UserId = user.UserId,
                EmailAddress = user.EmailAddress,
                ProfileImage = user.ProfilePicture,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
            };

            login.Ruleengine = _ruleEngineBL.GetRuleEngineByCustomerId(customer.Id);

            return login;
        }
        public LoginDTO SuperAdminLogin(UserDTO UserDTO)
        {
            if (string.IsNullOrEmpty(UserDTO.email) || string.IsNullOrEmpty(UserDTO.password))
            {
                return null;
            }
            var encryptedPassword = UserDTO.password.Encrypt();
            var user = _user.GetUserByEmailandPassword(UserDTO.email.ToLower(), encryptedPassword);
            if (user == null)
            {
                return null;
            }

            var role = (from u in _user.QueryUsers()
                        join ur in _userRole.QueryUserrole() on u.Id equals ur.UserId
                        join r in _role.QueryRole() on ur.RoleId equals r.Id
                        where u.Id == user.Id && r.Role1 == "SuperAdmin"
                        select r).FirstOrDefault();
            if (role == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "User is not Super Admin",
                    Data = null
                });
            }
            var customer = _customerBL.GetCustomer().Where(x => x.Id == user.CustomerId).FirstOrDefault();
            if (customer == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "Invalid Customer of Super Admin",
                    Data = null
                });
            }

            user.DeviceType = UserDTO.deviceType;
            user.DeviceIdentifier = UserDTO.deviceIdentifier;
            _user.UpdateUser(user);

            var login = new LoginDTO
            {
                CustomerId = customer.CustomerId,
                UserName = user.Username,
                UserId = user.UserId,
                ProfileImage = user.ProfilePicture,
            };

            return login;
        }
        public string GenerateRefreshToken(string email)
        {
            Refreshtokens rToken = new Refreshtokens();
            rToken.UserId = _user.GetUserDbIdByEmail(email);


            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {

                rng.GetBytes(randomNumber);
                rToken.RefreshToken = Convert.ToBase64String(randomNumber);
                _refreshTokensBL.AddOrUpdateRefreshToken(rToken);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_key),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "Invalid Access Token",
                    Data = null
                });
            }
            return principal;
        }
        public UserResponseDTO ResetPassword(UserDTO user)
        {

            UserResponseDTO res = new UserResponseDTO();

            var currentEncryptedPassword = user.currentPassword.Encrypt();
            if (ValidatePassword(user.email, currentEncryptedPassword))
            {
                var encryptedPassword = user.newPassword.Encrypt();
                if (currentEncryptedPassword == encryptedPassword)
                {
                    throw new ResponseException(new ResponseDTO
                    {
                        StatusCode = "Warning",
                        Message = "New password and old password are same. Please change your new password.",
                        Data = null
                    });
                }
                res = _user.ResetPassword(user.email, encryptedPassword);
                return res;
            }
            else
            {
                res.user_id = user.user_id;
                res.success = false;
                res.message = "Invalid Email Or Password";
                res.statusCode = 404;


                return res;

            }

        }

        public bool ValidatePassword(string email, string currentPassword)
        {
            return _user.ValidatePassword(email, currentPassword);
        }

        public Refreshtokens GetRefreshtokens(string refreshTokenFromUser)
        {
            var existingRefreshToken = _refreshTokensBL.GetRefreshtokens(refreshTokenFromUser);
            if (existingRefreshToken == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "Invalid Refresh Token",
                    Data = null
                });
            }
            else if (existingRefreshToken.RefreshToken != refreshTokenFromUser)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "Invalid Refresh Token",
                    Data = null
                });
            }
            return existingRefreshToken;
        }
    }
}
