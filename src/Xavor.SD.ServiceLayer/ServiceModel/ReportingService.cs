using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.ServiceLayer.Service;
using Xavor.SD.ServiceLayer.Transformations;

namespace Xavor.SD.ServiceLayer.ServiceModel
{
    public class ReportingService : IReportingService
    {
        private IDevicestatusBL _deviceStatusBL;
        private IDeviceBL _deviceBL;
        private ITransformations _transformation;
        private readonly IAlarmsService _alarmsService;
        private readonly IDeviceAlarmsHistoryBL _deviceAlarmHistoryBL;
        private readonly IDeviceService _deviceService;
        public ReportingService(IDevicestatusBL deviceStatusBL, IDeviceBL deviceBL, ITransformations transformation, IAlarmsService alarmsService, IDeviceAlarmsHistoryBL deviceAlarmHistoryBL, IDeviceService deviceService)
        {
            _deviceStatusBL = deviceStatusBL;
            _deviceBL = deviceBL;
            _transformation = transformation;
            _alarmsService = alarmsService;
            _deviceAlarmHistoryBL = deviceAlarmHistoryBL;
            _deviceService = deviceService;
        }

        public List<DeviceAlarmHistoryReportDTO> GetAlarmHistoryReport(string customerId, string deviceId, string date)
        {
            if (string.IsNullOrEmpty(date))
                date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            else
            {
                var day = DateTime.UtcNow.Day;
                date = Convert.ToDateTime(date).AddDays(day).ToString("yyyy-MM-dd");
            }
            //date = Convert.ToDateTime(date).AddDays(-7).ToString("yyyy-MM-dd");
            var result = _alarmsService.GetDeviceAlarmsHistoryByDate(customerId, deviceId, date).OrderByDescending(o => o.CreatedDate).ToList();
            var alarmReport = _transformation.TranformDevicealarmsHistory(result, "en");
            return alarmReport;
        }

        public List<MaintenanceReportDTO> GetMaintenanceReport(string customerId, string deviceId, string batchId, string date)
        {
            var report = new List<MaintenanceReportDTO>();
            if (customerId != null && batchId != null && deviceId != null && date != null)
            {
                var deviceStatusList = new List<Devicestatus>();
                var deviceList = new List<Device>();
                var device = _deviceBL.GetDeviceByUniqueId(deviceId);
                var deviceStatus = _deviceStatusBL.GetDevicestatusByDeviceId(device.Id);

                deviceStatusList.Add(deviceStatus);
                deviceList.Add(device);
                report = _transformation.TransformDeviceToMaintenanceReport(deviceList, deviceStatusList);


            }
            else if (customerId != null && batchId != null && deviceId != null)
            {
                var deviceStatusList = new List<Devicestatus>();
                var deviceList = new List<Device>();
                var device = _deviceBL.GetDeviceByUniqueId(deviceId);
                var deviceStatus = _deviceStatusBL.GetDevicestatusByDeviceId(device.Id);

                deviceStatusList.Add(deviceStatus);
                deviceList.Add(device);

                report = _transformation.TransformDeviceToMaintenanceReport(deviceList, deviceStatusList);
            }
            else if (customerId != null && batchId != null)
            {
                var deviceList = _deviceBL.GetDevicesByCustomerBatchId(customerId, batchId);
                var deviceStatusList = new List<Devicestatus>();
                foreach (Device deviceObj in deviceList)
                {
                    var deviceStatus = _deviceStatusBL.GetDevicestatusByDeviceId(deviceObj.Id);
                    deviceStatusList.Add(deviceStatus);
                }

                report = _transformation.TransformDeviceToMaintenanceReport(deviceList, deviceStatusList);
            }
            else if (customerId != null)
            {
                var deviceList = _deviceBL.GetDevicesByCustomer(customerId);
                var deviceStatusList = new List<Devicestatus>();
                foreach (Device deviceObj in deviceList)
                {
                    var deviceStatus = _deviceStatusBL.GetDevicestatusByDeviceId(deviceObj.Id);
                    deviceStatusList.Add(deviceStatus);
                }

                report = _transformation.TransformDeviceToMaintenanceReport(deviceList, deviceStatusList);
            }
            else if (deviceId != null)
            {
                var deviceStatusList = new List<Devicestatus>();
                var deviceList = new List<Device>();
                var device = _deviceBL.GetDeviceByUniqueId(deviceId);
                var deviceStatus = _deviceStatusBL.GetDevicestatusByDeviceId(device.Id);
                deviceStatusList.Add(deviceStatus);
                deviceList.Add(device);

                report = _transformation.TransformDeviceToMaintenanceReport(deviceList, deviceStatusList);
            }
            else if (batchId != null)
            {
                var deviceStatusList = new List<Devicestatus>();

                var deviceList = _deviceBL.GetDevicesByBatchId(batchId);
                foreach (Device deviceObj in deviceList)
                {
                    var deviceStatus = _deviceStatusBL.GetDevicestatusByDeviceId(deviceObj.Id);
                    deviceStatusList.Add(deviceStatus);
                }

                report = _transformation.TransformDeviceToMaintenanceReport(deviceList, deviceStatusList);

            }

            else if (date != null)
            {
                var deviceStatusList = new List<Devicestatus>();
                var deviceList = _deviceBL.QueryDevice().ToList();
                foreach (Device deviceObj in deviceList)
                {
                    var deviceStatus = _deviceStatusBL.GetDevicestatusByDeviceId(deviceObj.Id);
                    deviceStatusList.Add(deviceStatus);
                }
                report = _transformation.TransformDeviceToMaintenanceReport(deviceList, deviceStatusList);
                var datetime = Convert.ToDateTime(report[0].LastMaintenanceDate).Month;

                report = report.Where(x => Convert.ToDateTime(x.ExpectedMaintenanceDate).Date.Month.ToString() == Convert.ToDateTime(date).Month.ToString() && Convert.ToDateTime(x.ExpectedMaintenanceDate).Date.Year.ToString() == Convert.ToDateTime(date).Year.ToString()).ToList();
            }
            return report;
        }

