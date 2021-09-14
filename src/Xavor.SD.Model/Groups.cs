using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Groups
    {
        public Groups()
        {
            Commandhistory = new HashSet<Commandhistory>();
            Devicegroup = new HashSet<Devicegroup>();
            Groupcommand = new HashSet<Groupcommand>();
        }

        public int Id { get; set; }
        public Guid? RecordId { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public short? IsActive { get; set; }
        public short? IsDeleted { get; set; }
        public int? CustomerId { get; set; }
        public string GroupId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Commandhistory> Commandhistory { get; set; }
        public virtual ICollection<Devicegroup> Devicegroup { get; set; }
        public virtual ICollection<Groupcommand> Groupcommand { get; set; }
    }
}
