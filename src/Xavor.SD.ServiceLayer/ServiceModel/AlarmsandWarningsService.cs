using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.BusinessLayer;
using Xavor.SD.Model;
using System.Linq;
using Xavor.SD.ServiceLayer.Transformations;

namespace Xavor.SD.ServiceLayer
{
    public class AlarmsandwarningsService : IAlarmsandwarningsService
    {
        private IAlarmsandwarningsBL _alarmsBL;
        private IDeviceAlarmsHistoryBL _deviceAlarmsHistoryBL;
        private IDeviceBL _deviceBL;
        private ITransformations _transformation;


        public AlarmsandwarningsService(IAlarmsandwarningsBL alarmsBL, IDeviceAlarmsHistoryBL deviceAlarmsHistoryBL, IDeviceBL deviceBL, ITransformations transformation)
        {
            _alarmsBL = alarmsBL;
            _deviceAlarmsHistoryBL = deviceAlarmsHistoryBL;
            _deviceBL = deviceBL;
            _transformation = transformation;
        }


        // Need refactoring
        public List<AlarmWarningReportDTO> GetAlarmWarningReport(string customerId, string batchId, string deviceId, string date,string lang = "en")
        {
            var alarmwarningCode = _alarmsBL.GetAlarmsandwarnings().Where(x=>x.Language == lang).ToList();
            var report = new List<AlarmWarningReportDTO>();
            if (customerId != null && batchId != null && deviceId != null && date != null)
            {
                var device = _deviceBL.GetDeviceByUniqueId(deviceId);
                var Querydata = _deviceAlarmsHistoryBL.QueryDevicealarmshistory().Where(x => x.DeviceId == device.Id && x.CreatedDate.Value.Date.Month.ToString() == Convert.ToDateTime( date).Date.Month.ToString() && x.CreatedDate.Value.Date.Year.ToString() == Convert.ToDateTime(date).Date.Year.ToString());
                var alarmHistory = Querydata.Where(x => x.Alarm != "No Alarm" || x.Warning != "No Warning").ToList();
                var deviceList = new List<Device>();
                deviceList.Add(device);
                report = _transformation.TransformAlarmHistoryToAlarmHistoryReport(alarmHistory, deviceList, alarmwarningCode);
            }
            else if (customerId != null && batchId != null && deviceId != null)
            {
                var device = _deviceBL.GetDeviceByUniqueId(deviceId);
                var Querydata = _deviceAlarmsHistoryBL.QueryDevicealarmshistory().Where(x => x.DeviceId == device.Id);
                var alarmHistory = Querydata.Where(x => x.Alarm != "No Alarm" || x.Warning != "No Warning").ToList();
                var deviceList = new List<Device>();
                deviceList.Add(device);
                report = _transformation.TransformAlarmHistoryToAlarmHistoryReport(alarmHistory, deviceList, alarmwarningCode);
            }
            else if (customerId != null && batchId != null)
            {
                var device = _deviceBL.GetDevicesByCustomerBatchId(customerId, batchId);
                var alarmHistory = new List<Devicealarmshistory>();

                foreach (Device deviceObj in device)
                {
                    var alarmHistoryQuery = _deviceAlarmsHistoryBL.QueryDevicealarmshistory().Where(x => x.DeviceId == deviceObj.Id);
                    var alarmHistoryObj = alarmHistoryQuery.Where(x => x.Alarm != "No Alarm" || x.Warning != "No Warning").ToList();
                    if (alarmHistoryObj.Count != 0)
                    {
                        alarmHistory.AddRange(alarmHistoryObj);
                    }

                }
                report = _transformation.TransformAlarmHistoryToAlarmHistoryReport(alarmHistory, device, alarmwarningCode);
            }
            else if (customerId != null && date != null)
            {
                var device = _deviceBL.GetDevicesByCustomer(customerId);
                //var test = _deviceAlarmsHistoryBL.GetAlarmWarningReport(device[0].Id,customerId);

                var alarmHistory = new List<Devicealarmshistory>();
                foreach (Device deviceObj in device)
                {
                    var alarmHistoryQuery = _deviceAlarmsHistoryBL.QueryDevicealarmshistory().Where(x => x.DeviceId == deviceObj.Id && x.CreatedDate.Value.Date.Month.ToString() == Convert.ToDateTime(date).Date.Month.ToString() && x.CreatedDate.Value.Date.Year.ToString() == Convert.ToDateTime(date).Date.Year.ToString());
                    var alarmHistoryObj = alarmHistoryQuery.Where(x => x.Alarm != "No Alarm" || x.Warning != "No Warning").ToList();
                    if (alarmHistoryObj.Count != 0)
                    {
                        alarmHistory.AddRange(alarmHistoryObj);
                    }
                }

                report = _transformation.TransformAlarmHistoryToAlarmHistoryReport(alarmHistory, device, alarmwarningCode);
            }
            else if (customerId != null)
            {
                var device = _deviceBL.GetDevicesByCustomer(customerId);
                var alarmHistory = new List<Devicealarmshistory>();
                foreach (Device deviceObj in device)
                {
                    var alarmHistoryQuery = _deviceAlarmsHistoryBL.QueryDevicealarmshistory().Where(x => x.DeviceId == deviceObj.Id);
                    var alarmHistoryObj = alarmHistoryQuery.Where(x => x.Alarm != "No Alarm" || x.Warning != "No Warning").ToList();
                    if (alarmHistoryObj.Count != 0)
                    {
                        alarmHistory.AddRange(alarmHistoryObj);
                    }
                }

                report = _transformation.TransformAlarmHistoryToAlarmHistoryReport(alarmHistory, device, alarmwarningCode);
            }
            else if (deviceId != null)
            {
                var device = _deviceBL.GetDeviceByUniqueId(deviceId);
                var Querydata = _deviceAlarmsHistoryBL.QueryDevicealarmshistory().Where(x => x.DeviceId == device.Id);
                var alarmHistory = Querydata.Where(x => x.Alarm != "No Alarm" || x.Warning != "No Warning").ToList();
                var deviceList = new List<Device>();
                deviceList.Add(device);
                report = _transformation.TransformAlarmHistoryToAlarmHistoryReport(alarmHistory, deviceList, alarmwarningCode);
            }
            else if (batchId != null)
            {
                var device = _deviceBL.GetDevicesByBatchId(batchId);
                var alarmHistory = new List<Devicealarmshistory>();
                foreach (Device deviceObj in device)
                {
                    var alarmHistoryQuery = _deviceAlarmsHistoryBL.QueryDevicealarmshistory().Where(x => x.DeviceId == deviceObj.Id);
                    var alarmHistoryObj = alarmHistoryQuery.Where(x => x.Alarm != "No Alarm" || x.Warning != "No Warning").ToList();
                    if (alarmHistoryObj.Count != 0)
                    {
                        alarmHistory.AddRange(alarmHistoryObj);
                    }
                }
                report = _transformation.TransformAlarmHistoryToAlarmHistoryReport(alarmHistory, device, alarmwarningCode);
            }

            else if (date != null)
            {
                var data = _deviceAlarmsHistoryBL.QueryDevicealarmshistory().Where(x => x.CreatedDate.Value.Date.Month.ToString() == Convert.ToDateTime(date).Date.Month.ToString()&& x.CreatedDate.Value.Date.Year.ToString() == Convert.ToDateTime(date).Date.Year.ToString());
                var alarmHistory = data.Where(x => x.Alarm != "No Alarm" || x.Warning != "No Warning").ToList();
                var deviceList = new List<Device>();
                foreach (Devicealarmshistory alarmObj in alarmHistory)
                {
                    if (deviceList.Where(x => x.Id == alarmObj.DeviceId).FirstOrDefault() == null)
                    {
                        deviceList.Add(_deviceBL.GetDevice(alarmObj.DeviceId));
                    }
                }



                report = _transformation.TransformAlarmHistoryToAlarmHistoryReport(alarmHistory, deviceList, alarmwarningCode);
            }
            return report;
        }

