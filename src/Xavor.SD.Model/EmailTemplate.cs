using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class EmailTemplate
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public string Code { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
