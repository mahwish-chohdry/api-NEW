using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.ServiceLayer;
using Xavor.SD.WebAPI.Helper;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAccountCreation _accountCreation;
        private readonly IDeviceService _deviceService;
        private readonly ILogger<UserController> _logger;


        public UserController(IUserService userService, IAccountCreation accountCreation, IDeviceService deviceService, ILogger<UserController> logger)
        {
            _userService = userService;
            _accountCreation = accountCreation;
            _deviceService = deviceService;
            _logger = logger;

        }
        /// <summary>
        /// update existing users username and profile picture in database.
        /// </summary>
        /// <param name="user">user dto.</param>
        /// <param name="customerId">customer id.</param>
        /// <param name="UserId">user id.</param>
        /// <returns>returns status.</returns>
        [HttpPost("{CustomerId}/{UserId}")]
        public IActionResult EditProfile([FromBody]EditProfileDTO user, string customerId, string UserId)
        {
            try
            {
                ResponseDTO response = null;
                if (_userService.UpdateUser(customerId, UserId, user))
                {
                    response = new ResponseDTO()
                    {
                        StatusCode = "Success",
                        Message = "Successfully Updated User",
                        Data = null,
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
        /// sends feedback email to superadmins.
        /// </summary>
        /// <param name="emailDTO">email dto.</param>
        /// <returns>returns status.</returns>
        [HttpPost]
        public IActionResult Feedback([FromBody]EmailDTO emailDTO)
        {
            try
            {
                ResponseDTO response = null;
                var emailRecipientsList = _userService.GetEmailRecipients(emailDTO);
                _accountCreation.SendFeedbackEmails(emailDTO, emailRecipientsList);
                response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Successfully Sent Feedback To Super Admins",
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

        [AllowAnonymous]
        [HttpPost("{customerId}/{userId}")]
        public IActionResult DeleteUser(string customerId, string userId)
        {
            try
            {

                if (_userService.DeleteUserById(customerId, userId))
                {
                    var response = new ResponseDTO()
                    {
                        Message = "User has been deleted successfully.",
                        StatusCode = "Success",
                        Data = null
                    };
                    return Ok(response);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has occured: ");
                if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
                return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
                //return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = "An exception has occured", Data = null });
            }
        }

        [HttpGet()]
        public ActionResult GetUser(string customerId)
        {
            try
            {
                var users = _userService.GetUser(customerId);

                var usersList = users.Select(x=>new {x.Id,x.IdentityProvider,x.DeviceIdentifier,x.DomainUserName,x.Refreshtokens, x.CreatedBy,x.CreatedDate,x.CustomerId,x.EmailAddress,x.FirstName,x.IsActive ,x.IsDeleted,x.LastName,x.ModifiedBy,x.ModifiedDate,x.ParentId,x.ProfilePicture,x.UserId,x.Username}).ToList();
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched users.",
                    Data = users,
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

        [HttpPost()]
        public ActionResult UpdateUser([FromBody] User user)
        {
            try
            {
                var updatedUser = _userService.UpdateUser(user);
                updatedUser.Password = null;
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully updated user.",
                    Data = updatedUser
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

        [HttpPost()]
        public ActionResult UpdateUserProfile([FromBody] User user)
        {
            try
            {
                var updatedUser = _userService.UpdateUserProfile(user);
                updatedUser.Password = null;
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully updated user.",
                    Data = updatedUser
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

        [HttpPost()]
        public ActionResult UpdateUserPassword([FromBody] User user)
        {
            try
            {
                
                var updatedUser = _userService.UpdateUserPassword(user);
                updatedUser.Password = null;
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully updated user.",
                    Data = updatedUser
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


        [HttpGet]
        public ActionResult GetRoles()
        {
            try
            {
                var roles = _userService.GetRoles();
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched roles",
                    Data = roles
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

        [HttpGet]
        public ActionResult GetRoleByPersona(string personaName)
        {
            try
            {
                var roles = _userService.GetRoleByDescription(personaName);
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched roles",
                    Data = roles
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

        [HttpGet]
        public ActionResult GetRoleByNameAndPersona(string roleName, string personaName)
        {
            try
            {
                var roles = _userService.GetRoleByNameAndDescription(roleName, personaName);
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched roles",
                    Data = roles
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

        [HttpGet]
        public ActionResult GetForms()
        {
            try
            {
                var forms = _userService.GetForms();
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched forms",
                    Data = forms
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

        [HttpPost("{formName}")]
        public IActionResult CreateForm(string formName)
        {
            try
            {
                var form = new Form() { FormId = formName, FormName = formName };
                var result = _userService.InsertForm(form);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Form created successfully.",
                    Data = result
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

        [HttpPost("{formId}/{formName}")]
        public IActionResult UpdateForm(int formId, string formName)
        {
            try
            {
                var form = _userService.GetForm(formId);
                form.FormId = formName;
                form.FormName = formName;
                var result = _userService.UpdateForm(form);
                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Form updated successfully.",
                    Data = result
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

        [HttpPost("{formId}")]
        public IActionResult DeleteForm(string formId)
        {
            try
            {
                var result = _userService.DeleteForm(formId);
                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Form deleted successfully.",
                    Data = null
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

        [HttpGet]
        public ActionResult GetRolepermissions()
        {
            try
            {
                var roles = _userService.GetRolepermissions();
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched roles",
                    Data = roles
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

        [HttpPost]
        public IActionResult CreateRolepermission([FromBody]Rolepermission rolepermission)
        {
            try
            {
                var result = _userService.InsertRolepermission(rolepermission);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Form created successfully.",
                    Data = result
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

        [HttpPost]
        public IActionResult UpdateRolepermission([FromBody]Rolepermission rolepermission)
        {
            try
            {
                var per = _userService.GetRolepermission(rolepermission.Id);
                per.CanDelete = rolepermission.CanDelete;
                per.CanInsert = rolepermission.CanInsert;
                per.CanUpdate = rolepermission.CanUpdate;
                per.CanView = rolepermission.CanView;
                per.CanExport = rolepermission.CanExport;
                per.FormId = rolepermission.FormId;
                per.RoleId = rolepermission.RoleId;
                var result = _userService.UpdateRolepermission(per);
                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Rolepermission updated successfully.",
                    Data = result
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

        [HttpPost("{rolepermissionId}")]
        public IActionResult DeleteRolepermission(int rolepermissionId)
        {
            try
            {
                var result = _userService.DeleteRolepermission(rolepermissionId);
                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Rolepermission deleted successfully.",
                    Data = null
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

        [AllowAnonymous]
        [HttpGet("{customerId}/{userId}")]
        public ActionResult GetUserRoleandPersonaPermission(string customerId,string userId)
        {
            try
            {
                var rolesPermission = _userService.GetUserPermission(customerId, userId);
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched roles Permission",
                    Data = rolesPermission
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

    }
}
