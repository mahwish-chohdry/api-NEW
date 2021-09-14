using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common;
using Microsoft.Azure.Devices.Common.Security;
using Xavor.SD.BusinessLayer;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.ServiceLayer.Service;
using Xavor.SD.ServiceLayer.ServiceModel;
using Device = Xavor.SD.Model.Device;
//using Xavor.SD.ServiceLayer.Service;

namespace Xavor.SD.ServiceLayer
{
    public class IOTDeviceService : IIOTDeviceService
    {
        private readonly IConfigurationsBL _configBL;
        private readonly IDeviceBL _deviceBL;
        private RegistryManager _registryManager;
        private string _iotHostName;
        private string _sharedAccesskey;
        private string _sharedAccesskeyName;
        private string _primaryKey;
        private string _secondaryKey;
        private readonly ICommandhistoryBL _commandhistoryBL;
        private readonly IIOTHubService _iotHubService;
        private readonly IInverterService _inverterService;


        public IOTDeviceService(IConfigurationsBL configBL, IDeviceBL deviceBL, IIOTHubService iotHubService, ICommandhistoryBL commandhistoryBL, IInverterService inverterService)
        {
            _iotHubService = iotHubService;
            _configBL = configBL;
            _deviceBL = deviceBL;
            _commandhistoryBL = commandhistoryBL;
            _inverterService = inverterService;
        }
        private void parseIoTHubConnectionString(string connectionString, bool skipException = false)
        {
            try
            {
                var builder = IotHubConnectionStringBuilder.Create(connectionString);
                _iotHostName = builder.HostName;
                _sharedAccesskey = builder.SharedAccessKey;
                _sharedAccesskeyName = builder.SharedAccessKeyName;
            }
            catch (Exception ex)
            {
                if (!skipException)
                {
                    throw new ArgumentException("Invalid IoTHub connection string. " + ex.Message);
                }
            }
        }
        //public ResponseDTO GetSasToken(string deviceId, bool isPostStatus)
        //{
        //    ResponseDTO resposne = new ResponseDTO();
        //    string sasToken = string.Empty;
        //    try
        //    {
        //        Device device = _deviceBL.GetDevice(deviceId);
        //        if (device == null)
        //        {
        //            resposne.Data = null;
        //            resposne.Message = "Device does not exist: Invalid Device Id.";
        //            resposne.StatusCode = Constants.STATUSCODES.INVALID_DEVICE.ToString();
        //        }

        //        if(isPostStatus)
        //        {
        //           // _commandhistoryBL.GetCommandhistory(1);
        //            Commandhistory commandHist=_commandhistoryBL.GetLastExceutedCommand(device.Id);
        //            if (commandHist != null)
        //                _iotHubService.SendCloudtoDeviceMsg(commandHist,deviceId);
        //        }

        //        if (device.IsInstalled == 1)
        //        {
        //            sasToken=GenerateSasToken();
        //            resposne.Data = sasToken;
        //            resposne.Message =  "Sas Token created Successfully";
        //            resposne.StatusCode = HttpStatusCode.OK.ToString();
        //        }

        //        else 
        //        {
        //            if (CreateDevice(deviceId))
        //            {
        //                sasToken= GenerateSasToken();
        //                resposne.Data = sasToken;
        //                resposne.Message = "Device and Sas Token created Successfully";
        //                resposne.StatusCode = HttpStatusCode.OK.ToString();
        //            }
        //            else
        //            {
        //                resposne.Data = null;
        //                resposne.Message = "Failed to create device on IOT Hub.Please try again.";
        //                resposne.StatusCode = Constants.STATUSCODES.Create_Device_Failed.ToString();
        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //        throw ex;
        //    }

        //    return resposne;

        //}

