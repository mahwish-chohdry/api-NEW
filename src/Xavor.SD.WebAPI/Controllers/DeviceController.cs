using System;
using System.Collections.Generic;
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
    [Route("api/[controller]/")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private IDeviceService _deviceService;
        private readonly IAccountCreation _accountCreation;
        public DeviceController(ILogger<DeviceController> logger, IDeviceService deviceService, IAccountCreation accountCreation)
        {
            _logger = logger;
            _deviceService = deviceService;
            _accountCreation = accountCreation;
        }

        /// <summary>
        /// creates new operator and also assigns devices to that operator.
        /// </summary>
        /// <param name="userDevice">userDevice dto.</param>
        /// <param name="customerId">customer id.</param>
        /// <param name="AdminUserId">admin user id.</param>
        /// <returns>returns status.</returns>
        [HttpPost("SaveUserDevices/{customerId}/{AdminUserId}/{lang}")]
        public IActionResult SaveUserDevices([FromBody] UserDeviceDTO userDevice, string customerId, string AdminUserId, string lang = "en")
        {

            try
            {
                if (string.IsNullOrEmpty(userDevice.password))
                {
                    userDevice.password = _accountCreation.GetRandomPassword(8).Encrypt();
                }
                var result = _deviceService.SaveUserDevices(userDevice, customerId, AdminUserId);
                _accountCreation.SendActivationEmail(userDevice.userId, userDevice.password.Decrypt(), "", "", lang);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Data = null
                };

                if (result)
                {
                    response.Message = "Operator has been added successfully.";
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
        /// updates existing operator and devices assigend to that operator.
        /// </summary>
        /// <param name="userDevice">userDevice dto.</param>
        /// <param name="customerId">customer id.</param>
        /// <param name="AdminUserId">admin user id.</param>
        /// <returns>returns status.</returns>
        [HttpPost("UpdateUserDevices/{customerId}/{AdminUserId}")]
        public IActionResult UpdateUserDevices([FromBody] UserDeviceDTO userDevice, string customerId, string AdminUserId)
        {
            try
            {
                var result = _deviceService.UpdateUserDevices(userDevice, customerId, AdminUserId);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Data = null
                };

                if (result)
                {
                    response.Message = "Operator has been updated successfully.";
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
        /// fetches all users of a customer along with the total count of devices assigned to that user.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="AdminUserId">admin user id.</param>
        /// <returns>returns list of users.</returns>
        [HttpGet("GetDevices/{customerId}/{AdminUserId}/users")]
        public IActionResult GetDevices2(string customerId, string AdminUserId)
        {

            try
            {

                var res = _deviceService.GetTotalDevicesByOperators(customerId, AdminUserId);
                ResponseDTO response = new ResponseDTO();
                if (res != null)
                {
                    response.StatusCode = "Success";
                    response.Message = "Successfully fetched the list.";
                    response.Data = res;

                }
                else
                {

                    response.StatusCode = "Warning";
                    response.Message = "Invalid request.";
                    response.Data = res;
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
        /// fetches list of all the fans of a customer along with its groups and status, fans which are associated with no group are in ungroup category, fans which are not configured yet are in offBoardFans and Grouped fans will be under their specific group.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="AdminUserId">admin user id.</param>
        /// <returns>returns list of all devices.</returns>
        [HttpGet("GetDevices/{customerId}/{AdminUserId}")]

        public IActionResult GetDevices(string customerId, string AdminUserId)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(AdminUserId))
            {
                var response = new ResponseDTO()
                {
                    Message = "Invalid request.",
                    StatusCode = "Warning",
                    Data = null
                };
                return Ok(response);
            }
            try
            {
                var result = _deviceService.GetDevices(customerId, AdminUserId);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched the list.",
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

        /// <summary>
        /// fetches list of all ungrouped and off board fans which can be assigned to a newly created group.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="AdminUserId">admin user id.</param>
        /// <returns>returns list of ungrouped and off board fans.</returns>
        [HttpGet("GetDevices/{customerId}/{AdminUserId}/group")]

        public IActionResult GetGroupedDevices(string customerId, string AdminUserId)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(AdminUserId))
            {
                var response = new ResponseDTO()
                {
                    Message = "Invalid request.",
                    StatusCode = "Warning",
                    Data = null
                };
                return Ok(response);
            }
            try
            {
                var result = _deviceService.GetDevices(customerId, AdminUserId, null, true, true);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched the list.",
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

        /// <summary>
        /// get specific group’s Fans along with ungroup and off board Fan.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="AdminUserId">admin user id.</param>
        /// <param name="groupId">group id.</param>
        /// <returns>returns list of ungrouped and off board fans along with specific group's fans.</returns>
        [HttpGet("GetDevices/{customerId}/{AdminUserId}/group/{groupId}")]

        public IActionResult GetGroupbyId(string customerId, string AdminUserId, string groupId)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(AdminUserId))
            {
                var response = new ResponseDTO()
                {
                    Message = "Invalid request.",
                    StatusCode = "Warning",
                    Data = null
                };
                return Ok(response);
            }
            try
            {
                var result = _deviceService.GetDevices(customerId, AdminUserId, null, false, true, groupId);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched the list.",
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

        /// <summary>
        /// fetches list of all existing groups along with devices also ungrouped and off board fans which can be assigned to a group.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="AdminUserId">admin user id.</param>
        /// <returns>returns list of all existing groups, ungrouped and off board fans.</returns>
        [HttpGet("GetDevices/{customerId}/{AdminUserId}/all")]

        public IActionResult GetAllDevices(string customerId, string AdminUserId)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(AdminUserId))
            {
                var response = new ResponseDTO()
                {
                    Message = "Invalid request.",
                    StatusCode = "Warning",
                    Data = null
                };
                return Ok(response);
            }
            try
            {
                var result = _deviceService.GetAllDevices(customerId, AdminUserId);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched the list.",
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


        [HttpGet("GetDevices/{customerId}/{AdminUserId}/{userId}")]

        public IActionResult GetUserDevices(string customerId, string AdminUserId, string userId)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(AdminUserId))
            {
                var response = new ResponseDTO()
                {
                    Message = "Invalid request",
                    StatusCode = "Warning",
                    Data = null
                };
                return Ok(response);
            }
            try
            {
                var result = _deviceService.GetAllUserDevices(customerId, AdminUserId, userId);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched the list.",
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

        /// <summary>
        /// fetches list of all the devices assigned to a user along with statuses of these devices.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="userId">user id.</param>
        /// <returns>returns list of operator's devices.</returns>
        [HttpGet("GetDevices/{customerId}/operator/{userId}")]
        public IActionResult GetOperatorDevices(string customerId, string userId)
        {
            try
            {
                var result = _deviceService.GetAllUserDevices(customerId, null, userId, false);

                var response = new ResponseDTO()
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched the list.",
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

        [HttpGet("GetDeviceStats")]
        public IActionResult GetDeviceStats(string customerId, string userId)
        {
            try
            {
                var devices = _deviceService.GetDeviceStats(customerId, userId);
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched devices",
                    Data = devices
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

        [HttpGet("GetAllDevices")]
        public IActionResult GetAllDevices(string customerId)
        {
            try
            {
                var devices = _deviceService.GetAllDevices(customerId);
               
                ResponseDTO response = new ResponseDTO
                {
                    StatusCode = "Success",
                    Message = "Successfully fetched devices",
                    Data = devices
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

        //[HttpGet("GetAllDevices/{customer_id}")]
        //public IActionResult GetAllDevices(int customer_id)
        //{
        //    try
        //    {
        //        var result = _deviceService.GetAllDevices(customer_id);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //       _logger.LogError( ex, "An exception has occured: ");
        //        if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
        //        return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
        //        //return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = "An exception has occured", Data = null });
        //    }

        //}


        [HttpPost("DeleteDevice/{customer_id}/{device_id}")]
        public IActionResult DeleteDevice(string customer_id, string device_id)
        {
            try
            {
                _deviceService.DeleteDevice(customer_id, device_id);
                var response = new ResponseDTO()
                {
                    Message = "Device has been deleted.",
                    StatusCode = "Success",
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

        [HttpGet("GetUnGroupedDevices/{customer_id}")]
        public IActionResult GetUnGroupedDevices(int customer_id)
        {
            var res = _deviceService.GetUnGroupedDevices(customer_id);
            return Ok(res);
        }

        /// <summary>
        /// sends command to a specific device.
        /// </summary>
        /// <param name="deviceStatus">device status dto.</param>
        /// <param name="customerId">customer id.</param>
        /// <param name="userId">user id.</param>
        /// <param name="deviceId">device id.</param>
        /// <returns>returns status.</returns>
        [HttpPost("SendCommand/{customerId}/{userId}/{deviceId}/device")]
        public IActionResult SendDevicecommand(StatusDTO deviceStatus, string customerId, string userId, string deviceId)
        {
            try
            {
                deviceStatus.DeviceId = deviceId;
                var res = _deviceService.SendDevicecommand(deviceStatus, customerId, userId, deviceId);
                if (res)
                {
                    var response = new ResponseDTO()
                    {
                        Message = "Settings have been updated successfully.",
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

        /// <summary>
        /// sends command to a specific group.
        /// </summary>
        /// <param name="deviceStatus">device status dto.</param>
        /// <param name="customerId">customer id.</param>
        /// <param name="userId">user id.</param>
        /// <param name="groupId">group id.</param>
        /// <returns>returns status.</returns>
        [HttpPost("SendCommand/{customerId}/{userId}/{groupId}/group")]
        public IActionResult SendGroupcommand(StatusDTO deviceStatus, string customerId, string userId, string groupId)
        {
            try
            {
                var res = _deviceService.SendGroupcommand(deviceStatus, customerId, userId, groupId);
                if (res)
                {
                    var response = new ResponseDTO()
                    {
                        Message = "Settings have been updated successfully.",
                        StatusCode = "Success",
                        Data = null
                    };
                    return Ok(response);
                }

                return StatusCode(200, "Unknown error.");
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
        /// fetches settings of a specific group.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="userId">user id.</param>
        /// <param name="groupId">group id.</param>
        /// <returns>returns settings of a group.</returns>
        [HttpGet("GetSettings/{customerId}/{userId}/{groupId}/group")]
        public IActionResult GetGroupcommand(string customerId, string userId, string groupId)
        {
            try
            {
                var res = _deviceService.GetGroupcommand(customerId, userId, groupId);
                if (res != null)
                {
                    var response = new ResponseDTO()
                    {
                        Message = "Successfully fetched group command.",
                        StatusCode = "Success",
                        Data = res
                    };
                    return Ok(response);
                }
                else
                {

                    var response = new ResponseDTO()
                    {
                        Message = "Group command empty.",
                        StatusCode = "Success",
                        Data = res
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
        [HttpGet("GetSettings/{customerId}/{userId}/{deviceId}/device")]
        public IActionResult GetDevicecommand(string customerId, string userId, string deviceId)
        {
            try
            {
                var res = _deviceService.GetDevicecommand(customerId, userId, deviceId);
                if (res != null)
                {
                    var response = new ResponseDTO()
                    {
                        Message = "Successfully fetched device command.",
                        StatusCode = "Success",
                        Data = res
                    };
                    return Ok(response);
                }
                else
                {

                    var response = new ResponseDTO()
                    {
                        Message = "Group command empty.",
                        StatusCode = "Success",
                        Data = res
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
        [HttpPost("SendDeviceState/{customerId}/{deviceId}")]

        public IActionResult SendDeviceState(string customerId, string deviceId)
        {
            try
            {
                _deviceService.SendDeviceState(customerId, deviceId);

                var response = new ResponseDTO()
                {
                    Message = "State has been send to device",
                    StatusCode = "Success",
                    Data = null
                };
                return Ok(response);



            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost("SendAcknowledgement/{commandId}")]
        public IActionResult SendAcknowledgement(string commandId)
        {
            try
            {
                var res = _deviceService.SendAcknowledgement(commandId);
                if (res)
                {
                    var response = new ResponseDTO()
                    {
                        Message = "Command has been successfully executed.",
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

        /// <summary>
        /// resets device’s running hours.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="userId">user id.</param>
        /// <param name="deviceId">device id.</param>
        /// <returns>returns status.</returns>
        [HttpPost("ResetRunningHours/{customerId}/{userId}/{deviceId}")]
        public IActionResult ResetRunningHours(string customerId, string userId, string deviceId)
        {
            try
            {
                _deviceService.ResetRunningHours(customerId, userId, deviceId);

                var response = new ResponseDTO()
                {
                    Message = $"Running hours have been reset for the device: \"{deviceId}\".",
                    StatusCode = "Success",
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


        [HttpPost("ResetGroupRunningHours/{customerId}/{userId}/{groupId}")]
        public IActionResult ResetGroupRunningHours(string customerId, string userId, string groupId)
        {
            try
            {
                _deviceService.ResetGroupRunningHours(customerId, userId, groupId);

                var response = new ResponseDTO()
                {
                    Message = $"Running hours have been reset for the device of group: \"{groupId}\".",
                    StatusCode = "Success",
                    Data = null
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "ResponseException")
                {
                    return StatusCode(200, ((ResponseException)ex)._response);
                }

                return StatusCode(200, ex);
            }


        }

        /// <summary>
        /// get specific device's usage.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="userId">user id.</param>
        /// <param name="deviceId">device id.</param>
        /// <param name="days">days.</param>
        /// <param name="date">date.</param>
        /// <returns>returns device usage.</returns>
        [HttpGet("GetDeviceUsage/{customerId}/{userId}/{deviceId}/{date}/{days}")]
        public IActionResult GetDeviceUsage(string customerId, string userId, string deviceId, int days, string date)
        {
            try
            {
                var usageList = _deviceService.GetDeviceUsage(customerId, userId, deviceId, date, days);

                var response = new ResponseDTO()
                {
                    Message = $"Sucessfully retrieved list.",
                    StatusCode = "Success",
                    Data = usageList
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

        [HttpGet("GetAllUsageReport/{customerId}/{userId}/{date}/{days}")]
        public IActionResult GetAllUsageReport(string customerId, string userId, int days, string date)
        {
            try
            {
                var usageList = _deviceService.GetAllUsage(customerId, userId, date, days);

                var response = new ResponseDTO()
                {
                    Message = $"Sucessfully retrieved list.",
                    StatusCode = "Success",
                    Data = usageList
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
        /// to change the name of a device
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="userId">user id</param>
        /// <param name="deviceId">device id.</param>
        /// <param name="deviceDTO">device dto.</param>
        /// <returns>returns status.</returns>
        [HttpPost("CustomizeDevice/{customerId}/{userId}/{deviceId}")]
        public IActionResult CustomizeDevice(string customerId, string userId, string deviceId, DeviceDTO deviceDTO)
        {
            try
            {

                if (_deviceService.CustomizeDeviceName(customerId, userId, deviceId, deviceDTO.deviceName))
                {
                    var response = new ResponseDTO()
                    {
                        Message = "Device has been renamed successfully.",
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

        [HttpPost("UpdateDevice/{customerId}/{deviceId}/{deviceName}")]
        public IActionResult CustomizeDeviceName(string customerId, string deviceId, string devicename)
        {
            try
            {

                if (_deviceService.CustomizeDeviceName(customerId, deviceId, devicename))
                {
                    var response = new ResponseDTO()
                    {
                        Message = "Device has been renamed successfully.",
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

        [AllowAnonymous]
        [HttpPost("ChangeDeviceSpeed/{customerId}/{deviceId}/{speed}")]
        public IActionResult ChangeDeviceSpeed(string customerId, string deviceId, int speed)
        {
            try
            {

                if (_deviceService.ChangeDeviceSpeed(customerId, deviceId, speed))
                {
                    var response = new ResponseDTO()
                    {
                        Message = "Device speed has been changed.",
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

        [AllowAnonymous]
        [HttpPost("ChangeDevicePowerStatus/{customerId}/{deviceId}/{powerStatus}")]
        public IActionResult ChangeDevicePowerStatus(string customerId, string deviceId, short powerStatus)
        {
            try
            {

                if (_deviceService.ChangeDevicePowerStatus(customerId, deviceId, powerStatus))
                {
                    var response = new ResponseDTO()
                    {
                        Message = "Device power status has been updated.",
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

        [AllowAnonymous]
        [HttpPost("DeleteDevices/{customerId}/{deviceId}")]
        public IActionResult DeleteDeviceById(string customerId, string deviceId)
        {
            try
            {

                if (_deviceService.DeleteDeviceById(customerId, deviceId))
                {
                    var response = new ResponseDTO()
                    {
                        Message = "Device has been deleted successfully.",
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

        [Authorize]
        [HttpGet("BatchNumber")]
        public IActionResult BatchNumber()
        {
            try
            {
                var result = _deviceService.GetAllBatchNumber();
                if (result != null)
                {
                    var response = new ResponseDTO()
                    {
                        Message = "Device has been deleted successfully.",
                        StatusCode = "Success",
                        Data = result
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

    }
}