using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Devicegroup
    {
        public int Id { get; set; }
        public int? DeviceId { get; set; }
        public int? GroupId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Device Device { get; set; }
        public virtual Groups Group { get; set; }
    }
}
