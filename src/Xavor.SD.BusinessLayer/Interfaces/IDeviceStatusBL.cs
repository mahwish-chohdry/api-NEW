using System.Collections.Generic;
using System.Linq;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IDevicestatusBL
    {
        bool DeleteDevicestatus(int DevicestatusId);
        Devicestatus GetDevicestatus(int Id);
        IEnumerable<Devicestatus> GetDevicestatuss();
        Devicestatus InsertDevicestatus(Devicestatus Devicestatus);
        IQueryable<Devicestatus> QueryDevicestatus();
        Devicestatus UpdateDevicestatus(Devicestatus Devicestatus);
        Devicestatus UpdateDevicestatus(SmartDeviceContainer SmartDevice);
        Devicestatus UpdateDevicestatus(StatusDTO SmartDevice, string Devicestatus = "All");
        Devicestatus GetDevicestatusByDeviceId(int deviceId);
        bool DeleteDevicestatusByDeviceId(int deviceId);

    }
}