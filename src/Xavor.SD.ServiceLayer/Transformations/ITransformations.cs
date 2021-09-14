using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer.Transformations
{
    public interface ITransformations
    {
        void TransformOperatorDevices(List<DeviceGroupDTO> data, List<Device> d);
        StatusDTO TransformUpdatedDevicestatusToStatusDTO(Devicestatus updatedDevicestatus, StatusDTO existingStatusDTO);
        DevicestatusDTO TransformDevicestatusToStatusDTO(Devicestatus status, string DeviceId);
        DevicestatusManualDTO TransformStatusToManual(Devicestatus status);
        DevicestatusAutoDTO TransformStatusToAuto(Devicestatus status);
        Object TransformToCloudMessage(StatusDTO status, string customerID, string deviceID);
        DeviceDTO TransformDevicestatusDTOToDeviceDTO(DevicestatusDTO deviceStatusDTO, DateTime? modifiedDate, string customerId);
        Commandhistory TransformDefaultSettingToCommandHistory(Defaultsettings defaultsettings);
        Devicestatus TransformDefaultSettingsToDeviceStatus(string deviceUniqueId);
        Commandhistory TransformExistingCommandHistory(bool newPowerStatus, Commandhistory existingCommandhistory, int deviceDbId);

        List<AlarmWarningReportDTO> TransformAlarmHistoryToAlarmHistoryReport(List<Devicealarmshistory> deviceAlarmHistory, List<Device> devices, List<Alarmsandwarnings> alarmsandwarnings);

        List<MaintenanceReportDTO> TransformDeviceToMaintenanceReport(List<Device> devices, List<Devicestatus> devicesStatus);
        List<DeviceAlarmHistoryReportDTO> TranformDevicealarmsHistory(List<Devicealarmshistory> deviceAlarms, string lang);
    }
}
