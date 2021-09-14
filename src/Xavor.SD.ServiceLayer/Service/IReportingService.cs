using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer.Service
{
    public interface IReportingService
    {
        List<MaintenanceReportDTO> GetMaintenanceReport(string customerId, string deviceId, string batchId, string Date);

        CustomerDeviceMaintenanceReportDTO GetDeviceMaintenanceReport(string customerId, string Date,string Day);
        List<DeviceAlarmHistoryReportDTO> GetAlarmHistoryReport(string customerId, string deviceId, string Date);

        List<DevicePowerConsumptionReportDTO> GetConsumptionReport(string customerId, string deviceId, string batchId, string Date);
    }
}
