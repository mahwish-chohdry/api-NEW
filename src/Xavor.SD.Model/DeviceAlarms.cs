using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Devicealarms
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string CustomerId { get; set; }
        public int? Speed { get; set; }
        public int? Direction { get; set; }
        public string Warning { get; set; }
        public string Alarm { get; set; }
        public double? OutputFrequency { get; set; }
        public double? OutputCurrent { get; set; }
        public double? OutputVoltage { get; set; }
        public double? OutputPower { get; set; }
        public double? Rpm { get; set; }
        public double? BusVoltage { get; set; }
        public string Timestamp { get; set; }
    }
}
