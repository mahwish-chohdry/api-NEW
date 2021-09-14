using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.ServiceLayer;
using Xavor.SD.WebAPI.Helper;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;
        private IAuth _auth;
        private readonly IAccountCreation _accountCreation;
        private readonly IUserService _userService;

        public AuthorizationController(ILogger<AuthorizationController> logger, IAuth auth, IAccountCreation accountCreation, IUserService userService)
        {
            _logger = logger;
            _auth = auth;
            _accountCreation = accountCreation;
            _userService = userService;
        }

        /// <summary>
        /// signs in the user.
        /// </summary>
        /// <param name="user">user dto.</param>
        /// <returns>returns user data.</returns>
        [HttpPost]
        public ActionResult SignIn(UserDTO user)
        {
            try
            {

                _logger.LogInformation("Loggin in...");

                var UserResponse = _auth.Login(user);

                ResponseDTO response = new ResponseDTO();
                if (UserResponse != null)
                {
                    var token = _auth.GetToken(user.email);
                    var refToken = _auth.GenerateRefreshToken(user.email);
                    Response.Headers.Add("X-API-Key", token.ToString());
                    Response.Headers.Add("RefreshToken", refToken);
                    UserResponse.Token = token.ToString();
                    UserResponse.userPermission = _userService.GetUserPermission( UserResponse.CustomerId, UserResponse.UserId);
                    response.StatusCode = "Success";
                    response.Message = "Logged In Successfully";
                    response.Data = UserResponse;

                }
                else
                {

                    response.StatusCode = "Warning";
                    response.Message = "Username or password is incorrect.";
                    response.Data = UserResponse;
                }


                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has occured: ");
                if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
                return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
                //return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = "An exception has occured", Data = null });
            }


        }
        /// <summary>
        /// generates a new access token by validating expired access token and refresh token.
        /// </summary>
        /// <param name="tokensFromUser">token model.</param>
        /// <returns>returns new access and refresh token.</returns>
        [HttpPost]
        public IActionResult RefreshAccessToken(Tokens tokensFromUser)
        {
            try
            {
                var principal = _auth.GetPrincipalFromExpiredToken(tokensFromUser.accessToken);
                var existingRefreshToken = _auth.GetRefreshtokens(tokensFromUser.refreshToken);
                string email = principal.Identity.Name;
                var token = _auth.GetToken(email);
                var refToken = _auth.GenerateRefreshToken(email);
                Response.Headers.Add("X-API-Key", token.ToString());
                Response.Headers.Add("RefreshToken", refToken);
                ResponseDTO response = null;
                response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "New Access And Refresh Token Generated Successfully",
                    Data = null,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has occured: ");
                if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
                return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
                //return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = "An exception has occured", Data = null });
            }
        }

        /// <summary>
        /// resets the password.
        /// </summary>
        /// <param name="user">user dto.</param>
        /// <returns>returns status.</returns>
        [HttpPost]
        public ActionResult ResetPassword(UserDTO user)
        {

            try
            {
                if (user.newPassword.Length < 6)
                {
                    throw new ResponseException(new ResponseDTO
                    {
                        StatusCode = "Warning",
                        Message = "Invalid New Password",
                        Data = null
                    });
                }
                var response = new ResponseDTO();
                var res = _auth.ResetPassword(user);
                if (res.message == "Invalid Email Or Password")
                {


                    response = new ResponseDTO()
                    {
                        StatusCode = "Warning",
                        Message = "Current password is not correct.",
                    };

                    Ok(response);
                }
                else
                {
                    response = new ResponseDTO()
                    {
                        StatusCode = "Success",
                        Message = res.message,
                    };
                }


                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has occured: ");
                if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
                return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
                //return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = "An exception has occured", Data = null });
            }
        }

        /// <summary>
        /// forgot password.
        /// </summary>
        /// <param name="user">user dto.</param>
        /// <returns>returns status.</returns>
        [HttpPost]
        public ActionResult ForgotPassword(UserDTO user)
        {
            try
            {

                var res = _auth.ForgotPassword(user.email);
                ResponseDTO response = new ResponseDTO();
                if (res != null)
                {
                    _accountCreation.SendForgottenPasswordEmail(res.Username, user.email, res.Password.Decrypt(), "en");

                    response.StatusCode = "Success";
                    response.Message = "Email Sent Successfully";

                }
                else
                {

                    response.StatusCode = "Warning";
                    response.Message = "Your email address does not exist.";
                }


                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has occured: ");
                if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
                return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
                //return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = "An exception has occured", Data = null });
            }
        }

        [HttpPost("{email}/{lang}")]
        public ActionResult ForgotPassword(string email, string lang = "en")
        {
            try
            {

                var res = _auth.ForgotPassword(email);
                ResponseDTO response = new ResponseDTO();
                if (res != null)
                {
                    _accountCreation.SendForgottenPasswordEmail(res.Username, email, res.Password.Decrypt(), lang);

                    response.StatusCode = "Success";
                    response.Message = "Email Sent Successfully";

                }
                else
                {

                    response.StatusCode = "Warning";
                    response.Message = "Your email address does not exist.";
                }


                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has occured: ");
                if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
                return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
                //return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = "An exception has occured", Data = null });
            }
        }
    }
}
