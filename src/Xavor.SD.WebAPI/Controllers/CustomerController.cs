using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
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
    public class CustomerController : ControllerBase
    {
        private ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;
        private readonly IAccountCreation _accountCreation;
        private readonly IAuth _auth;
        private readonly IConfiguration _config;
        private readonly IDeviceService _deviceService;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger, IAccountCreation accountCreation, IAuth auth, IConfiguration configuration, IDeviceService deviceService)
        {
            _customerService = customerService;
            _logger = logger;
            _accountCreation = accountCreation;
            _auth = auth;
            _config = configuration;
            _deviceService = deviceService;
        }

        [HttpPost]
        public IActionResult Uploadbom(Bom bom)
        {
            try
            {
                var result = _customerService.uploadBom(bom);
                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Successfully uploaded bom",
                    Data = null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has occured: ");
                if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
                return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
            }
           
           
        }

        [HttpPost("{id}/{customerName}/{customerType}/{address}")]
        public IActionResult updateCustomer(int Id, string customerName, string CustomerType, string address)
        {
            try
            {
                var result = _customerService.updateCustomer(Id, customerName, address);
                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Successfully uploaded bom",
                    Data = null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has occured: ");
                if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
                return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
            }
            
           
        }

        /// <summary>
        /// list of all configurations of customer.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <returns>returns list of configurations.</returns>
        [HttpGet("{customerId}")]
        public IActionResult GetConfiguration(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                var response = new ResponseDTO()
                {
                    Message = "Invalid Request",
                    StatusCode = "Warning",
                    Data = null
                };
                return Ok(response);
            }
            try
            {
                var result = _customerService.GetConfigurationsByCustomer(customerId);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Successfully Fetched the List",
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

        [AllowAnonymous]
        [HttpPost("{userId}/{customerId}/{customerType}/{address}/{firstName}/{lastName}/{adminEmail}/{lang}")]
        public IActionResult CreateCustomer(string userId, string customerId, string customerType, string address, string firstName, string lastName, string adminEmail, string lang = "en")
        {
            try
            {
                var createdAdmin = _customerService.CreateCustomer(userId, customerId, customerType, address, firstName, lastName, adminEmail);
                _accountCreation.SendActivationEmail(createdAdmin.UserId, createdAdmin.Password.Decrypt(), firstName, lastName, lang);
                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Customer and admin created successfully.",
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
        [HttpPost("{userId}/{customerId}/{roleType}/{firstName}/{lastName}/{emailaddress}/{lang}")]
        public IActionResult CreateCustomerUser(string userId, string customerId, string roleType, string firstName, string lastName, string emailaddress, string lang = "en")
        {
            try
            {
                var createdUser = _customerService.CreateCustomerUser(userId, customerId, roleType, firstName, lastName, emailaddress);
                _accountCreation.SendActivationEmail(createdUser.UserId, createdUser.Password.Decrypt(), firstName, lastName, lang);
                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "User profile created successfully.",
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

        //[AllowAnonymous]
        [HttpPost]
        public IActionResult CreateDevices(string deviceIdPrefix, int startRange, int endRange, string customerId = null, string batchId = null)
        {
            try
            {
                if (_customerService.CreateDevices(deviceIdPrefix, startRange, endRange, customerId, batchId))
                {
                    var response = new ResponseDTO()
                    {
                        StatusCode = "Success",
                        Message = "Devices created or assigned successfully.",
                        Data = null
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new ResponseDTO()
                    {
                        StatusCode = "Failure",
                        Message = "An exception has occured",
                        Data = null
                    };
                    return Ok(response);
                }
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
        [HttpPost]
        public ActionResult SuperAdminSignIn(UserDTO user)
        {
            try
            {

                _logger.LogInformation("Loggin in...");

                var UserResponse = _auth.SuperAdminLogin(user);

                ResponseDTO response = new ResponseDTO();
                if (UserResponse != null)
                {
                    var token = _auth.GetToken(user.email);
                    Response.Headers.Add("X-API-Key", token.ToString());

                    response.StatusCode = "Success";
                    response.Message = "Logged In Successfully";
                    response.Data = token;

                }
                else
                {

                    response.StatusCode = "Warning";
                    response.Message = "Username or password is incorrect.";
                    response.Data = null;
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
        [HttpGet]
        public ActionResult GetCustomers()
        {
            try
            {
                var customerIds = _customerService.GetAllCustomerIds();
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched customers",
                    Data = customerIds
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
        public ActionResult GetAllCustomers()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched customers",
                    Data = customers
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
        public ActionResult GetPersonas()
        {
            try
            {
                var personas = _customerService.GetPersonas();
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched personas",
                    Data = personas
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
        [HttpPost, DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = 1048576000)]
        public async Task<IActionResult> UploadFirmwareFile(string customerId, string fileName)
        {
            try
            {
                var azureStorageAccount = _config.GetSection("BlobStorage:AzureStorage").Value;
                var containerName = _config.GetSection("BlobStorage:Container").Value;

                var file = Request.Form.Files;
                string fileUrl = null;
                if (file.Count > 0)
                {
                    fileUrl = await Utility.UploadFileAsync(file[0], azureStorageAccount, containerName, customerId, fileName, false);
                }

                return Ok(fileUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UploadDeviceFirmwareFile(Firmware firmware)
        {
            try
            {
                var azureStorageAccount = _config.GetSection("BlobStorage:AzureStorage").Value;
                var containerName = _config.GetSection("BlobStorage:Container").Value;

                var base64FirmwareData = firmware.FirmwareData;
                byte[] bytes = Convert.FromBase64String(base64FirmwareData);
                MemoryStream ms = new MemoryStream(bytes);
                string fileUrl = null;
                fileUrl = await Utility.UploadStreamAsync(ms, azureStorageAccount, containerName, firmware.CustomerId, firmware.BatchId, true, false);
                firmware.FirmwareData = fileUrl;
                _customerService.uploadfirmware(firmware);
                _deviceService.UpdateDeviceFirmware(firmware.FirmwareVersion, firmware.BatchId);
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched personas",
                    Data = fileUrl
                };
                return Ok(response);
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has occured: ");
                if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
                return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
            }
        }


        [AllowAnonymous]
        [HttpGet("{customerId}/{firmwareVersion}")]
        public async Task<IActionResult> GetUpdateFirmwareUrl(string customerId, string firmwareVersion)
        {
            try
            {
                var Url = _customerService.getFirmwareUrl(customerId, firmwareVersion);
                var net = new System.Net.WebClient();
                var data = net.DownloadData(Url);
                var content = new System.IO.MemoryStream(data);
                var contentType = "APPLICATION/octet-stream";
                var fileName = "Firmware.bin";
                return File(content, contentType, fileName);

                // return Ok(Utility.RestAPICall("", Url, "Get"));



            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public ActionResult GetPersonapermissions()
        {
            try
            {
                var roles = _customerService.GetPersonapermissions();
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched personapermissions",
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
        public IActionResult CreatePersonapermission([FromBody]Personapermission personapermission)
        {
            try
            {
                var result = _customerService.InsertPersonapermission(personapermission);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Personapermission created successfully.",
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
        public IActionResult UpdatePersonapermission([FromBody]Personapermission personapermission)
        {
            try
            {
                var per = _customerService.GetPersonapermission(personapermission.Id);
                per.FormId = personapermission.FormId;
                per.PersonaId = personapermission.PersonaId;
                var result = _customerService.UpdatePersonapermission(per);
                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Personapermission updated successfully.",
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

        [HttpPost("{PersonapermissionId}")]
        public IActionResult DeletePersonapermission(int personapermissionId)
        {
            try
            {
                var result = _customerService.DeletePersonapermission(personapermissionId);
                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Personapermission deleted successfully.",
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

    }
}