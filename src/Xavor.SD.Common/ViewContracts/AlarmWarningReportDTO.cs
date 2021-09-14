using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class AlarmWarningReportDTO
    {
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceCode { get; set; }
        public string DeviceStatus { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string ReasonAnalysis { get; set; }
        public int RegisterNumber { get; set; }
        public string timestamp { get; set; }
    }
}