        public List<MaintenanceReportDTO> GetDeviceMaintenanceReport(string customerId)
        {
            var deviceList = _deviceBL.GetDevicesByCustomer(customerId);
            var deviceStatusList = new List<Devicestatus>();
            foreach (Device deviceObj in deviceList)
            {
                var deviceStatus = _deviceStatusBL.GetDevicestatusByDeviceId(deviceObj.Id);
                deviceStatusList.Add(deviceStatus);
            }

            var report = _transformation.TransformDeviceToMaintenanceReport(deviceList, deviceStatusList);
            return report;
        }


        public CustomerDeviceMaintenanceReportDTO GetDeviceMaintenanceReport(string customerId, string Date, string Day)
        {
            var pendingDevices = new List<MaintenanceReportDTO>();
            var maintainedDevices = new List<MaintenanceReportDTO>();
            var report = new CustomerDeviceMaintenanceReportDTO();
            var reportResult = GetDeviceMaintenanceReport(customerId);
            for (int i = 0; i < Convert.ToInt32(Day); i++)
            {
                var date = Convert.ToDateTime(Date).AddDays(-i).ToString("yyyy-MM-dd");

                pendingDevices.AddRange(reportResult.Where(x => Convert.ToDateTime(x.ExpectedMaintenanceDate).Date.ToString() == Convert.ToDateTime(date).ToString()).ToList());
                maintainedDevices.AddRange(reportResult.Where(x => x.LastMaintenanceDate != "").Where(x => Convert.ToDateTime(x.LastMaintenanceDate).Date.ToString() == Convert.ToDateTime(date).ToString()).ToList());
            }
            report.maintainedDevice = maintainedDevices;
            report.pendingDevice = pendingDevices;
            return report;

        }

        public double averageEnergyConsumptionPerhour(string deviceId, string Date)
        {
            var device = _deviceBL.GetDeviceDBId(deviceId);
            var deviceAlarm = _deviceAlarmHistoryBL.QueryDevicealarmshistory().Where(x => x.DeviceId == device && Convert.ToDateTime(x.Timestamp).Date.ToString("yyyy-MM-dd") == Date).ToList();
            double averageEnergyConsumptionPerhour = 0;
            for (int i = 0; i < deviceAlarm.Count(); i++)
            {
                var AlarmHistorHourlyBases = deviceAlarm.Where(x => Convert.ToDateTime(x.Timestamp).Hour == deviceAlarm[i].CreatedDate.Value.Hour).ToList();
                double? averagePowerPerHour = 0;
                for (int j = 0; j < AlarmHistorHourlyBases.Count(); j++)
                {
                    averagePowerPerHour = averagePowerPerHour + AlarmHistorHourlyBases[j].OutputPower;
                }

                foreach (Devicealarmshistory alarmObj in AlarmHistorHourlyBases)
                {
                    deviceAlarm.Remove(alarmObj);
                }

                averageEnergyConsumptionPerhour = Convert.ToDouble(averageEnergyConsumptionPerhour + (averagePowerPerHour / 60));
            }
            return averageEnergyConsumptionPerhour;
        }

