using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class User
    {
        public User()
        {
            Refreshtokens = new HashSet<Refreshtokens>();
            Userdevice = new HashSet<Userdevice>();
            Userrole = new HashSet<Userrole>();
        }

        public int Id { get; set; }
        public Guid? RecordId { get; set; }
        public int? ParentId { get; set; }
        public int? CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string DeviceType { get; set; }
        public string DeviceIdentifier { get; set; }
        public string IdentityProvider { get; set; }
        public string DomainUserName { get; set; }
        public string ProfilePicture { get; set; }
        public short? IsActive { get; set; }
        public short? IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Refreshtokens> Refreshtokens { get; set; }
        public virtual ICollection<Userdevice> Userdevice { get; set; }
        public virtual ICollection<Userrole> Userrole { get; set; }
    }
}
