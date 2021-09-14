using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class CustomerDeviceMaintenanceReportDTO
    {
        public List<MaintenanceReportDTO> pendingDevice {get;set;}

        public List<MaintenanceReportDTO> maintainedDevice { get; set; }
    }
}
