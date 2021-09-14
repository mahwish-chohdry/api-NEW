using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IDeviceAlarmsHistoryBL
    {
        Devicealarmshistory InsertDevicealarmshistory(Devicealarmshistory Devicealarmshistory);
        Devicealarmshistory UpdateDevicealarmshistory(Devicealarmshistory Devicealarmshistory);
        IEnumerable<Devicealarmshistory> GetDevicealarmshistory();
        bool DeleteDevicealarmshistory(int DevicealarmshistoryId);
        Devicealarmshistory GetDevicealarmshistory(int Id);
        IQueryable<Devicealarmshistory> QueryDevicealarmshistory();
        List<Devicealarmshistory> GetDevicealarmshistoryByIssueType(int deviceId, string issueType);
        bool DeleteDeviceAlarmshistoryByDeviceId(int deviceId);
        List<Devicealarmshistory> GetAlarmWarningReport(int deviceId, string customerId);
    }
}
