using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Customer
    {
        public Customer()
        {
            Configurations = new HashSet<Configurations>();
            Device = new HashSet<Device>();
            Groups = new HashSet<Groups>();
            Ruleengine = new HashSet<Ruleengine>();
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public Guid? RecordId { get; set; }
        public string CustomerId { get; set; }
        public int? ParentId { get; set; }
        public string CustomerType { get; set; }
        public int? PersonaId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public short? IsActive { get; set; }
        public short? IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<Configurations> Configurations { get; set; }
        public virtual ICollection<Device> Device { get; set; }
        public virtual ICollection<Groups> Groups { get; set; }
        public virtual ICollection<Ruleengine> Ruleengine { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
