using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Groupcommand
    {
        public int Id { get; set; }
        public int? GroupId { get; set; }
        public int? CommandHistoryId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Commandhistory CommandHistory { get; set; }
        public virtual Groups Group { get; set; }
    }
}
