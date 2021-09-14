using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class EmailDTO
    {
        public string customerId { get; set; }
        public string userId { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
}
