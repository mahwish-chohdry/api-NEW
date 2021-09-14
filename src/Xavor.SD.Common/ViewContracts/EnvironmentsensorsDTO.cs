using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
   public class EnvironmentsensorsDTO
    {

        public int Id { get; set; }
        public string Type { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string Unit { get; set; }
    }
}
