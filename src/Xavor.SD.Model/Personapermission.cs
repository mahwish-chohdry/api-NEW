using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Personapermission
    {
        public int Id { get; set; }
        public int? PersonaId { get; set; }
        public int? FormId { get; set; }

        public virtual Form Form { get; set; }
        public virtual Persona Persona { get; set; }
    }
}
