using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.Common.ViewContracts
{
    public class LoginDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public int Customer_Id { get; set; }
        public string CustomerId { get; set; }
        public string Customertype { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
        public Ruleengine Ruleengine { get; set; }
        public string ProfileImage { get; set; }
        public string Token { get; set; }
        public UserPermissionManagement userPermission { set; get; }
    }
}
