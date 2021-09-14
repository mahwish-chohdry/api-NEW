using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Xavor.SD.Model
{
    public class InverterAlarms
    {
        /// <summary>
        /// Guid id for cosmosdb... to be set by smart device
        /// </summary>
        public string id { get; set; }
        [DefaultValue(0)]
        public int MessageType { get; set; }
        public string DeviceId { get; set; }
        public string CustomerId { get; set; }

        public int Speed { get; set; }

        public int Direction { get; set; }

        public string Warning { get; set; }

        public string Alarm { get; set; }

        public double OutputFrequency { get; set; }

        public double OutputCurrent { get; set; }

        public double OutputVoltage { get; set; }

        public double OutputPower { get; set; }

        public double Rpm { get; set; }

        public double BusVoltage { get; set; }
        public string EventProcessedUtcTime { get; set; }
        public string Timestamp { get; set; }
    }
}
