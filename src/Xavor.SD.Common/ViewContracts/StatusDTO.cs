using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class StatusDTO
    {
        
        public string DeviceId { get; set; }
        public bool? PowerStatus { get; set; }
        public int Speed { get; set; }


        public double Temp { get; set; }        
        public double Humidity { get; set; }
        public int? Pressure { get; set; }
        public double? Iaq { get; set; }
        public int? IaqAccuracy { get; set; }
        public double? StaticIaq { get; set; }
        public int? StaticIaqAccuracy { get; set; }
        public double? Co2Concentration { get; set; }
        public int? Co2ConcentrationAccuracy { get; set; }
        public double? VocConcentration { get; set; }
        public int? VocConcentrationAccuracy { get; set; }
        public double? GasPercentage { get; set; }


        public bool? AutoTemp { get; set; }
        public bool? AutoTimer { get; set; }
        public string AutoStartTime { get; set; }
        public string AutoEndTime { get; set; }
        public bool? HasPreviousSetting { get; set; }
        public int IdealTemp { get; set; }
        public int MaintenanceHours { get; set; }
        public int MaxTemp { get; set; }
        public int MinTemp { get; set; }
        public bool OverrideSettings { get; set; }
        public string TimeZone { get; set; }
        public int UsageHours { get; set; }
        public bool? IsExecuted { get; set; }
        public string CommandType { get; set; }
        public string CommandId { get; set; }
        public string error { get; set; }
        public string warning { get; set; }
        public string CustomerName { get; set; }
        public bool resetRunningHour { get; set; }
    }
}
