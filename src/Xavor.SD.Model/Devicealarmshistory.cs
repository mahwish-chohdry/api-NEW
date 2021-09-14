using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Devicealarmshistory
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int? Speed { get; set; }
        public int? Direction { get; set; }
        public string Warning { get; set; }
        public string Alarm { get; set; }
        public double? OutputFrequency { get; set; }
        public double? OutputCurrent { get; set; }
        public double? OutputVoltage { get; set; }
        public double? OutputPower { get; set; }
        public double? Rpm { get; set; }
        public string Timestamp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Device Device { get; set; }
    }
}
