using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer.Interfaces
{
    public interface IUserdeviceBL
    {
        bool DeleteUserDevice(int UserDeviceId);
        bool DeleteUserDevicesByUserId(int UserId);
        IEnumerable<Userdevice> GetUserDevice();
        Userdevice InsertUserDevice(Userdevice userdevice);
        IQueryable<Userdevice> QueryDevice();
        Userdevice UpdateUserDevice(Userdevice userdevice);
        bool UserDevice(int userdeviceId);
        IQueryable<Userdevice> GetUserDeviceMappingsByUser(int userId);
        bool DeleteUserDeviceByDeviceId(int deviceId);

    }
}
