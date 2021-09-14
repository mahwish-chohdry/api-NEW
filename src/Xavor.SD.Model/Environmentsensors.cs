using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Environmentsensors
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string Unit { get; set; }
    }
}
