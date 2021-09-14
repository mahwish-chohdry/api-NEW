using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Model
{
   public class DevicePowerConsumptionReportDTO
    {
        public double PowerConsumption { get; set; }

        public string deviceId { get; set; }
        public string deviceName { get; set; }
        public string deviceCode { get; set; }
        public string deviceStatus { get; set; }

        public string date { get; set; }
    }
}
