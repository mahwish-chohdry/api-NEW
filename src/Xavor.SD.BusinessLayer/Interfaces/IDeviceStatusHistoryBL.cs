using System.Collections.Generic;
using System.Linq;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IDevicestatushistoryBL
    {
        Devicestatushistory InsertDevicestatushistory(Devicestatushistory Devicestatushistory);
        IQueryable<Devicestatushistory> QueryDevicestatushistory();
        void AddOrUpdateDevicestatushistory(int deviceId, SmartDeviceContainer SmartDevice, int totalSeconds);
        Devicestatushistory UpdateDevicestatushistory(Devicestatushistory Devicestatushistory);
        bool DeleteDeviceStatusHistoryByDeviceId(int deviceId);
    }
}
