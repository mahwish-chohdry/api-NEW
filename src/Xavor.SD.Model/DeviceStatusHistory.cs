using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Devicestatushistory
    {
        public int Id { get; set; }
        public int? DeviceId { get; set; }
        public short? PowerStatus { get; set; }
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
        public short? AutoTemp { get; set; }
        public short? AutoTimer { get; set; }
        public string AutoStartTime { get; set; }
        public string AutoEndTime { get; set; }
        public int MaxTemp { get; set; }
        public int MinTemp { get; set; }
        public int IdealTemp { get; set; }
        public int MaintenanceHours { get; set; }
        public int RunningTime { get; set; }
        public string CommandType { get; set; }
        public string TimeZone { get; set; }
        public short? HasPreviousSetting { get; set; }
        public short? OverrideSettings { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
