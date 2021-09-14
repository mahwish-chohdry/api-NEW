using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Role
    {
        public Role()
        {
            Rolepermission = new HashSet<Rolepermission>();
            Userrole = new HashSet<Userrole>();
        }

        public int Id { get; set; }
        public string Role1 { get; set; }
        public string Description { get; set; }
        public short? IsActive { get; set; }
        public short? IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<Rolepermission> Rolepermission { get; set; }
        public virtual ICollection<Userrole> Userrole { get; set; }
    }
}
