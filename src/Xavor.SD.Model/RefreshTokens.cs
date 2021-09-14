using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Refreshtokens
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string RefreshToken { get; set; }

        public virtual User User { get; set; }
    }
}
