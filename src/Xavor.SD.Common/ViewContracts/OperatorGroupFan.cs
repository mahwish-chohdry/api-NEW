using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class OperatorGroupFan
    {
        public int customer_id { get; set; }
        public int user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public List<int> device_ids { get; set; }
        //public List<int> group_ids { get; set; }

    }
}
