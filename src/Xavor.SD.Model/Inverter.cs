using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Inverter
    {
        public Inverter()
        {
            Alarmsandwarnings = new HashSet<Alarmsandwarnings>();
            Device = new HashSet<Device>();
        }

        public int Id { get; set; }
        public string InverterId { get; set; }
        public string InverterName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ZhInverterName { get; set; }

        public virtual ICollection<Alarmsandwarnings> Alarmsandwarnings { get; set; }
        public virtual ICollection<Device> Device { get; set; }
    }
}
