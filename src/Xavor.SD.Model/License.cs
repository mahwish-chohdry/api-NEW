using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class License
    {
        public int Id { get; set; }
        public Guid? RecordId { get; set; }
        public string LicenseName { get; set; }
        public string LicenseType { get; set; }
    }
}
