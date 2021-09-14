using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class DevicealarmsDTO
    {

        public DevicealarmsDTO()
        {
            Id = 0;
            DeviceId = "";
            Speed = 0;
            Direction = 0;
            Warning = "No Warning";
            Alarm = "No Alarm";
            OutputFrequency = 0;
            OutputCurrent = 0;
            OutputPower = 0;
            OutputVoltage = 0;
            Rpm = 0;
            BusVoltage = 0;


        }
        public bool MessageType { get; set; }
        public int Id { get; set; }
        public string DeviceId { get; set; }

        public bool IsDeviceStatus { get; set; }

        public string CustomerId { get; set; }

        public int? Speed { get; set; }

        public int? Direction { get; set; }

        public string Warning { get; set; }
        public string WarningCode { get; set; }

        public string Alarm { get; set; }
        public string AlarmCode { get; set; }

        public double? OutputFrequency { get; set; }

        public double? OutputCurrent { get; set; }

        public double? OutputVoltage { get; set; }

        public double? OutputPower { get; set; }

        public double? Rpm { get; set; }

        public double? BusVoltage { get; set; }

        public string Timestamp { get; set; }
        public bool isAlarm { get; set; }
        public string AlarmsReasonAnalysis { get; set; }
        public string WarningReasonAnalysis { get; set; }

    }

    }
