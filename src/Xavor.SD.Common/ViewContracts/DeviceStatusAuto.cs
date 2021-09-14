using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public  class DevicestatusManualDTO
    {
        public int? PowerStatus { get; set; }
        public int Speed { get; set; }
        public string DeviceId { get; set; }
        public string CustomerId { get; set; }
        public string CommandType { get; set; }
        public string CommandId { get; set; }
        public int Maintainence { get; set; }
        public int UsageHour { get; set; }
        public string CurrentFirmwareVersion { get; set; }
        public string LatestFirmwareVersion { get; set; }
        public string InverterId { get; set; }
    }
}
