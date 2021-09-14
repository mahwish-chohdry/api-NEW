using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Devicecommand
    {
        public int Id { get; set; }
        public int CommandHistoryId { get; set; }
        public int DeviceId { get; set; }
        public short? IsGrouped { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Commandhistory CommandHistory { get; set; }
        public virtual Device Device { get; set; }
    }
}
