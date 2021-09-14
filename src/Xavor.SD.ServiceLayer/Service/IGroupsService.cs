using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Common;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.ServiceLayer
{
    public interface IGroupsService
    {
        void AddDevices(int groupId, List<string> deviceUniqueIdList, string adminUserId);
        string SaveGroup(UserDeviceDTO userDevices, string customerUniqueId, string adminUserId);
        bool SaveGroup(UserDeviceDTO userDevices, string customerUniqueId, string adminUserId, string groupId);

    }
}
