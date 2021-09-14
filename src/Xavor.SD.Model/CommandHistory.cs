using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Commandhistory
    {
        public Commandhistory()
        {
            Devicecommand = new HashSet<Devicecommand>();
            Groupcommand = new HashSet<Groupcommand>();
        }

        public int Id { get; set; }
        public int? GroupId { get; set; }
        public int? DeviceId { get; set; }
        public short? PowerStatus { get; set; }
        public double Temp { get; set; }
        public int Speed { get; set; }
        public double Humidity { get; set; }
        public short? AutoTemp { get; set; }
        public short? AutoTimer { get; set; }
        public string AutoStartTime { get; set; }
        public string AutoEndTime { get; set; }
        public short? HasPreviousSetting { get; set; }
        public int IdealTemp { get; set; }
        public int MaintenanceHours { get; set; }
        public int MaxTemp { get; set; }
        public int MinTemp { get; set; }
        public short? OverrideSettings { get; set; }
        public string TimeZone { get; set; }
        public int UsageHours { get; set; }
        public short? IsExecuted { get; set; }
        public string CommandType { get; set; }
        public string CommandId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Device Device { get; set; }
        public virtual Groups Group { get; set; }
        public virtual ICollection<Devicecommand> Devicecommand { get; set; }
        public virtual ICollection<Groupcommand> Groupcommand { get; set; }
    }
}
