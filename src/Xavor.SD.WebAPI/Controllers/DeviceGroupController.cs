using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.ServiceLayer;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DeviceGroupController : ControllerBase
    {
        private readonly IGroupsService _groupService;
        private readonly ILogger<DeviceGroupController> _logger;

        public DeviceGroupController(IGroupsService groupService, ILogger<DeviceGroupController> logger)
        {
            _groupService = groupService;
            _logger = logger;
        }

        /// <summary>
        /// save group with group name and device list into database.
        /// </summary>
        /// <param name="userDevice">user device dto.</param>
        /// <param name="customerId">customer id.</param>
        /// <param name="AdminUserId">admin user dto.</param>
        /// <returns>returns status.</returns>
        [HttpPost("{customerId}/{AdminUserId}")] //create
        //Save Group
        public IActionResult SaveGroup([FromBody] UserDeviceDTO userDevice, string customerId, string AdminUserId)
        {
            try
            {
                var result = _groupService.SaveGroup(userDevice, customerId, AdminUserId);

                if (!string.IsNullOrEmpty(result))
                {
                    var response = new ResponseDTO()
                    {
                        StatusCode = "Success",
                        Message = userDevice.deviceGroupList[0].groupName + " has been added successfully.",
                        Data = new
                        {
                            groupId = result
                        }
                    };

                    return Ok(response);
                }

                return Ok(null);


            }
            catch (Exception ex)
            {
               _logger.LogError( ex, "An exception has occured: ");
                if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
                return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
                //return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = "An exception has occured", Data = null });
            }

        }
        /// <summary>
        /// update existing group with group name and devices list in database.
        /// </summary>
        /// <param name="userDevice">user device dto.</param>
        /// <param name="customerId">customer id.</param>
        /// <param name="AdminUserId">admin user id.</param>
        /// <param name="groupId">group id.</param>
        /// <returns>returns status and updated group id.</returns>
        [HttpPost("{customerId}/{AdminUserId}/{groupId}")] //update
        //Update group
        public IActionResult SaveGroup([FromBody] UserDeviceDTO userDevice, string customerId, string AdminUserId, string groupId)
        {
            try
            {

                var result = _groupService.SaveGroup(userDevice, customerId, AdminUserId, groupId);

                if (result)
                {
                    var response = new ResponseDTO()
                    {
                        StatusCode = "Success",
                        Message = "Group has been updated successfully.",
                        Data = new
                        {
                            groupId
                        }
                    };

                    return Ok(response);
                }

                return Ok(null);
            }
            catch (Exception ex)
            {
               _logger.LogError( ex, "An exception has occured: ");
                if (ex.GetType().Name == "ResponseException") { return StatusCode(200, ((Common.Utilities.ResponseException)ex)._response); }
                return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = ex.Message, Data = null });
                //return StatusCode(200, new ResponseDTO() { StatusCode = "Failure", Message = "An exception has occured", Data = null });
            }

        }

    }
}