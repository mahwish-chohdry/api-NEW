using System.Collections.Generic;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer
{
    public interface IAlarmsService
    {
        List<Devicealarms> InsertNewAlarmsIfNotExistInDatabase(List<InverterAlarms> inverterAlarms);
        string GetLastTimeStamp(string customerId, string deviceId);
        List<Devicealarms> ConvertInvertToDevicealarms(List<InverterAlarms> inverterAlarms, string CustomerId, string DeviceId);
        List<DevicealarmsDTO> TranformDevicealarms(List<Devicealarms> deviceAlarms, string lang);
        List<DevicealarmsHistoryDTO> TranformDevicealarmsHistory(List<Devicealarmshistory> deviceAlarms, string issueType, string lang);

        //List<InverterAlarms> ConvertAlarmsWarnings(List<InverterAlarms> alarms);
        List<Devicealarmshistory> GetDeviceAlarmsHistory(string customerId, string deviceId, string issueType);
        List<Devicealarmshistory> GetDeviceAlarmsHistory(string customerId, string deviceId);
        List<Devicealarmshistory> GetDeviceAlarmsHistoryByDate(string customerId, string deviceId, string lastDate);

    }
}