        public List<AlarmsandwarningsDTO> GetAllAlarms()
        {
            IEnumerable<Alarmsandwarnings> Alarms = _alarmsBL.GetAlarmsandwarnings();
            List<AlarmsandwarningsDTO> AlarmsDtos = new List<AlarmsandwarningsDTO>();

            foreach (var alarm in Alarms)
            {
                AlarmsandwarningsDTO dto = new AlarmsandwarningsDTO();
                dto.Code = alarm.Code;
                dto.Description = alarm.Description;
                dto.Type = alarm.Type;
                dto.ReasonAnalysis = alarm.ReasonAnalysis;
                dto.RegisterNumber = alarm.RegisterNumber.Value;
                dto.timestamp = alarm.Timestamp;
                AlarmsDtos.Add(dto);
            }
            return AlarmsDtos;
        }

        public List<AlarmsandwarningsDTO> GetAllAlarms(string lang, string inverterId)
        {
            IEnumerable<Alarmsandwarnings> Alarms;
            if (inverterId != null)
            {
                var inverter = Validations.Validator.ValidateInverter(inverterId);
                 Alarms = _alarmsBL.GetAlarmsandwarnings(lang, inverter.Id);

            }
            else
            {
               
                Alarms = _alarmsBL.GetAlarmsandwarnings(lang);
            }
           
            List<AlarmsandwarningsDTO> AlarmsDtos = new List<AlarmsandwarningsDTO>();

            foreach (var alarm in Alarms)
            {
                AlarmsandwarningsDTO dto = new AlarmsandwarningsDTO
                {
                    Code = alarm.Code,
                    Description = alarm.Description,
                    Type = alarm.Type,
                    ReasonAnalysis = alarm.ReasonAnalysis,
                    RegisterNumber = alarm.RegisterNumber.Value,
                    timestamp = alarm.Timestamp,
                    InverterId = alarm.Inverter.InverterId,
                    InverterName = lang == "en" ? alarm.Inverter.InverterName : alarm.Inverter.ZhInverterName,
                };
                AlarmsDtos.Add(dto);
            }
            return AlarmsDtos;
        }

        public AlarmsandwarningsDTO GetAlarmAndWarningByCode(string code, string lang)
        {
            var alarm = _alarmsBL.GetAlarmAndWarningByCode(code, lang);
            var alarmDto = new AlarmsandwarningsDTO()
            {
                Code = alarm.Code,
                Description = alarm.Description,
                Type = alarm.Type,
                ReasonAnalysis = alarm.ReasonAnalysis,
                RegisterNumber = alarm.RegisterNumber.Value,
                timestamp = alarm.Timestamp
            };
            return alarmDto;
        }
    }
}
