using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Common.ViewContracts;

namespace Xavor.SD.ServiceLayer
{
    public interface IAlarmsandwarningsService
    {
        List<AlarmsandwarningsDTO> GetAllAlarms();
        List<AlarmsandwarningsDTO> GetAllAlarms(string lang,string invertorId);
        AlarmsandwarningsDTO GetAlarmAndWarningByCode(string code, string lang);
        List<AlarmWarningReportDTO> GetAlarmWarningReport(string Customer,string batchId,string DeviceId,string date , string lang = "en");

    }
}
