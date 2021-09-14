using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.ServiceLayer.Service;

namespace Xavor.SD.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IReportingService _reportingService;
        public ReportController(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }


        [HttpGet]
        public IActionResult GetMaintenanceReport(string Customer, string BatchId, string DeviceId, string Date)
        {
            
            ResponseDTO response = new ResponseDTO();
            try
            {
                
                response.Data = _reportingService.GetMaintenanceReport(Customer, DeviceId, BatchId, Date);
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
        public IActionResult GetDeviceMaintenanceReport(string Customer, string Date,string Day)
        {

            ResponseDTO response = new ResponseDTO();
            try
            {

                response.Data = _reportingService.GetDeviceMaintenanceReport(Customer, Date,Day);
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
        public IActionResult GetConsumptionReport(string CustomerId, string BatchId, string DeviceId, string Date)
        {

            ResponseDTO response = new ResponseDTO();
            try
            {

                response.Data = _reportingService.GetConsumptionReport(CustomerId, DeviceId, BatchId, Date);
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



        [HttpGet("{CustomerId}/{DeviceId}")]
        public IActionResult GetAlarmHistoryReport(string CustomerId, string DeviceId, string Date)
        {

            ResponseDTO response = new ResponseDTO();
            try
            {

                response.Data = _reportingService.GetAlarmHistoryReport(CustomerId, DeviceId, Date);
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