using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.Common.ViewContracts
{
    public class SmartDeviceContainerDTO
    {
        public string id { get; set; }
        public string MessageType { get; set; }
        public string CustomerId { get; set; }

        public string GroupId { get; set; }
        public string DeviceId { get; set; }
        // device properties

        public string FanStatus { get; set; }

        public bool PowerStatus { get; set; }

        public string Speed { get; set; }

        public string Timestamp { get; set; }

        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Errors { get; set; }
        public string Warnings { get; set; }
        public string Alarm { get; set; }
        public InverterAlarms Alarms { get; set; }

        public bool AutoTemp { get; set; }
        public bool AutoTimer { get; set; }
        public string AutoStartTime { get; set; }
        public string AutoEndTime { get; set; }
        public bool HasPreviousSetting { get; set; }
        public int IdealTemp { get; set; }
        public int MaintenanceHours { get; set; }
        public int MaxTemp { get; set; }
        public int MinTemp { get; set; }
        public bool OverrideSettings { get; set; }
        public string TimeZone { get; set; }
        public int UsageHours { get; set; }
        public string EventProcessedUtcTime { get; set; }
    }
}
