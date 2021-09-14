using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common
{
    public class UnGroupedDevicesDTO
    {
        public int customer_id { get; set; }
        public string device_ap_name { get; set; }
        public string device_display_name { get; set; }
        public int device_id { get; set; }
        public byte? isActive { get; set; }
    }
}
