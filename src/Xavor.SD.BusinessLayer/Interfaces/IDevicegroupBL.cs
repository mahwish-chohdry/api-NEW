using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IDevicegroupBL
    {
        Devicegroup InsertDevicegroup(Devicegroup Devicegroup);
        Devicegroup UpdateDevicegroup(Devicegroup Devicegroup);
        IEnumerable<Devicegroup> GetDevicegroup();
        bool DeleteDevicegroup(int DevicegroupId);
        Devicegroup GetDevicegroup(int Id);
        IQueryable<Devicegroup> QueryDevicegroup();
        bool DeleteDeviceGroupByGroupId(int group_id);
        bool RemoveDeviceFromGroup(int group_id, List<int> device_ids);
        bool DeleteDeviceGroupByDeviceId(int deviceId);
    }
}
