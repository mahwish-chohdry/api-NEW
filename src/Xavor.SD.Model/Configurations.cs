using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Configurations
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public int? CustomerId { get; set; }
        public short? IsActive { get; set; }
        public short? IsDeleted { get; set; }
        public short? IsMobileConfiguration { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
