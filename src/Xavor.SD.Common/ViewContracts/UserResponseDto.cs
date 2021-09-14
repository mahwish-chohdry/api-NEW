using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xavor.SD.WebAPI.ViewContracts
{
    public class UserResponseDTO
    {
        public int user_id { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
    }
}
