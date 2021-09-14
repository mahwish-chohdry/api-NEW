using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.Common;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer
{
    public class IOTHubService : IIOTHubService
    {
        private readonly IConfigurationsBL _configBL;
        private ServiceClient _serviceClient;
        public IOTHubService(IConfigurationsBL configBL)
        {
            _configBL = configBL;
        }
        public void SendCloudtoDeviceMsg(Object message,string deviceId)
        {
            Configurations iotConnectionConfig = _configBL.GetConfiguration(Constants.IOT_HUB_CONNECTION_STRING);
            _serviceClient = ServiceClient.CreateFromConnectionString(iotConnectionConfig.Value);
            string messageString = JsonConvert.SerializeObject(message);
            var commandMessage = new Message(Encoding.ASCII.GetBytes(messageString));
            // commandMessage.Ack = DeliveryAcknowledgement.Full;
            _serviceClient.SendAsync(deviceId, commandMessage);
        }
    }
}
