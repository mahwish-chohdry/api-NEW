using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
        public class DevicealarmsHistoryDTO
        {
        public DevicealarmsHistoryDTO()
        {
            

        }
        public string Title { get; set; }
        public string Code { get; set; }     
        public string ReasonAnalysis { get; set; }
        public string Timestamp { get; set; }
    }
}
