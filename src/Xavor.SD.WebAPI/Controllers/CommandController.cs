using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.ServiceLayer;


namespace Xavor.SD.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommandController : ControllerBase
    {

        /// <summary>
        /// sends acknowledgement that a command has been successfully executed.
        /// </summary>
        /// <param name="commandId">command id.</param>
        /// <returns>returns status.</returns>
        public IActionResult SendAcknowledgement(string commandId)
        {
            ResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = true;
                response.Message = "Command Acknowledged";
                response.StatusCode = HttpStatusCode.OK.ToString();
                return Ok(response);
            }
            catch(Exception e)
            {
                response.Data = false;
                response.Message = "Some error occured while Acknowledging command";
                response.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Ok(response);
            }            
        }
    }
}
