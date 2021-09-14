using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IPermissionBL
    {
        Permission InsertPermission(Permission permission);
        Permission UpdatePermission(Permission permission);
        IEnumerable<Permission> GetPermission();
        bool DeletePermission(int PermissionId);
        Permission GetPermission(int Id);
        IQueryable<Permission> QueryPermission();
    }
}
