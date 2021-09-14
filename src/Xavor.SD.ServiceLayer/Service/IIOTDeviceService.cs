using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Security;
using Xavor.SD.BusinessLayer;
using Xavor.SD.Common;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer
{
    public interface IIOTDeviceService
    {
       // ResponseDTO GetSasToken(string deviceId,bool isPostStatus);
          Task<string> CreateDevice(string deviceId,string inverterId = null, bool symetricKeys = true);


    }
}
