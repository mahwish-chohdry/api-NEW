using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Alarmsandwarnings
    {
        public int Id { get; set; }
        public int? InverterId { get; set; }
        public string Language { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string EncryptedCode { get; set; }
        public string ReasonAnalysis { get; set; }
        public string Timestamp { get; set; }
        public int? RegisterNumber { get; set; }

        public virtual Inverter Inverter { get; set; }
    }
}
