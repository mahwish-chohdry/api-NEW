using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Ruleengine
    {
        public int Id { get; set; }
        public Guid? RecordId { get; set; }
        public int? CustomerId { get; set; }
        public bool? Switch { get; set; }
        public bool? Speed { get; set; }
        public int? Interval { get; set; }
        public bool? Troubleshoot { get; set; }
        public bool? Sensor { get; set; }
        public bool? Maintenance { get; set; }
        public bool? Report { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
