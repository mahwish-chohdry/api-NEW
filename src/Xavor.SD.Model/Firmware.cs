using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Firmware
    {
        public int Id { get; set; }
        public string BatchId { get; set; }
        public string CustomerId { get; set; }
        public string FirmwareData { get; set; }
        public string FirmwareVersion { get; set; }
        public string FileFormat { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
