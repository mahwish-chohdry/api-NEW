using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer.Transformations
{
    public class Transformations : ITransformations
    {
        private readonly IAlarmsandwarningsBL _alarmsandWarningsBL;
        private readonly IDeviceBL _deviceBL;
        private readonly IDefaultSettingsBL _defaultSettingsBL;
        private readonly IDevicestatusBL _deviceStatus;
        private readonly IInverterBL _inverterBL;

        public Transformations(IAlarmsandwarningsBL alarmsandWarningsBL, IDeviceBL deviceBL, IDefaultSettingsBL defaultSettingsBL, IDevicestatusBL deviceStatus, IInverterBL inverterBL)
        {
            _alarmsandWarningsBL = alarmsandWarningsBL;
            _deviceBL = deviceBL;
            _defaultSettingsBL = defaultSettingsBL;
            _deviceStatus = deviceStatus;
            _inverterBL = inverterBL;
        }
        public void TransformOperatorDevices(List<DeviceGroupDTO> data, List<Device> d)
        {
            foreach (var obj in data)
            {
                foreach (var devices in obj.deviceList)
                {
                    if (d.Where(x => x.DeviceId == devices.deviceId).Count() == 0)
                    {
                        devices.hasPermission = false;
                    }

                }

            }
        }

        public StatusDTO TransformUpdatedDevicestatusToStatusDTO(Devicestatus updatedDevicestatus, StatusDTO existingStatusDTO)
        {
            return new StatusDTO()
            {
                DeviceId = existingStatusDTO.DeviceId,
                PowerStatus = updatedDevicestatus.PowerStatus == 1 ? true : false,
                Temp = updatedDevicestatus.Temp,
                Speed = updatedDevicestatus.Speed,
                Humidity = updatedDevicestatus.Humidity,
                AutoTemp = updatedDevicestatus.AutoTemp == 1 ? true : false,
                AutoTimer = updatedDevicestatus.AutoTimer == 1 ? true : false,
                AutoStartTime = updatedDevicestatus.AutoStartTime,
                AutoEndTime = updatedDevicestatus.AutoEndTime,
                HasPreviousSetting = updatedDevicestatus.HasPreviousSetting == 1 ? true : false,
                IdealTemp = updatedDevicestatus.IdealTemp,
                MaintenanceHours = updatedDevicestatus.MaintenanceHours,
                MaxTemp = updatedDevicestatus.MaxTemp,
                MinTemp = updatedDevicestatus.MinTemp,
                OverrideSettings = updatedDevicestatus.OverrideSettings == 1 ? true : false,
                TimeZone = updatedDevicestatus.TimeZone,
                UsageHours = Convert.ToInt32(updatedDevicestatus.RunningTime) / 3600,
                IsExecuted = updatedDevicestatus.IsExecuted == 1 ? true : false,
                CommandType = existingStatusDTO.CommandType,
                CommandId = existingStatusDTO.CommandId,
                error = updatedDevicestatus.Alarm,
                warning = updatedDevicestatus.Warnings,
                CustomerName = existingStatusDTO.CustomerName,
                resetRunningHour = existingStatusDTO.resetRunningHour
            };
        }

        public DevicestatusDTO TransformDevicestatusToStatusDTO(Devicestatus status, string DeviceId)
        {
            var deviceStatus = new DevicestatusDTO();
            //extra query just to get inverter detail - Need refactoring 
            var deviceDetails = _deviceBL.GetDeviceByUniqueId(DeviceId);
            var deviceInverter = new Inverter();
            if (deviceDetails.InverterId != null)
            {
                deviceInverter = _inverterBL.GetInverter(Convert.ToInt32(deviceDetails.InverterId));
            }

            if (status != null)
            {
                deviceStatus.DeviceId = DeviceId;
                deviceStatus.AutoEndTime = status.AutoEndTime;

                deviceStatus.AutoStartTime = status.AutoStartTime;
                deviceStatus.AutoTemp = status.AutoTemp == 1 ? true : false;
                deviceStatus.AutoTimer = status.AutoTimer == 1 ? true : false;
                deviceStatus.CommandType = status.CommandType;
                deviceStatus.HasPreviousSetting = status.HasPreviousSetting == 1 ? true : false;

                deviceStatus.Temp = status.Temp;
                deviceStatus.Humidity = status.Humidity;
                deviceStatus.Pressure = status.Pressure;
                deviceStatus.Iaq = status.Iaq;
                deviceStatus.IaqAccuracy = status.IaqAccuracy;
                deviceStatus.StaticIaq = status.StaticIaq;
                deviceStatus.StaticIaqAccuracy = status.StaticIaqAccuracy;
                deviceStatus.Co2Concentration = status.Co2Concentration;
                deviceStatus.Co2ConcentrationAccuracy = status.Co2ConcentrationAccuracy;
                deviceStatus.VocConcentration = status.VocConcentration;
                deviceStatus.VocConcentrationAccuracy = status.VocConcentrationAccuracy;
                deviceStatus.GasPercentage = status.GasPercentage;
                //if (status.GasPercentage != null) deviceStatus.GasPercentage = Math.Round(Convert.ToDouble(status.GasPercentage), 3, MidpointRounding.AwayFromZero);

                deviceStatus.IdealTemp = status.IdealTemp;
                deviceStatus.IsExecuted = status.IsExecuted == 1 ? true : false;
                deviceStatus.MaintenanceHours = status.MaintenanceHours;
                deviceStatus.MaxTemp = status.MaxTemp;
                deviceStatus.MinTemp = status.MinTemp;
                deviceStatus.OverrideSettings = status.OverrideSettings == 1 ? true : false;
                deviceStatus.PowerStatus = status.PowerStatus == 1 ? true : false;
                deviceStatus.Speed = status.Speed;
                deviceStatus.TimeZone = status.TimeZone;
                deviceStatus.UsageHours = Convert.ToInt32(status.RunningTime) / 3600;

                // Transforming Alarms and warnings into description 
                if (deviceInverter.InverterId == "Holip")
                {
                    //holip error and warning description conversion
                    deviceStatus.alarm = status.Alarm;
                    deviceStatus.warning = status.Warnings;
                    if (status.Alarm != "No Alarm" && status.Alarm != null)
                    {
                        var alarmCode = "E." + deviceStatus.alarm;
                        var alarmWarning = _alarmsandWarningsBL.QueryAlarmsandwarnings().Where(x => x.Code == alarmCode).FirstOrDefault();
                        if (alarmWarning != null) { deviceStatus.alarm = alarmWarning.Description; }

                    }
                    if (status.Warnings != "No Warning" && status.Warnings != null)
                    {
                        var warningCode = "A." + deviceStatus.warning;
                        var alarmWarning = _alarmsandWarningsBL.QueryAlarmsandwarnings().Where(x => x.Code == warningCode).FirstOrDefault();
                        if (alarmWarning != null) { deviceStatus.warning = alarmWarning.Description; }

                    }
                }
                else if (deviceInverter.InverterId == "Schneider")
                {
                    //Schneider error and warning description conversion
                    if (status.Alarm != "No Alarm" && status.Alarm != null)
                    {
                        var alarmCode = Convert.ToInt32(status.Alarm);
                        var AlarmDBCode = Enum.ToObject(typeof(ScheniederAlarms), alarmCode).ToString();

                        var alarm = _alarmsandWarningsBL.QueryAlarmsandwarnings().Where(x => x.InverterId == deviceInverter.Id && x.Code == AlarmDBCode).FirstOrDefault();

                        deviceStatus.alarm = alarm.Description;


                    }

                }

                DateTime date1 = (DateTime)status.ModifiedDate;

                var timeResult = DateTime.UtcNow.Subtract(date1);
                var minutes = timeResult.TotalMinutes;
            }
            return deviceStatus;
        }

        public DevicestatusManualDTO TransformStatusToManual(Devicestatus status)
        {
            DevicestatusManualDTO result = new DevicestatusManualDTO();

            result.CommandType = "manual";
            result.PowerStatus = status.PowerStatus;
            result.Speed = status.Speed;
            result.UsageHour = Convert.ToInt32(status.RunningTime) / 3600;
            result.Maintainence = status.MaintenanceHours;

            return result;
        }

        public DevicestatusAutoDTO TransformStatusToAuto(Devicestatus status)
        {
            DevicestatusAutoDTO result = new DevicestatusAutoDTO();
            result.AutoEndTime = status.AutoEndTime;
            result.AutoStartTime = status.AutoStartTime;
            result.AutoTemp = status.AutoTemp;
            result.AutoTimer = status.AutoTimer;
            result.IdealTemp = status.IdealTemp;
            result.MaxTemp = status.IdealTemp + 2;
            result.MinTemp = status.IdealTemp - 2;
            result.Maintainence = status.MaintenanceHours;
            result.UsageHour = Convert.ToInt32(status.RunningTime) / 3600;

            result.CommandType = "auto";

            return result;
        }

        public Object TransformToCloudMessage(StatusDTO status, string customerID, string deviceID)
        {
            //again Query the device object could be passed from the calling function need refactoring
            var device = _deviceBL.QueryDevice().Where(x => x.DeviceId == deviceID).FirstOrDefault();
            if (status.CommandType.ToLower().Contains("auto"))
            {
                DevicestatusAutoDTO autostatus = new DevicestatusAutoDTO();
                autostatus.CommandType = "auto";
                autostatus.CustomerId = customerID;
                autostatus.DeviceId = deviceID;
                autostatus.AutoTimer = status.AutoTimer == true ? 1 : 0;
                autostatus.AutoTemp = status.AutoTemp == true ? 1 : 0;
                autostatus.AutoEndTime = status.AutoEndTime;
                autostatus.AutoStartTime = status.AutoStartTime;
                autostatus.IdealTemp = status.IdealTemp;
                autostatus.MaxTemp = status.MaxTemp;
                autostatus.MinTemp = status.MinTemp;
                autostatus.CommandId = status.CommandId;
                autostatus.Maintainence = status.MaintenanceHours;
                autostatus.UsageHour = status.UsageHours;

                //Version Info
                autostatus.CurrentFirmwareVersion = device.CurrentFirmwareVersion;
                autostatus.LatestFirmwareVersion = device.LatestFirmwareVersion;

                //InverterId 
                if (device.InverterId != null)
                    autostatus.InverterId = _inverterBL.GetInverter(Convert.ToInt32(device.InverterId)).InverterId;

                return autostatus;
            }
            else
            {
                DevicestatusManualDTO manualStatus = new DevicestatusManualDTO();
                manualStatus.CustomerId = customerID;
                manualStatus.DeviceId = deviceID;
                manualStatus.CommandType = "manual";
                manualStatus.PowerStatus = status.PowerStatus == true ? 1 : 0;
                manualStatus.Speed = status.Speed == 0 ? 1 : status.Speed;
                manualStatus.CommandId = status.CommandId;
                manualStatus.Maintainence = status.MaintenanceHours;
                manualStatus.UsageHour = status.UsageHours;

                //Version Info
                manualStatus.CurrentFirmwareVersion = device.CurrentFirmwareVersion;
                manualStatus.LatestFirmwareVersion = device.LatestFirmwareVersion;

                //InverterId
                if (device.InverterId != null)
                {
                    manualStatus.InverterId = _inverterBL.GetInverter(Convert.ToInt32(device.InverterId)).InverterId;
                }
                return manualStatus;
            }
        }

        public DeviceDTO TransformDevicestatusDTOToDeviceDTO(DevicestatusDTO deviceStatusDTO, DateTime? modifiedDate, string customerId)
        {
            var deviceDetails = _deviceBL.GetDeviceByUniqueId(deviceStatusDTO.DeviceId);
            var deviceInverter = new Inverter();
            if (deviceDetails.InverterId != null)
            {
                deviceInverter = _inverterBL.GetInverter(Convert.ToInt32(deviceDetails.InverterId));
            }
            var deviceDTO = new DeviceDTO();
            deviceDTO.deviceName = deviceDetails.Name;
            deviceDTO.customerId = customerId;
            deviceDTO.deviceId = deviceDetails.DeviceId;
            deviceDTO.hasPermission = true;
            deviceDTO.IsConfigured = deviceDetails.IsInstalled == 1 ? true : false;
            deviceDTO.apSsid = deviceDetails.Apssid;
            deviceDTO.apPassword = deviceDetails.Appassword;
            deviceDTO.deviceStatus = deviceStatusDTO;
            deviceDTO.currentFirmwareVersion = deviceDetails.CurrentFirmwareVersion;
            deviceDTO.latestFirmwareVersion = deviceDetails.LatestFirmwareVersion;
            deviceDTO.inverterId = deviceInverter.InverterId;
            deviceDTO.connectivityStatus = Utility.GetStatusMessage((DateTime)modifiedDate);
            //var timeResult = DateTime.UtcNow.Subtract(date1);
            //var minutes = timeResult.TotalMinutes;
            //if (minutes > 10)
            //{
            //    deviceDTO.connectivityStatus = "Offline";
            //}
            //else if (minutes < 10 && minutes > 5)
            //{
            //    deviceDTO.connectivityStatus = "Idle";
            //}
            //else
            //{
            //    deviceDTO.connectivityStatus = "Online";
            //}


            return deviceDTO;
        }
        public Devicestatus TransformDefaultSettingsToDeviceStatus(string deviceUniqueId)
        {
            int deviceId = _deviceBL.GetDeviceDBId(deviceUniqueId);

            var defaultSettingsStatus = _defaultSettingsBL.QueryDefaultsettings().FirstOrDefault();
            var newDeviceStatus = new Devicestatus();
            newDeviceStatus.DeviceId = deviceId;
            newDeviceStatus.IdealTemp = defaultSettingsStatus.IdealTemp.Value;
            newDeviceStatus.AutoStartTime = defaultSettingsStatus.AutoStartTime;
            newDeviceStatus.AutoEndTime = defaultSettingsStatus.AutoEndTime;
            newDeviceStatus.MaintenanceHours = defaultSettingsStatus.MaintenanceHours.Value;
            newDeviceStatus.AutoTemp = defaultSettingsStatus.AutoTemp;
            newDeviceStatus.AutoTimer = defaultSettingsStatus.AutoTimer;
            newDeviceStatus.HasPreviousSetting = defaultSettingsStatus.HasPreviousSetting;
            newDeviceStatus.OverrideSettings = defaultSettingsStatus.OverrideSettings;
            newDeviceStatus.TimeZone = defaultSettingsStatus.TimeZone;
            newDeviceStatus.Speed = defaultSettingsStatus.Speed.Value;
            newDeviceStatus.PowerStatus = defaultSettingsStatus.PowerStatus;
            newDeviceStatus.MinTemp = defaultSettingsStatus.IdealTemp.Value - 2;
            newDeviceStatus.MaxTemp = defaultSettingsStatus.IdealTemp.Value + 2;
            newDeviceStatus.IsExecuted = 0;
            newDeviceStatus.CommandType = "default";
            newDeviceStatus.ModifiedDate = DateTime.UtcNow;
            newDeviceStatus.Alarm = "No Alarm";
            newDeviceStatus.Warnings = "No Warning";

            return newDeviceStatus;
        }

        public Commandhistory TransformDefaultSettingToCommandHistory(Defaultsettings groupSetting)
        {
            Commandhistory commandhistory = new Commandhistory
            {
                PowerStatus = groupSetting.PowerStatus,
                Temp = 0,
                HasPreviousSetting = groupSetting.HasPreviousSetting,
                OverrideSettings = groupSetting.OverrideSettings,
                AutoEndTime = groupSetting.AutoEndTime,
                AutoStartTime = groupSetting.AutoStartTime,
                IdealTemp = groupSetting.IdealTemp.Value,
                AutoTemp = groupSetting.AutoTemp.Value,
                AutoTimer = groupSetting.AutoTimer.Value,
                Speed = groupSetting.Speed.Value,
                TimeZone = groupSetting.TimeZone,
                Humidity = 0,
                MaintenanceHours = groupSetting.MaintenanceHours.Value,
                UsageHours = 0,
                IsExecuted = 0,
                CommandId = "0",
                CommandType = "default",
                MaxTemp = groupSetting.IdealTemp.Value + 5,
                MinTemp = groupSetting.IdealTemp.Value - 5,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
            };
            return commandhistory;
        }

        public Commandhistory TransformExistingCommandHistory(bool newPowerStatus, Commandhistory existingCommandhistory, int deviceDbId)
        {
            return new Commandhistory()
            {
                DeviceId = deviceDbId,
                PowerStatus = Convert.ToByte(newPowerStatus),
                Temp = existingCommandhistory.Temp,
                Speed = existingCommandhistory.Speed,
                Humidity = existingCommandhistory.Humidity,
                AutoTemp = Convert.ToByte(existingCommandhistory.AutoTemp),
                AutoTimer = Convert.ToByte(existingCommandhistory.AutoTimer),
                AutoStartTime = existingCommandhistory.AutoStartTime,
                AutoEndTime = existingCommandhistory.AutoEndTime,
                HasPreviousSetting = Convert.ToByte(existingCommandhistory.HasPreviousSetting),
                IdealTemp = existingCommandhistory.IdealTemp,
                MaintenanceHours = existingCommandhistory.MaintenanceHours,
                MaxTemp = existingCommandhistory.MaxTemp,
                MinTemp = existingCommandhistory.MinTemp,
                OverrideSettings = Convert.ToByte(existingCommandhistory.OverrideSettings),
                TimeZone = existingCommandhistory.TimeZone,
                UsageHours = existingCommandhistory.UsageHours,
                IsExecuted = 0,
                CommandType = existingCommandhistory.CommandType,
                //CommandId,CreatedDate,Device,Devicecommand,Group,Groupcommand,GroupId,Id,ModifiedDate
            };
        }

        public List<AlarmWarningReportDTO> TransformAlarmHistoryToAlarmHistoryReport(List<Devicealarmshistory> deviceAlarmHistory, List<Device> devices, List<Alarmsandwarnings> alarmsandwarnings)
        {
            var alarmWarningReport = new List<AlarmWarningReportDTO>();
            try
            {


                int count = 0;
                foreach (Devicealarmshistory devicealarmhistoryObj in deviceAlarmHistory)
                {
                    count++;
                    var alarmWarningCode = new Alarmsandwarnings();
                    var alarmWarningReportObj = new AlarmWarningReportDTO();
                    // var device = devices.Where(x => x.Id == devicealarmhistoryObj.DeviceId).FirstOrDefault();
                    //   var deviceStatus = _deviceStatus.GetDevicestatusByDeviceId(device.Id);

                    if (devicealarmhistoryObj.Alarm != "No Alarm")
                    {
                        alarmWarningCode = alarmsandwarnings.Where(x => x.Code == ("E." + devicealarmhistoryObj.Alarm)).FirstOrDefault();
                    }
                    else
                    {
                        alarmWarningCode = alarmsandwarnings.Where(x => x.Code == ("A." + devicealarmhistoryObj.Warning)).FirstOrDefault();
                    }
                    if (alarmWarningCode != null)
                    {

                        // Alarm or Warning Description from static data
                        alarmWarningReportObj.Code = alarmWarningCode.Code;
                        alarmWarningReportObj.Description = alarmWarningCode.Description;
                        alarmWarningReportObj.ReasonAnalysis = alarmWarningCode.ReasonAnalysis;
                        alarmWarningReportObj.RegisterNumber = alarmWarningCode.RegisterNumber.Value;
                        alarmWarningReportObj.Type = alarmWarningCode.Type;
                    }


                    //device Alarm Data
                    var device = devices.Where(w => w.Id == devicealarmhistoryObj.DeviceId).FirstOrDefault();
                    alarmWarningReportObj.timestamp = devicealarmhistoryObj.CreatedDate.ToString();
                    alarmWarningReportObj.DeviceId = device.DeviceId;
                    alarmWarningReportObj.DeviceName = device.Name;
                    alarmWarningReportObj.DeviceCode = device.DeviceCode;
                    //var deviceStatus = _deviceStatus.GetDevicestatusByDeviceId(device.Id);
                    //DateTime modifiedDate = (DateTime)deviceStatus.ModifiedDate;
                    //alarmWarningReportObj.DeviceStatus = Utility.GetStatusMessage(modifiedDate);

                    alarmWarningReport.Add(alarmWarningReportObj);

                }


                foreach (AlarmWarningReportDTO AlarmWarningReportObj in alarmWarningReport)
                {
                    var device = devices.Where(w => w.DeviceId == AlarmWarningReportObj.DeviceId).FirstOrDefault();
                    var deviceStatus = _deviceStatus.GetDevicestatusByDeviceId(device.Id);
                    //device data
                    DateTime modifiedDate = (DateTime)deviceStatus.ModifiedDate;
                    AlarmWarningReportObj.DeviceStatus = Utility.GetStatusMessage(modifiedDate);
                    //var timeResult = DateTime.UtcNow.Subtract(date1);
                    //var minutes = timeResult.TotalMinutes;
                    //if (minutes > 10)
                    //    AlarmWarningReportObj.DeviceStatus = "Offline";
                    //else if (minutes < 10 && minutes > 5)
                    //    AlarmWarningReportObj.DeviceStatus = "Idle";
                    //else
                    //    AlarmWarningReportObj.DeviceStatus = "Online";
                }
                return alarmWarningReport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MaintenanceReportDTO> TransformDeviceToMaintenanceReport(List<Device> devices, List<Devicestatus> devicesStatus)
        {
            var report = new List<MaintenanceReportDTO>();
            foreach (Device deviceObj in devices)
            {
                var status = devicesStatus.Where(x => x.DeviceId == deviceObj.Id).FirstOrDefault();
                var maintenanceReport = new MaintenanceReportDTO();
                // device attribute
                maintenanceReport.DeviceId = deviceObj.DeviceId;
                maintenanceReport.DeviceName = deviceObj.Name;
                maintenanceReport.LastMaintenanceDate = deviceObj.LastMaintenanceDate.ToString();

                // Device Status attribute
                DateTime modifiedDate = (DateTime)status.ModifiedDate;
                maintenanceReport.DeviceStatus = Utility.GetStatusMessage(modifiedDate);
                //var timeResult = DateTime.UtcNow.Subtract(date1);
                //var minutes = timeResult.TotalMinutes;
                //if (minutes > 10)
                //    maintenanceReport.DeviceStatus = "Offline";
                //else if (minutes < 10 && minutes > 5)
                //    maintenanceReport.DeviceStatus = "Idle";
                //else
                //    maintenanceReport.DeviceStatus = "Online";

                maintenanceReport.Temperature = status.Temp.ToString();
                maintenanceReport.RunningHours = Convert.ToString(Convert.ToInt32(status.RunningTime) / 3600);
                maintenanceReport.MaintenanceHour = Convert.ToString(status.MaintenanceHours);


                // predictive maintenenace paramters

                //average Running Hours 
                var totalRunningHour = Convert.ToInt32(status.TotalRunningTime) / 3600;
                var totalDays = DateTime.UtcNow.Subtract((DateTime)deviceObj.CreatedDate).Days;

                var averageHours = 0.0;
                if (totalDays != 0)
                {
                    averageHours = totalRunningHour / totalDays;
                    if (averageHours <= 0)
                    {
                        maintenanceReport.AverageUsagehour = "1";
                    }
                    else
                        maintenanceReport.AverageUsagehour = Convert.ToString(averageHours);
                }
                else
                    maintenanceReport.AverageUsagehour = totalRunningHour.ToString();

                //expected maintenance day

                var remainingMaintenanceHour = status.MaintenanceHours - Convert.ToInt32(maintenanceReport.RunningHours);
                var daysRequired = 0;
                if (Convert.ToInt32(maintenanceReport.AverageUsagehour) != 0 && remainingMaintenanceHour > 0)
                {
                    daysRequired = remainingMaintenanceHour / Convert.ToInt32(maintenanceReport.AverageUsagehour);
                }

                maintenanceReport.ExpectedMaintenanceDate = DateTime.UtcNow.AddDays(daysRequired).ToString();

                report.Add(maintenanceReport);
            }
            return report;
        }

        public List<DeviceAlarmHistoryReportDTO> TranformDevicealarmsHistory(List<Devicealarmshistory> deviceAlarms, string lang)
        {
            List<DeviceAlarmHistoryReportDTO> newDeviceAlarmList = new List<DeviceAlarmHistoryReportDTO>();
            var possibleAlarmsAndWarnings = _alarmsandWarningsBL.GetAlarmsandwarnings();
            foreach (var obj in deviceAlarms)
            {
                DeviceAlarmHistoryReportDTO newDeviceAlarm = new DeviceAlarmHistoryReportDTO();
                // PLC data
                newDeviceAlarm.Timestamp = obj.Timestamp;
                newDeviceAlarm.OutputCurrent = obj.OutputCurrent;
                newDeviceAlarm.OutputFrequency = obj.OutputFrequency;
                newDeviceAlarm.OutputPower = obj.OutputPower;
                newDeviceAlarm.OutputVoltage = obj.OutputVoltage;
                newDeviceAlarm.Rpm = obj.Rpm;
                newDeviceAlarm.Direction = obj.Direction;
                newDeviceAlarm.Speed = obj.Speed;
                if (obj.OutputCurrent != null)
                    newDeviceAlarm.OutputCurrent = Math.Round(Convert.ToDouble(obj.OutputCurrent), 2, MidpointRounding.AwayFromZero);
                if (obj.OutputFrequency != null)
                    newDeviceAlarm.OutputFrequency = Math.Round(Convert.ToDouble(obj.OutputFrequency), 2, MidpointRounding.AwayFromZero);
                if (obj.OutputPower != null)
                    newDeviceAlarm.OutputPower = Math.Round(Convert.ToDouble(obj.OutputPower), 2, MidpointRounding.AwayFromZero);
                if (obj.OutputVoltage != null)
                    newDeviceAlarm.OutputVoltage = Math.Round(Convert.ToDouble(obj.OutputVoltage), 2, MidpointRounding.AwayFromZero);
                if (obj.Rpm != null)
                    newDeviceAlarm.Rpm = Math.Round(Convert.ToDouble(obj.Rpm), 2, MidpointRounding.AwayFromZero);

                if (obj.Alarm != "No Alarm" && obj.Alarm != null)
                {
                    newDeviceAlarm.Code = "E." + obj.Alarm;
                    var alarmWarning = possibleAlarmsAndWarnings.Where(x => x.Language == lang && x.Code == newDeviceAlarm.Code).FirstOrDefault();


                    if (alarmWarning != null)
                    {
                        newDeviceAlarm.Title = alarmWarning.Description;
                        newDeviceAlarm.ReasonAnalysis = alarmWarning.ReasonAnalysis;
                        newDeviceAlarm.Timestamp = obj.Timestamp;
                        // newDeviceAlarmList.Add(newDeviceAlarm);
                    }

                }
                else if (obj.Warning != null && obj.Warning != "No Warning")
                {
                    newDeviceAlarm.Code = "A." + obj.Warning;
                    var alarmWarning = possibleAlarmsAndWarnings.Where(x => x.Language == lang && x.Code == newDeviceAlarm.Code).FirstOrDefault();
                    if (alarmWarning != null)
                    {
                        newDeviceAlarm.Title = alarmWarning.Description;
                        newDeviceAlarm.ReasonAnalysis = alarmWarning.ReasonAnalysis;
                        newDeviceAlarm.Timestamp = obj.Timestamp;
                        //  newDeviceAlarmList.Add(newDeviceAlarm);
                    }
                }
                newDeviceAlarmList.Add(newDeviceAlarm);
            }
            return newDeviceAlarmList;
        }
    }
}
