using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.ServiceLayer;

namespace Xavor.SD.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IOTHubController : ControllerBase
    {
        private IIOTHubService _iotHubService;

        /// <summary>
        /// sends message from cloud to IOT Hub.
        /// </summary>
        /// <param name="message">device ir.</param>
        /// <param name="symetricKeys">symetric keys.</param>
        /// <returns>returns message delivery status.</returns>
        [HttpPost]
        public IActionResult SendCloudToDeviceMessage(dynamic message, bool symetricKeys = true)
        {
            ResponseDTO response = new ResponseDTO();
            try
            {

                _iotHubService.SendCloudtoDeviceMsg(message,message.deviceId);
                response.Data = true;
                response.Message = "Message has been sent to IOTHub Successfully";
                response.StatusCode = HttpStatusCode.OK.ToString();
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.Message = "An error occoured while sending message to IOTHub";
                response.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Ok(response);
            }

        }
    }
}
