using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class DevicestatusAutoDTO
    {
        public string DeviceId { get; set; }
        public string CustomerId { get; set; }
        public string CommandId { get; set; }
        public string CommandType { get; set; }
        public int? AutoTemp { get; set; }
        public int? AutoTimer { get; set; }
        public string AutoStartTime { get; set; }
        public string AutoEndTime { get; set; }
        public int MaxTemp { get; set; }
        public int MinTemp { get; set; }
        public int IdealTemp { get; set; }

        public int Maintainence { get; set; }
        public int UsageHour { get; set; }
        public string CurrentFirmwareVersion { get; set; }
        public string LatestFirmwareVersion { get; set; }

        public string InverterId { get; set; }
    }
}
