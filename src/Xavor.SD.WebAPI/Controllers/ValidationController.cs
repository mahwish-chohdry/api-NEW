using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Xavor.SD.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ValidationController : ControllerBase
    {
        // GET: Validation

        [HttpGet]
        public IActionResult Testing()
        {
            return Ok("API layer is working.");
        }

      
   

    
       

      
    }
}