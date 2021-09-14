using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Defaultsettings
    {
        public int Id { get; set; }
        public int? IdealTemp { get; set; }
        public string AutoStartTime { get; set; }
        public string AutoEndTime { get; set; }
        public int? MaintenanceHours { get; set; }
        public short? AutoTemp { get; set; }
        public short? AutoTimer { get; set; }
        public short? HasPreviousSetting { get; set; }
        public short? OverrideSettings { get; set; }
        public string TimeZone { get; set; }
        public int? Speed { get; set; }
        public short? PowerStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
