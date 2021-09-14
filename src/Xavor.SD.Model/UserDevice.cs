using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Userdevice
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? DeviceId { get; set; }

        public virtual Device Device { get; set; }
        public virtual User User { get; set; }
    }
}
