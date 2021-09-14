using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xavor.SD.Common.ViewContracts;


namespace Xavor.SD.ServiceLayer
{
    public interface ISmartDeviceContainerService
    {
        Task<List<DevicealarmsDTO>> GetDevicealarms(string customerId, string deviceId);
    }
}
