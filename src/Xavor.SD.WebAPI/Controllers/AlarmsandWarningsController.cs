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
using Xavor.SD.ServiceLayer.Service;

namespace Xavor.SD.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AlarmsandwarningsController : ControllerBase
    {
        private IAlarmsandwarningsService _alarmsService;
        private IEnvironmentstandardsService _standardService;

        private IEnvironmentsensorsService _sensorService;
        public AlarmsandwarningsController(IAlarmsandwarningsService alarmsService,IEnvironmentstandardsService standardservice, IEnvironmentsensorsService sensorService)
        {
            _alarmsService = alarmsService;
            _standardService = standardservice;
            _sensorService = sensorService;
        }


        [HttpGet]
        public IActionResult GetAlarmWarningReport(string Customer, string BatchId, string DeviceId, string Date)
        {

            ResponseDTO response = new ResponseDTO();
            try
            {

                response.Data = _alarmsService.GetAlarmWarningReport(Customer, BatchId, DeviceId, Date);
                response.Message = "SUCCESS";
                response.StatusCode = "Success";
                return Ok(response);
            }
            catch (Exception e)
            {

                response.Data = null;
                response.Message = e.ToString();
                response.StatusCode = "Success";
                return Ok(response);
            }
        }

        /// <summary>
        /// list of all alarms and warnings.
        /// </summary>
        /// <returns>return list of alarms and warnings.</returns>
        [HttpGet]
        public IActionResult GetAlarmsandwarnings(string lang = "en",string inverterId = null)
        {
            ResponseDTO response = new ResponseDTO();
            try
            {

                response.Data = _alarmsService.GetAllAlarms(lang, inverterId);
                response.Message = "SUCCESS";
                response.StatusCode = "Success";
                return Ok(response);
            }
            catch (Exception e)
            {

                response.Data = null;
                response.Message = e.ToString();
                response.StatusCode = "Success";
                return Ok(response);
            }
        }

        [HttpGet]
        public IActionResult GetAlarmAndWarningByCode(string code, string lang = "en")
        {
            ResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _alarmsService.GetAlarmAndWarningByCode(code, lang);
                response.Message = "SUCCESS";
                response.StatusCode = "Success";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Data = null;
                response.Message = e.ToString();
                response.StatusCode = "Success";
                return Ok(response);
            }
        }


        [HttpGet]
        public IActionResult GetEnvironmentstandards()
        {
            ResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _standardService.GetAllStandards();
                response.Message = "SUCCESS";
                response.StatusCode = "Success";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Data = null;
                response.Message = e.ToString();
                response.StatusCode = "Success";
                return Ok(response);
            }
        }
        [HttpGet]
        public IActionResult GetEnvironmentsensors()
        {
            ResponseDTO response = new ResponseDTO();
            try
            {
                response.Data = _sensorService.GetAllSensors();
                response.Message = "SUCCESS";
                response.StatusCode = "Success";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Data = null;
                response.Message = e.ToString();
                response.StatusCode = "Success";
                return Ok(response);
            }
        }


    }
}
