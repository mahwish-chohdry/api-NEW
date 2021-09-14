using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class GroupcommandDTO
    {
        public bool PowerStatus { get; set; }
        public double Temp { get; set; }
        public int Speed { get; set; }
        public double Humidity { get; set; }
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
        public bool IsExecuted { get; set; }
        public string CommandType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
