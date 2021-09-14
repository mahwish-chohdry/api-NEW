using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class AlarmsandwarningsDTO
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string ReasonAnalysis { get; set; }
        public int RegisterNumber { get; set; } 
        public string timestamp { get; set; }
        public string InverterId { get; set; }
        public string InverterName { get; set; }
    }
}
