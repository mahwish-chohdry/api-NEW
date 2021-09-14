using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Xavor.SD.Model
{
    public class SmartDeviceContainer
    {
        public string id { get; set; }
        [DefaultValue(1)]
        public string MessageType { get; set; }
        public string CustomerId { get; set; }
        public string DeviceId { get; set; }
        public int PowerStatus { get; set; }
        public int Speed { get; set; }


        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Pressure { get; set; }
        public string Iaq { get; set; }
        public string IaqAccuracy { get; set; }
        public string StaticIaq { get; set; }
        public string StaticIaqAccuracy { get; set; }
        public string Co2Concentration { get; set; }
        public string Co2ConcentrationAccuracy { get; set; }
        public string VocConcentration { get; set; }
        public string VocConcentrationAccuracy { get; set; }
        public string GasPercentage { get; set; }


        public int PLC { get; set; }
        public string CurrentFirmwareVersion { get; set; }
        public string Warning { get; set; }
        public string Alarm { get; set; }


        public int? AutoTemp { get; set; }
        public int? AutoTimer { get; set; }
        public int IdealTemp { get; set; }
        public string AutoStartTime { get; set; }
        public string AutoEndTime { get; set; }


        public int MaintenanceHours { get; set; }
        public int MaxTemp { get; set; }
        public int MinTemp { get; set; }
        public string Timestamp { get; set; }
        public string InverterId { get; set; }

      
    }
}