        public double averageEnergyConsumption(string deviceId, string Date)
        {
            var outputPower = new List<double>();
            double outputPowerAverage = 0;
            var device = _deviceBL.GetDeviceDBId(deviceId);
            var deviceAlarms = _deviceAlarmHistoryBL.QueryDevicealarmshistory().Where(x => x.DeviceId == device).ToList();
            var deviceAlarm = deviceAlarms.Where(x => Convert.ToDateTime(x.Timestamp).Date.ToString("yyyy-MM-dd") == Date).ToList();

            // linq to have distinct hours from deviceAlarm list of that date
            var distantHour = deviceAlarm.GroupBy(x => Convert.ToDateTime(x.Timestamp).Hour).Select(y => y.First()).Distinct();


            //foreach of that distinct hours list

            foreach (var DeviceAlarmHistoryObj in distantHour)
            {
                //get records of that hour from deviceAlarm list, make an average of outputPower and move the result to outpotPower list as a double item

                var deviceAlarmsList = deviceAlarm.Where(x => Convert.ToDateTime(x.Timestamp).Hour == Convert.ToDateTime(DeviceAlarmHistoryObj.Timestamp).Hour);
                double OutputPowerPerhour = 0;
                foreach (var AlarmHistoryObj in deviceAlarmsList)
                {
                    OutputPowerPerhour = Convert.ToDouble(OutputPowerPerhour + AlarmHistoryObj.OutputPower);
                }
                var averageOutputPower = OutputPowerPerhour / deviceAlarmsList.Count();
                outputPower.Add(averageOutputPower);
            }


            outputPower.ForEach(x => outputPowerAverage = outputPowerAverage + x);


            //take the average of outputPower list and move to outputPowerAverage

            //return it to multiple with that day running/usage hours

            return outputPowerAverage;
        }

        public DevicePowerConsumptionReportDTO DeviceConsumptionreport(string customerId, string deviceId, string date)
        {

            var reportObj = new DevicePowerConsumptionReportDTO();
            var SumofAveragePowerPerHour = averageEnergyConsumption(deviceId, date);
            reportObj.PowerConsumption = CalculatePowerConsumption(deviceId, customerId, date, SumofAveragePowerPerHour);
            reportObj.date = date;

            var status = _deviceStatusBL.QueryDevicestatus().Where(x => x.DeviceId == _deviceBL.GetDeviceByUniqueId(deviceId).Id).FirstOrDefault();
            reportObj.deviceId = deviceId;
            reportObj.deviceName = status.Device.Name;
            reportObj.deviceCode = status.Device.DeviceCode;
            reportObj.deviceStatus = Utility.GetStatusMessage((DateTime)status.ModifiedDate);

            return reportObj;
        }

        public string DeviceStatus(string deviceId)
        {
            var status = _deviceStatusBL.QueryDevicestatus().Where(x => x.DeviceId == _deviceBL.GetDeviceByUniqueId(deviceId).Id).FirstOrDefault();
            DateTime modifiedDate = (DateTime)status.ModifiedDate;
            return Utility.GetStatusMessage(modifiedDate);
            //var timeResult = DateTime.UtcNow.Subtract(date1);
            //var minutes = timeResult.TotalMinutes;
            //if (minutes > 10)
            //    return "Offline";
            //else if (minutes < 10 && minutes > 5)
            //    return  "Idle";
            //else
            //    return  "Online";
        }
        public double CalculatePowerConsumption(string deviceId, string customerId, string date, double SumofAveragePowerPerHour)
        {
            var usageReport = _deviceService.GetDeviceUsage(customerId, deviceId, date).FirstOrDefault();
            var runninghour = 0;
            if (usageReport != null)
            {
                runninghour = Convert.ToInt32(usageReport.RunningHours);
            }


            //var averagePowerPerDay = SumofAveragePowerPerHour / 24;
            var PowerConsumptionPerDay = Convert.ToDouble(SumofAveragePowerPerHour * runninghour);
            return Math.Round(Convert.ToDouble(PowerConsumptionPerDay), 2, MidpointRounding.AwayFromZero);
        }

        public List<DevicePowerConsumptionReportDTO> GetConsumptionReport(string customerId, string deviceId, string batchId, string date)
        {
            List<DevicePowerConsumptionReportDTO> report = new List<DevicePowerConsumptionReportDTO>();
            var day = DateTime.UtcNow.Day;
            date = Convert.ToDateTime(date).AddDays(day).ToString("yyyy-MM-dd");
            for (int i = 0; i < 7; i++)
            {
                var Date = Convert.ToDateTime(date).AddDays(-i).ToString("yyyy-MM-dd");
                report.Add(DeviceConsumptionreport(customerId, deviceId, Date));
            }
            return report;



        }
    }
}
