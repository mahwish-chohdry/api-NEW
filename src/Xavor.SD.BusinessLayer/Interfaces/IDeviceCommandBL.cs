using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IDevicecommandBL
    {
        Devicecommand InsertDevicecommand(Devicecommand Devicecommand);
        Devicecommand UpdateDevicecommand(Devicecommand Devicecommand);
        IEnumerable<Devicecommand> GetDevicecommand();
        bool DeleteDevicecommand(int DevicecommandId);
        Devicecommand GetDevicecommand(int Id);
        IQueryable<Devicecommand> QueryDevicecommand();
        void AddOrUpdateDevicecommand(Devicecommand deviceCommand);
        Devicecommand GetGroupedDevicecommand(int deviceId);
        bool DeleteDevicecommandByDeviceId(int deviceId);
    }
}
