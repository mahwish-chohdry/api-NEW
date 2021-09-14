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
//using Xavor.SD.ServiceLayer.Service;

namespace Xavor.SD.WebAPI.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class IOTDeviceController : ControllerBase
    {
        private IIOTDeviceService _iotDeviceService;
        private readonly ICustomerService _customerService;
        private readonly IDeviceService _deviceService;

        public IOTDeviceController(IIOTDeviceService iotDeviceService, ICustomerService customerService, IDeviceService deviceService)
        {
            _iotDeviceService = iotDeviceService;
            _customerService = customerService;
            _deviceService = deviceService;
        }

        //[HttpGet("GetSasToken/{deviceId}")]
        //public IActionResult GetSasToken(string deviceId, bool requestStatus = false)
        //{
        //    ResponseDTO response = new ResponseDTO();
        //    try
        //    {
        //        var res = _iotDeviceService.GetSasToken(deviceId, requestStatus);
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Data = null;
        //        response.Message = "Error occoured while generating Sas Token";
        //        response.StatusCode = HttpStatusCode.InternalServerError.ToString();
        //        return Ok(response);
        //    }
        //}


        /// <summary>
        /// creates a new device.
        /// </summary>
        /// <param name="deviceId">device id.</param>
        /// <param name="symetricKeys">symetric keys.</param>
        /// <returns>returns device creation status.</returns>
        /// FIRMWARE UPDATION IS NEEDED TO BE MOVED TO THIS API 

        [HttpPost("CreateDevice/{deviceId}")]
        public async Task<IActionResult> CreateDevice(string deviceId, bool symetricKeys = true)
        {
            SmartBoxResponseDTO response = new SmartBoxResponseDTO();
            try
            {
                var res = await _iotDeviceService.CreateDevice(deviceId, null);
                response.Data = res;
                response.StatusCode = HttpStatusCode.OK.ToString();

                if (!string.IsNullOrEmpty(res))
                {
                    var device = _deviceService.GetDeviceByUniqueId(deviceId);
                    var customer = _customerService.GetCustomerById(device.CustomerId.Value);
                    response.CustomerId = customer.CustomerId;
                    response.Timezone = _customerService.GetConfigurationsByName(customer.Id, "TimeZone").Value;
                    response.PostStatusFrequency = _customerService.GetConfigurationsByName(customer.Id, "PostStatusFrequency").Value;
                    //response.Ruleengine = _customerService.GetRuleEngineByCustomerId(customer.Id);
                    response.Message = "Device has been created Successfully";
                }
                else
                    response.Message = "An error occured while creating device or getting its connection string from IOT Hub";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = "An error occoured while creating the device: " + ex.Message;
                response.StatusCode = HttpStatusCode.OK.ToString();
                return Ok(response);
            }
        }

        /// <summary>
        /// creates a new device.
        /// </summary>
        /// <param name="deviceId">device id.</param>
        /// <returns>returns device creation status.</returns>
        [HttpGet("CreateDevice/{deviceId}")]
        public async Task<IActionResult> CreateDevice(string deviceId)
        {
            SmartBoxResponseDTO response = new SmartBoxResponseDTO();
            try
            {
                var res = await _iotDeviceService.CreateDevice(deviceId, null);
                response.Data = res;
                response.StatusCode = HttpStatusCode.OK.ToString();

                if (!string.IsNullOrEmpty(res))
                {
                    var device = _deviceService.GetDeviceByUniqueId(deviceId);
                    var customer = _customerService.GetCustomerById(device.CustomerId.Value);
                    response.CustomerId = customer.CustomerId;
                    response.Timezone = _customerService.GetConfigurationsByName(customer.Id, "TimeZone").Value;
                    response.PostStatusFrequency = _customerService.GetConfigurationsByName(customer.Id, "PostStatusFrequency").Value;
                    //response.Ruleengine = _customerService.GetRuleEngineByCustomerId(customer.Id);
                    response.Message = "Device has been created Successfully";
                }
                else
                    response.Message = "An error occured while creating device or getting its connection string from IOT Hub";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = "An error occoured while creating the device: " + ex.Message;
                response.StatusCode = HttpStatusCode.OK.ToString();
                return Ok(response);
            }
        }

        [HttpGet("CreateDevice/{deviceId}/{inverterId}")]
        public async Task<IActionResult> CreateDevice(string deviceId, string inverterId)
        {
            SmartBoxResponseDTO response = new SmartBoxResponseDTO();
            try
            {
                var res = await _iotDeviceService.CreateDevice(deviceId, inverterId);
                response.Data = res;
                response.StatusCode = HttpStatusCode.OK.ToString();

                if (!string.IsNullOrEmpty(res))
                {
                    var device = _deviceService.GetDeviceByUniqueId(deviceId);
                    var customer = _customerService.GetCustomerById(device.CustomerId.Value);
                    response.CustomerId = customer.CustomerId;
                    response.Timezone = _customerService.GetConfigurationsByName(customer.Id, "TimeZone").Value;
                    response.PostStatusFrequency = _customerService.GetConfigurationsByName(customer.Id, "PostStatusFrequency").Value;
                    //response.Ruleengine = _customerService.GetRuleEngineByCustomerId(customer.Id);
                    response.Message = "Device has been created Successfully";
                }
                else
                    response.Message = "An error occured while creating device or getting its connection string from IOT Hub";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = "An error occoured while creating the device: " + ex.Message;
                response.StatusCode = HttpStatusCode.OK.ToString();
                return Ok(response);
            }
        }
    }
}
