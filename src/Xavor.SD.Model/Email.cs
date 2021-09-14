using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Email
    {
        public int Id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
