using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IDevicealarmsBL
    {
        Devicealarms InsertDevicealarms(Devicealarms Devicealarms);
        Devicealarms UpdateDevicealarms(Devicealarms Devicealarms);
        IEnumerable<Devicealarms> GetDevicealarms();
        bool DeleteDevicealarms(int DevicealarmsId);
        Devicealarms GetDevicealarms(int Id);
        IQueryable<Devicealarms> QueryDevicealarms();
        Devicealarms GetDevicealarmsByDeviceId(string deviceId, string customerUniqueId);
        bool DeleteDeviceAlarmsByDeviceId(string deviceId);
    }
}
