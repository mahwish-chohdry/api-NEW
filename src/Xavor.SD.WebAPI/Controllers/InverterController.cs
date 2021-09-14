using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.ServiceLayer.Service;

namespace Xavor.SD.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InverterController : Controller
    {
        private readonly IInverterService _inverterService;
        private readonly ILogger<InverterController> _logger;
        public InverterController(IInverterService inverterService, ILogger<InverterController> logger)
        {
            _inverterService = inverterService;
            _logger = logger;
        }

        
        public ActionResult GetInverterList(string lang = "en")
        {
            try
            {
                ResponseDTO response = null;
                var inverterList = _inverterService.GetInverterList(lang);
                
                    response = new ResponseDTO()
                    {
                        StatusCode = "Success",
                        Message = "Successfully fetched inverter list.",
                        Data = inverterList,
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