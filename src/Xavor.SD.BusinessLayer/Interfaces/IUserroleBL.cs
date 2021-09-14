using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IUserroleBL
    {
        Userrole InsertUserrole(Userrole userrole);
        Userrole UpdateUserrole(Userrole userrole);
        IEnumerable<Userrole> GetUserrole();
        bool DeleteUserrole(int userroleId);
        Userrole GetUserrole(int Id);
        IQueryable<Userrole> QueryUserrole();
        bool DeleteUserRoleByUserId(int userId);
    }
}
