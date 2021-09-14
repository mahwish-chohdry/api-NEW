using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xavor.SD.WebAPI.ViewContracts
{
    public class UserDTO
    {
        public int user_id { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string currentPassword { get; set; }
        public string newPassword { get; set; }
        public string deviceType { get; set; }
        public string deviceIdentifier { get; set; }
        public string usertype { get; set; }

        


    }
}
