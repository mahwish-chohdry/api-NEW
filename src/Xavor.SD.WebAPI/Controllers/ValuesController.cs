using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xavor.SD.ServiceLayer;

namespace Xavor.SD.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;
        public ValuesController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var response = "OK 200";
            _logger.LogInformation(response);
            return Ok(response);
        }
    }
}
