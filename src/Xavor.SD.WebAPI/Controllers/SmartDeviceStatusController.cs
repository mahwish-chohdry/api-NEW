using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

using Xavor.SD.ServiceLayer;
using Xavor.SD.ServiceLayer.Transformations;

namespace Xavor.SD.WebAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SmartDevicestatusController : ControllerBase
    {
        IConfiguration configuration;
        private readonly IDeviceService deviceService;
        private readonly IAlarmsService alarmsService;
        private readonly ITransformations _transformations;
        private readonly ILogger<SmartDevicestatusController> _logger;

        //public SmartDevicestatusController(ISmartDeviceContainerBL _bl)
        public SmartDevicestatusController(IDeviceService _deviceService, IConfiguration _configuration, ILogger<SmartDevicestatusController> logger, IAlarmsService _alarmsService, ITransformations transformations)
        {
            deviceService = _deviceService;
            configuration = _configuration;
            alarmsService = _alarmsService;
            _transformations = transformations;
        }

        /// <summary>
        /// sets status of a device.
        /// </summary>
        /// <param name="deviceStatus">smart device container dto.</param>
        /// <returns>returns status.</returns>
        [HttpPost(Name = "SetDevicestatus")]
        public async Task<IActionResult> SetDevicestatus(SmartDeviceContainer deviceStatus)
        {
            try
            {
                var result = deviceService.AddOrUpdateDevicestatus(deviceStatus);
                var transformedResponse = _transformations.TransformDevicestatusToStatusDTO(result, deviceStatus.DeviceId);
                transformedResponse.IsDeviceStatus = true;
                transformedResponse.connectivityStatus = "Online";
                deviceService.UpdateDeviceState(deviceStatus.CurrentFirmwareVersion, deviceStatus.DeviceId, deviceStatus.InverterId);
                if (!Utility.CompareDeviceStatus(deviceStatus, transformedResponse))
                {
                    //deviceService.SendDeviceState(deviceStatus.CustomerId, deviceStatus.DeviceId);
                }
                var response = new ResponseDTO()
                {
                    Message = "Success",
                    StatusCode = "Success",
                    Data = transformedResponse
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseDTO()
                {
                    Message = "Failure",
                    StatusCode = "Failure",
                    Data = "Something Went Wrong"
                };
                return Ok(response);
            }
        }

        /// <summary>
        /// sets alarms of a device.
        /// </summary>
        /// <param name="Alarm">inverter alarms.</param>
        /// <returns>returns alarms of a device.</returns>
        [HttpPost(Name = "SetDevicealarms")]
        public async Task<IActionResult> SetDevicealarms(InverterAlarms Alarm)
        {
            List<InverterAlarms> allAlarms = new List<InverterAlarms>();
            allAlarms.Add(Alarm);
            List<Devicealarms> addedAlarms = new List<Devicealarms>();


            if (allAlarms != null && allAlarms.Count() > 0)
            {
                addedAlarms = alarmsService.InsertNewAlarmsIfNotExistInDatabase(allAlarms);
            }
            var result = alarmsService.TranformDevicealarms(addedAlarms, "en");
            result[0].IsDeviceStatus = false;
            var response = new ResponseDTO()
            {
                Message = "Success",
                StatusCode = "Success",
                Data = result
            };

            return Ok(response);
        }


        /// <summary>
        /// sends device state from cloud to device.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="deviceId">device id.</param>
        /// <returns>returns status.</returns>
        [HttpPost("/{customerId}/{deviceId}")]
        public IActionResult SendDeviceState(string customerId, string deviceId)
        {
            try
            {
                deviceService.SendDeviceState(customerId, deviceId);

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

        /// <summary>
        /// fetches device status.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="deviceId">device id.</param>
        /// <returns>returns device status.</returns>
        [HttpGet(Name = "GetDevicestatus")]
        public async Task<IActionResult> GetDevicestatus(string customerId, string deviceId)
        {
            try
            {
                // getting devicestatus from MSSQL
                var responseList = deviceService.GetDevicestatus(customerId, deviceId);
                if (responseList == null)
                {
                    responseList = _transformations.TransformDefaultSettingsToDeviceStatus(deviceId);
                }


                DevicestatusDTO transformedResponse = null;
                DeviceDTO transformedDeviceDTO = null;
                if (responseList != null)
                {
                    transformedResponse = _transformations.TransformDevicestatusToStatusDTO(responseList, deviceId);
                    transformedResponse.MessageType = true;
                    transformedDeviceDTO = _transformations.TransformDevicestatusDTOToDeviceDTO(transformedResponse, responseList.ModifiedDate, customerId);
                }

                var response = new ResponseDTO()
                {
                    Message = "Success",
                    StatusCode = "Success",
                    Data = transformedDeviceDTO
                };
                if (transformedDeviceDTO == null)
                {
                    response.Message = "No record found.";
                    response.StatusCode = "Warning";
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



        /// <summary>
        /// fetches device alarms list.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="deviceId">device id.</param>
        /// <returns>returns list of device alarms.</returns>
        [HttpGet(Name = "GetDevicealarms")]
        public async Task<IActionResult> GetDevicealarms(string customerId, string deviceId, string lang = "en")
        {
            try
            {
                var deviceAlarmsFromDb = deviceService.GetDevicealarms(customerId, deviceId);
                var allAlarms = new List<Devicealarms>();
                allAlarms.Add(deviceAlarmsFromDb);

                List<DevicealarmsDTO> result = null;
                if (allAlarms.Count != 0 && allAlarms != null && allAlarms[0] != null)
                {
                    result = alarmsService.TranformDevicealarms(allAlarms, lang);
                }

                var response = new ResponseDTO()
                {
                    Message = "Success",
                    StatusCode = "Success",
                    Data = result
                };

                if (result == null || result.Count == 0)
                {
                    response.Message = "No record found.";
                    response.StatusCode = "Warning";
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

        /// <summary>
        /// fetches history of device alarms.
        /// </summary>
        /// <param name="customerId">customer id.</param>
        /// <param name="deviceId">device id.</param>
        /// <param name="issueType">issue type.</param>
        /// <returns>returns list of device alarms history.</returns>
        [HttpGet(Name = "GetDevicealarmsHistory")]
        public async Task<IActionResult> GetDevicealarmsHistory(string customerId, string deviceId, string issueType, string lang = "en")
        {
            try
            {
                List<Devicealarms> addedAlarms = new List<Devicealarms>();

                var deviceAlarmsHistoryFromDb = alarmsService.GetDeviceAlarmsHistory(customerId, deviceId, issueType);

                var result = alarmsService.TranformDevicealarmsHistory(deviceAlarmsHistoryFromDb, issueType, lang);

                var response = new ResponseDTO()
                {
                    Message = "Success",
                    StatusCode = "Success",
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
        /// performs preprocessing on alarms.
        /// </summary>
        /// <param name="inverterAlarms">list of device alarms dto.</param>
        /// <returns>returns list of alarms after preprocessing.</returns>
        [HttpGet(Name = "PreProcessAlarmsandwarnings")]
        public List<Devicealarms> PreProcessAlarmsandwarnings(List<Devicealarms> inverterAlarms)
        {
            foreach (var reading in inverterAlarms)
            {
                // reading.Alarm = ((Alarms)Convert.ToInt32(reading.Alarm)).ToString();
                // reading.Warning = ((Warnings)Convert.ToInt32(reading.Warning)).ToString();
            }
            return inverterAlarms;
        }
    }
}