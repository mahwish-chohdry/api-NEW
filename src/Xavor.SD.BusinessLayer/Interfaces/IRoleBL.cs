using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IRoleBL
    {
        Role InsertRole(Role role);
        Role UpdateRole(Role role);
        IEnumerable<Role> GetRoles();
        IEnumerable<Role> GetRoleByDescription(string description);
        Role GetRoleByNameAndDescription(string roleName, string description);
        bool DeleteRole(int roleId);
        Role GetRole(int Id);
        IQueryable<Role> QueryRole();
    }
}
