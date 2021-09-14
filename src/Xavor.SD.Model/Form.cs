using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Form
    {
        public Form()
        {
            Personapermission = new HashSet<Personapermission>();
            Rolepermission = new HashSet<Rolepermission>();
        }

        public int Id { get; set; }
        public string FormId { get; set; }
        public string FormName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<Personapermission> Personapermission { get; set; }
        public virtual ICollection<Rolepermission> Rolepermission { get; set; }
    }
}
