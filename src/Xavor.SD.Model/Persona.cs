using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Persona
    {
        public Persona()
        {
            Personapermission = new HashSet<Personapermission>();
        }

        public int Id { get; set; }
        public string PersonaId { get; set; }
        public string PersonaName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<Personapermission> Personapermission { get; set; }
    }
}