        public string GenerateSasToken()
        {
            string builder = string.Empty;
            Configurations iotConnectionConfig = _configBL.GetConfiguration(Constants.IOT_HUB_CONNECTION_STRING);
            parseIoTHubConnectionString(iotConnectionConfig.Value);

            Configurations sasTokenExpiryConfig = _configBL.GetConfiguration(Constants.SAS_TOKEN_EXPIRY);

            builder = new SharedAccessSignatureBuilder()
            {
                KeyName = _sharedAccesskeyName,
                Key = _sharedAccesskey,
                Target = _iotHostName,
                TimeToLive = sasTokenExpiryConfig != null ? TimeSpan.FromHours(Convert.ToDouble(sasTokenExpiryConfig.Value)) : TimeSpan.FromHours(Convert.ToDouble(Constants.SAS_TOKEN_EXPIRY_DEFAULT))
            }.ToSignature();

            return builder;
        }
        public async Task<string> CreateDevice(string deviceId, string inverterId, bool symetricKeys = true)
        {
            string deviceConnectionstring = string.Empty;
            try
            {
                Configurations iotHubConnectionConfig = _configBL.GetConfiguration(Constants.IOT_HUB_CONNECTION_STRING);
                _registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionConfig.Value);
                var existingDevice = await _registryManager.GetDeviceAsync(deviceId);
                if (existingDevice == null)
                {

                    var iotDevice = new Microsoft.Azure.Devices.Device(deviceId);
                    iotDevice.Authentication = new AuthenticationMechanism();

                    if (symetricKeys)
                    {
                        autoGenerateDeviceKeys();
                        iotDevice.Authentication.SymmetricKey.PrimaryKey = _primaryKey;
                        iotDevice.Authentication.SymmetricKey.SecondaryKey = _secondaryKey;
                    }

                    _registryManager.AddDeviceAsync(iotDevice);

                    deviceConnectionstring = CreateDeviceConnectionString(iotDevice, iotHubConnectionConfig.Value);
                }

                else
                {
                    deviceConnectionstring = CreateDeviceConnectionString(existingDevice, iotHubConnectionConfig.Value);
                }
                //update device installation status

                Device device = _deviceBL.GetDevice(deviceId);
                if (device != null)
                {//is installed should be bool
                    if (!Convert.ToBoolean(device.IsInstalled))
                    {
                        device.IsInstalled = 1;
                        device.ModifiedDate = DateTime.UtcNow;
                    }
                    //updating inverter;
                    if (inverterId != null)
                    {
                        device.InverterId = _inverterService.GetInverter(inverterId).Id;
                    }

                    _deviceBL.UpdateDevice(device);
                }
                return deviceConnectionstring;
            }
            catch (Exception e)
            {
                return null;
                throw e;
            }
        }

        private void autoGenerateDeviceKeys()
        {
            _secondaryKey = CryptoKeyGenerator.GenerateKey(32);
            _primaryKey = CryptoKeyGenerator.GenerateKey(32);
        }

        private String CreateDeviceConnectionString(Microsoft.Azure.Devices.Device device, string iotHubConnectionString)
        {
            StringBuilder deviceConnectionString = new StringBuilder();

            var hostName = String.Empty;
            var tokenArray = iotHubConnectionString.Split(';');
            for (int i = 0; i < tokenArray.Length; i++)
            {
                var keyValueArray = tokenArray[i].Split('=');
                if (keyValueArray[0] == "HostName")
                {
                    hostName = tokenArray[i] + ';';
                    break;
                }
            }

            if (!String.IsNullOrWhiteSpace(hostName))
            {
                deviceConnectionString.Append(hostName);
                deviceConnectionString.AppendFormat("DeviceId={0}", device.Id);

                if (device.Authentication != null)
                {
                    if ((device.Authentication.SymmetricKey != null) && (device.Authentication.SymmetricKey.PrimaryKey != null))
                    {
                        deviceConnectionString.AppendFormat(";SharedAccessKey={0}", device.Authentication.SymmetricKey.PrimaryKey);
                    }
                    else
                    {
                        deviceConnectionString.AppendFormat(";x509=true");
                    }
                }

                //if (this.protocolGatewayHostName.Length > 0)
                //{
                //    deviceConnectionString.AppendFormat(";GatewayHostName=ssl://{0}:8883", this.protocolGatewayHostName);
                //}
            }

            return deviceConnectionString.ToString();
        }
    }
}
