using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.ServiceLayer.Validations;

namespace Xavor.SD.ServiceLayer.ServiceModel
{
    public class AlarmsService : IAlarmsService
    {

        private IDevicealarmsBL deviceAlarmsBL;
        private IAlarmsandwarningsBL alarmsandWarningsBL;
        private IDeviceAlarmsHistoryBL _deviceAlarmsHistoryBL;
        private IDeviceBL _deviceBL;
        private IInverterBL _inverterBL;
        private readonly IDevicestatusBL _devicestatusBL;

        public AlarmsService(IDevicealarmsBL _deviceAlarmsBL, IAlarmsandwarningsBL _alarmsandWarningsBL, IDeviceAlarmsHistoryBL deviceAlarmsHistoryBL, IDeviceBL deviceBL, IDevicestatusBL devicestatusBL, IInverterBL inverterBL)
        {
            deviceAlarmsBL = _deviceAlarmsBL;
            alarmsandWarningsBL = _alarmsandWarningsBL;
            _deviceAlarmsHistoryBL = deviceAlarmsHistoryBL;
            _deviceBL = deviceBL;
            _devicestatusBL = devicestatusBL;
            _inverterBL = inverterBL;
        }

        public string GetLastTimeStamp(string customerId, string deviceId)
        {
            try
            {
                IQueryable<Devicealarms> existingDevicealarms = deviceAlarmsBL.QueryDevicealarms();
                //var l=existingDevicealarms.ToList();


                if (existingDevicealarms != null && existingDevicealarms.Where(x => x.DeviceId == deviceId).Count() > 0)
                {
                    return existingDevicealarms.Where(x => x.DeviceId == deviceId).OrderByDescending(x => x.Timestamp).FirstOrDefault().Timestamp;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Devicealarms> InsertNewAlarmsIfNotExistInDatabase(List<InverterAlarms> inverterAlarms)
        {
            List<Devicealarms> lstDevicealarms = new List<Devicealarms>();
            if (inverterAlarms != null && inverterAlarms.Count > 0)
            {
                IQueryable<Devicealarms> existingDevicealarms = deviceAlarmsBL.QueryDevicealarms();

                string customerId = inverterAlarms.First().CustomerId;
                string deviceId = inverterAlarms.First().DeviceId;

                try
                {
                    var result = existingDevicealarms.Where(x => x.CustomerId == customerId && x.DeviceId == deviceId);
                    lstDevicealarms = result.ToList();
                }
                catch (Exception ex)
                {
                    lstDevicealarms = null;
                }

                if (lstDevicealarms != null && lstDevicealarms.Count() == 0)
                {
                    foreach (InverterAlarms ia in inverterAlarms)
                    {
                        Devicealarms alarms = new Devicealarms();
                        alarms.Alarm = ia.Alarm; //converting alarms and warnings integer to binary and save in database
                        alarms.BusVoltage = ia.BusVoltage;
                        alarms.CustomerId = customerId;
                        alarms.DeviceId = deviceId;
                        alarms.Direction = ia.Direction;
                        alarms.OutputCurrent = Math.Round(ia.OutputCurrent, 3, MidpointRounding.AwayFromZero); // ia.OutputCurrent;
                        alarms.OutputFrequency = ia.OutputFrequency;
                        alarms.OutputPower = ia.OutputPower;
                        alarms.OutputVoltage = ia.OutputVoltage;
                        alarms.Rpm = ia.Rpm;
                        alarms.Speed = ia.Speed;
                        //alarms.Timestamp = DateTime.UtcNow.ToString();
                        alarms.Timestamp = ia.Timestamp;
                        alarms.Warning = ia.Warning;
                        deviceAlarmsBL.InsertDevicealarms(alarms);
                        UpdateDeviceStatusAlarmAttribute(deviceId, alarms);
                        //Insert Devicealarmshistory
                        InsertDeviceAlarmsHistory(alarms);
                        //need to send alarm and warning code to mobile
                        alarms.Alarm = ia.Alarm;
                        alarms.Warning = ia.Warning;
                        lstDevicealarms.Add(alarms);
                    }
                    return lstDevicealarms;
                }
                else if (lstDevicealarms.Count() == 1)
                {
                    var inverterAlarmFromCosmos = inverterAlarms.FirstOrDefault(); // only one alarm according to query logic
                    var dbDeviceAlarm = lstDevicealarms.FirstOrDefault(); // only one alarm from Devicealarms table

                    string existingAlarm = dbDeviceAlarm.Alarm;
                    string existingWarning = dbDeviceAlarm.Warning;

                    dbDeviceAlarm.Alarm = inverterAlarmFromCosmos.Alarm;
                    dbDeviceAlarm.BusVoltage = inverterAlarmFromCosmos.BusVoltage;
                    dbDeviceAlarm.CustomerId = customerId;
                    dbDeviceAlarm.DeviceId = deviceId;
                    dbDeviceAlarm.Direction = inverterAlarmFromCosmos.Direction;
                    dbDeviceAlarm.OutputCurrent = Math.Round(inverterAlarmFromCosmos.OutputCurrent, 3, MidpointRounding.AwayFromZero); // inverterAlarmFromCosmos.OutputCurrent;
                    dbDeviceAlarm.OutputFrequency = inverterAlarmFromCosmos.OutputFrequency;
                    dbDeviceAlarm.OutputPower = inverterAlarmFromCosmos.OutputPower;
                    dbDeviceAlarm.OutputVoltage = inverterAlarmFromCosmos.OutputVoltage;
                    dbDeviceAlarm.Rpm = inverterAlarmFromCosmos.Rpm;
                    dbDeviceAlarm.Speed = inverterAlarmFromCosmos.Speed;
                    //dbDeviceAlarm.Timestamp = DateTime.UtcNow.ToString();
                    dbDeviceAlarm.Timestamp = inverterAlarmFromCosmos.Timestamp;
                    dbDeviceAlarm.Warning = inverterAlarmFromCosmos.Warning;
                    deviceAlarmsBL.UpdateDevicealarms(dbDeviceAlarm);
                    UpdateDeviceStatusAlarmAttribute(deviceId, dbDeviceAlarm);

                    //Insert Devicealarmshistory
                    if (inverterAlarmFromCosmos.Alarm == existingAlarm && inverterAlarmFromCosmos.Warning == existingWarning)
                    {
                        //Only insert in No Alarm and No Warning case
                        if (inverterAlarmFromCosmos.Alarm == "No Alarm" && inverterAlarmFromCosmos.Warning == "No Warning")
                        {
                            InsertDeviceAlarmsHistory(dbDeviceAlarm);
                        }

                    }
                    else //insert if alarm is changed from previous state
                    {
                        InsertDeviceAlarmsHistory(dbDeviceAlarm);
                    }

                    var updatedDevicealarms = new List<Devicealarms>();
                    updatedDevicealarms.Add(dbDeviceAlarm);

                    return updatedDevicealarms;
                }

            }
            return lstDevicealarms;
        }

        private void UpdateDeviceStatusAlarmAttribute(string deviceUniqueId, Devicealarms devicealarms)
        {
            int deviceDbId = _deviceBL.GetDeviceDBId(deviceUniqueId);
            var existingDeviceStatus = _devicestatusBL.GetDevicestatusByDeviceId(deviceDbId);
            if (devicealarms.Alarm != "No Alarm" && existingDeviceStatus.Alarm == "No Alarm")
            {
                existingDeviceStatus.PowerStatus = 0;
            }
            if (existingDeviceStatus.Alarm != "No Alarm" && devicealarms.Alarm == "No Alarm")
            {
                if (devicealarms.Direction == 0)
                {
                    existingDeviceStatus.PowerStatus = 0;
                }
                else
                {
                    existingDeviceStatus.PowerStatus = 1;
                }
            }

            // Sync feature
            existingDeviceStatus.Speed = VfdSpeedSync(devicealarms, existingDeviceStatus.Speed);

            existingDeviceStatus.Alarm = devicealarms.Alarm;
            existingDeviceStatus.Warnings = devicealarms.Warning;
            if (existingDeviceStatus != null)
            {
                existingDeviceStatus.ModifiedDate = DateTime.UtcNow;
                _devicestatusBL.UpdateDevicestatus(existingDeviceStatus);
            }
        }

        private int VfdSpeedSync(Devicealarms devicealarms, int vfdSpeed)
        {
            int speed;
            // 1-5.3, 2-7.3, 3-9.6, 4-10.9, 5-16.5, 6-19.8, 7-23.1, 8-26.4, 9-29.7, 10-33 
            switch (Convert.ToDouble(devicealarms.OutputFrequency))
            {
                case double n when (n >= 32.25): // 10-33 
                    {
                        speed = 10;
                        break;
                    }
                case double n when (n >= 28.95 && n < 32.25): // 9-29.7
                    {
                        speed = 9;
                        break;
                    }
                case double n when (n >= 25.65 && n < 28.95): // 8-26.4
                    {
                        speed = 8;
                        break;
                    }
                case double n when (n >= 22.35 && n < 25.65): // 7-23.1
                    {
                        speed = 7;
                        break;
                    }
                case double n when (n >= 19.05 && n < 22.35): // 6-19.8
                    {
                        speed = 6;
                        break;
                    }
                case double n when (n >= 15.75 && n < 19.05): // 5-16.5
                    {
                        speed = 5;
                        break;
                    }
                case double n when (n >= 10.15 && n < 15.75): // 4-10.9
                    {
                        speed = 4;
                        break;
                    }
                case double n when (n >= 8.85 && n < 10.15): // 3-9.6
                    {
                        speed = 3;
                        break;
                    }
                case double n when (n >= 6.55 && n < 8.85): // 2-7.3
                    {
                        speed = 2;
                        break;
                    }
                case double n when (n >= 4.5 && n < 6.55): // 1-5.3 
                    {
                        speed = 1;
                        break;
                    }
                default: // existing value
                    {
                        speed = vfdSpeed;
                        break;
                    }
            }
            return speed;
        }

        private Devicealarmshistory InsertDeviceAlarmsHistory(Devicealarms devicealarms)
        {
            var newDevicealarmshistory = new Devicealarmshistory
            {
                DeviceId = _deviceBL.GetDeviceDBId(devicealarms.DeviceId),
                Speed = devicealarms.Speed,
                Direction = devicealarms.Direction,
                Warning = devicealarms.Warning,
                Alarm = devicealarms.Alarm,
                OutputFrequency = devicealarms.OutputFrequency,
                OutputCurrent = devicealarms.OutputCurrent,
                OutputVoltage = devicealarms.OutputVoltage,
                OutputPower = devicealarms.OutputPower,
                Rpm = devicealarms.Rpm,
                Timestamp = devicealarms.Timestamp,
                CreatedDate = DateTime.UtcNow
            };

            if (devicealarms.OutputCurrent != null)
                newDevicealarmshistory.OutputCurrent = Math.Round(Convert.ToDouble(devicealarms.OutputCurrent), 3, MidpointRounding.AwayFromZero);
            return _deviceAlarmsHistoryBL.InsertDevicealarmshistory(newDevicealarmshistory);
        }
        public string InsertSpaceBeforeUpperCase(string str)
        {
            var sb = new StringBuilder();
            char previousChar = char.MinValue;
            foreach (char c in str)
            {
                if (char.IsUpper(c))
                {
                    if (sb.Length != 0 && previousChar != ' ') { sb.Append(' '); }
                }
                sb.Append(c);
                previousChar = c;
            }
            return sb.ToString();
        }


        //public string BinaryToAlarm(string value)
        //{
        //    var alarmValue = "No Alarm";
        //    for (int i = value.Length - 1; i >= 0; i--)
        //    {
        //        if (alarmValue != "No Alarm") break;
        //        var alarm = value.Substring(i, 1);
        //        switch (Convert.ToInt32(alarm))
        //        {
        //            case (int)Alarms.DriveOverload: { alarmValue = InsertSpaceBeforeUpperCase(Alarms.DriveOverload.ToString()); break; }
        //            case (int)Alarms.DriveOverTemperature: { alarmValue = InsertSpaceBeforeUpperCase(Alarms.DriveOverTemperature.ToString()); break; }
        //            case (int)Alarms.EarthFault: { alarmValue = InsertSpaceBeforeUpperCase(Alarms.EarthFault.ToString()); break; }
        //            case (int)Alarms.InternalFault: { alarmValue = InsertSpaceBeforeUpperCase(Alarms.InternalFault.ToString()); break; }
        //            case (int)Alarms.MainPhaseLoss: { alarmValue = InsertSpaceBeforeUpperCase(Alarms.MainPhaseLoss.ToString()); break; }
        //            case (int)Alarms.MotorPhaseMissing: { alarmValue = InsertSpaceBeforeUpperCase(Alarms.MotorPhaseMissing.ToString()); break; }
        //            case (int)Alarms.OverCurrent: { alarmValue = InsertSpaceBeforeUpperCase(Alarms.OverCurrent.ToString()); break; }
        //            case (int)Alarms.ShortCircuit: { alarmValue = InsertSpaceBeforeUpperCase(Alarms.ShortCircuit.ToString()); break; }
        //            default: { break; }
        //        }
        //    }
        //    return alarmValue;
        //}

        //public string BinaryToWarning(string value)
        //{
        //    var warningValue = "No Warning";
        //    for (int i = value.Length - 1; i >= 0; i--)
        //    {
        //        if (warningValue != "No Warning") break;
        //        var warning = value.Substring(i, 1);
        //        switch (Convert.ToInt32(warning))
        //        {
        //            case (int)Warnings.CurrentLimit: { warningValue = InsertSpaceBeforeUpperCase(Warnings.CurrentLimit.ToString()); break; }
        //            case (int)Warnings.DriveOverload: { warningValue = InsertSpaceBeforeUpperCase(Warnings.DriveOverload.ToString()); break; }
        //            case (int)Warnings.FanFault: { warningValue = InsertSpaceBeforeUpperCase(Warnings.FanFault.ToString()); break; }
        //            case (int)Warnings.MainPhaseLoss: { warningValue = InsertSpaceBeforeUpperCase(Warnings.MainPhaseLoss.ToString()); break; }
        //            case (int)Warnings.OverCurrent: { warningValue = InsertSpaceBeforeUpperCase(Warnings.OverCurrent.ToString()); break; }
        //            case (int)Warnings.OverVoltage: { warningValue = InsertSpaceBeforeUpperCase(Warnings.OverVoltage.ToString()); break; }
        //            case (int)Warnings.UnderVoltage: { warningValue = InsertSpaceBeforeUpperCase(Warnings.UnderVoltage.ToString()); break; }
        //        }
        //    }
        //    return warningValue;
        //}

        public List<Devicealarms> ConvertInvertToDevicealarms(List<InverterAlarms> inverterAlarms, string customerId, string deviceId)
        {
            List<Devicealarms> Devicealarms = new List<Devicealarms>();
            foreach (var ia in inverterAlarms)
            {
                Devicealarms alarms = new Devicealarms();
                alarms.Alarm = ia.Alarm.ToString();//converting alarms and warnings integer to binary and save in database
                alarms.BusVoltage = ia.BusVoltage;
                alarms.CustomerId = customerId;
                alarms.DeviceId = deviceId;
                alarms.Direction = ia.Direction;
                //IsDevicestatus = ia.IsDevicestatus,
                alarms.OutputCurrent = Math.Round(Convert.ToDouble(ia.OutputCurrent), 3, MidpointRounding.AwayFromZero);
                alarms.OutputFrequency = ia.OutputFrequency;
                alarms.OutputPower = ia.OutputPower;
                alarms.OutputVoltage = ia.OutputVoltage;
                alarms.Rpm = ia.Rpm;
                alarms.Speed = ia.Speed;
                alarms.Timestamp = ia.EventProcessedUtcTime;
                alarms.Warning = ia.Warning.ToString();

                //need to send alarm and warning code to mobile
                //alarms.Alarm = ((Alarms)Convert.ToInt32(ia.Alarm)).ToString();
                // alarms.Warning = ((Warnings)Convert.ToInt32(ia.Warning)).ToString();
                Devicealarms.Add(alarms);
            }
            return Devicealarms;
        }

        public List<DevicealarmsHistoryDTO> TranformDevicealarmsHistory(List<Devicealarmshistory> deviceAlarms, string issueType, string lang)
        {
            List<DevicealarmsHistoryDTO> newDeviceAlarmList = new List<DevicealarmsHistoryDTO>();
            var possibleAlarmsAndWarnings = alarmsandWarningsBL.GetAlarmsandwarnings();
            foreach (var obj in deviceAlarms)
            {
                DevicealarmsHistoryDTO newDeviceAlarm = new DevicealarmsHistoryDTO();

                if (obj.Alarm != "No Alarm" && obj.Alarm != null && issueType == "1")
                {
                    newDeviceAlarm.Code = "E." + obj.Alarm;
                    var alarmWarning = possibleAlarmsAndWarnings.Where(x => x.Language == lang && x.Code == newDeviceAlarm.Code).FirstOrDefault();


                    if (alarmWarning != null)
                    {
                        newDeviceAlarm.Title = alarmWarning.Description;
                        newDeviceAlarm.ReasonAnalysis = alarmWarning.ReasonAnalysis;
                        newDeviceAlarm.Timestamp = obj.Timestamp;
                        newDeviceAlarmList.Add(newDeviceAlarm);
                    }

                }
                else if (issueType == "0" && obj.Warning != null && obj.Warning != "No Warning")
                {
                    newDeviceAlarm.Code = "A." + obj.Warning;
                    var alarmWarning = possibleAlarmsAndWarnings.Where(x => x.Language == lang && x.Code == newDeviceAlarm.Code).FirstOrDefault();
                    if (alarmWarning != null)
                    {
                        newDeviceAlarm.Title = alarmWarning.Description;
                        newDeviceAlarm.ReasonAnalysis = alarmWarning.ReasonAnalysis;
                        newDeviceAlarm.Timestamp = obj.Timestamp;
                        newDeviceAlarmList.Add(newDeviceAlarm);
                    }
                }
            }
            return newDeviceAlarmList;
        }

        public List<DevicealarmsDTO> TranformDevicealarms(List<Devicealarms> deviceAlarms, string lang)
        {
            List<DevicealarmsDTO> newDeviceAlarmList = new List<DevicealarmsDTO>();
            foreach (var obj in deviceAlarms)
            {
                DevicealarmsDTO newDeviceAlarm = new DevicealarmsDTO();
                var device = _deviceBL.GetDeviceByUniqueId(obj.DeviceId);
                var inverter = new Inverter();
                if (device.InverterId != null)
                {
                    inverter = _inverterBL.GetInverter(Convert.ToInt32(device.InverterId));
                }

                if (inverter.InverterId == "Holip")
                {
                    //if Inverter type Holip

                    newDeviceAlarm.Warning = obj.Warning;
                    newDeviceAlarm.Alarm = obj.Alarm;
                    newDeviceAlarm.AlarmCode = "0000";
                    newDeviceAlarm.WarningCode = "0000";
                    if (obj.Alarm != "No Alarm" && obj.Alarm != null)
                    {
                        newDeviceAlarm.AlarmCode = "E." + newDeviceAlarm.Alarm;
                        newDeviceAlarm.isAlarm = true;
                        // newDeviceAlarm.Alarm = alarmsandWarningsBL.QueryAlarmsandwarnings().Where(x => x.Code == newDeviceAlarm.AlarmCode).FirstOrDefault().Description;
                        var alarmWarning = alarmsandWarningsBL.QueryAlarmsandwarnings().Where(x => x.Language == lang && x.Code == newDeviceAlarm.AlarmCode).FirstOrDefault();
                        if (alarmWarning != null)
                        {
                            newDeviceAlarm.AlarmsReasonAnalysis = alarmWarning.ReasonAnalysis;
                            newDeviceAlarm.Alarm = alarmWarning.Description;
                        }

                    }
                    if (obj.Warning != "No Warning" && obj.Warning != null)
                    {
                        newDeviceAlarm.WarningCode = "A." + newDeviceAlarm.Warning;
                        newDeviceAlarm.isAlarm = false;
                        //  newDeviceAlarm.Warning = alarmsandWarningsBL.QueryAlarmsandwarnings().Where(x => x.Code == newDeviceAlarm.WarningCode).FirstOrDefault().Description;
                        var alarmWarning = alarmsandWarningsBL.QueryAlarmsandwarnings().Where(x => x.Language == lang && x.Code == newDeviceAlarm.WarningCode).FirstOrDefault();
                        if (alarmWarning != null)
                        {
                            newDeviceAlarm.WarningReasonAnalysis = alarmWarning.ReasonAnalysis;
                            newDeviceAlarm.Warning = alarmWarning.Description;
                        }

                    }

                }
                else if (inverter.InverterId == "Schneider")
                {
                    if (obj.Alarm != "No Alarm" && obj.Alarm != null)
                    {
                        var alarmCode = Convert.ToInt32(obj.Alarm);
                        var AlarmDBCode = Enum.ToObject(typeof(ScheniederAlarms), alarmCode).ToString();

                        var alarm = alarmsandWarningsBL.QueryAlarmsandwarnings().Where(x => x.Language == lang && x.InverterId == inverter.Id && x.Code == AlarmDBCode).FirstOrDefault();
                        newDeviceAlarm.AlarmsReasonAnalysis = alarm.ReasonAnalysis;
                        newDeviceAlarm.Alarm = alarm.Description;
                        newDeviceAlarm.isAlarm = true;

                    }
                    //if Inverter type Schneider
                }

                newDeviceAlarm.BusVoltage = obj.BusVoltage;
                newDeviceAlarm.CustomerId = obj.CustomerId;
                newDeviceAlarm.DeviceId = obj.DeviceId;
                newDeviceAlarm.Direction = obj.Direction;
                newDeviceAlarm.OutputCurrent = obj.OutputCurrent;                
                newDeviceAlarm.OutputFrequency = obj.OutputFrequency;
                newDeviceAlarm.OutputPower = obj.OutputPower;
                newDeviceAlarm.OutputVoltage = obj.OutputVoltage;
                newDeviceAlarm.Rpm = obj.Rpm;
                newDeviceAlarm.Speed = obj.Speed;
                newDeviceAlarm.Timestamp = obj.Timestamp;
                if (obj.OutputCurrent != null)
                    newDeviceAlarm.OutputCurrent = Math.Round(Convert.ToDouble(obj.OutputCurrent), 2, MidpointRounding.AwayFromZero);
                if (obj.OutputFrequency != null)
                    newDeviceAlarm.OutputFrequency = Math.Round(Convert.ToDouble(obj.OutputFrequency), 2, MidpointRounding.AwayFromZero);
                if (obj.OutputPower != null)
                    newDeviceAlarm.OutputPower = Math.Round(Convert.ToDouble(obj.OutputPower), 2, MidpointRounding.AwayFromZero);
                if (obj.OutputVoltage != null)
                    newDeviceAlarm.OutputVoltage = Math.Round(Convert.ToDouble(obj.OutputVoltage), 2, MidpointRounding.AwayFromZero);
                newDeviceAlarmList.Add(newDeviceAlarm);
            }
            return newDeviceAlarmList;
        }
        public List<Devicealarmshistory> GetDeviceAlarmsHistory(string customerId, string deviceId, string issueType)
        {
            Validator.ValidateCustomer(customerId);
            var device = Validator.ValidateDevice(customerId, deviceId);
            if (issueType != "0" && issueType != "1")
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Invalid issue type.",
                    Data = null
                });
            }

            var deviceAlarmsHistoryFromDb = _deviceAlarmsHistoryBL.GetDevicealarmshistoryByIssueType(device.Id, issueType);
            return deviceAlarmsHistoryFromDb;
        }

        public List<Devicealarmshistory> GetDeviceAlarmsHistory(string customerId, string deviceId)
        {
            Validator.ValidateCustomer(customerId);
            var device = Validator.ValidateDevice(customerId, deviceId);


            var deviceAlarmsHistoryFromDb = _deviceAlarmsHistoryBL.QueryDevicealarmshistory().Where(x => x.DeviceId == device.Id).ToList();
            return deviceAlarmsHistoryFromDb;
        }

        public List<Devicealarmshistory> GetDeviceAlarmsHistoryByDate(string customerId, string deviceId, string lastDate)
        {
            Validator.ValidateCustomer(customerId);
            var device = Validator.ValidateDevice(customerId, deviceId);


            var deviceAlarmsHistoryFromDb = _deviceAlarmsHistoryBL.QueryDevicealarmshistory().Where(x => x.DeviceId == device.Id && Convert.ToDateTime(x.Timestamp).Date >= Convert.ToDateTime(lastDate).Date).ToList();
            return deviceAlarmsHistoryFromDb;
        }
    }
}

