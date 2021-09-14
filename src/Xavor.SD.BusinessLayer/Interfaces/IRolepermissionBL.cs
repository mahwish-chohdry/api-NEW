using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IRolepermissionBL
    {
        Rolepermission InsertRolepermission(Rolepermission rolepermission);
        Rolepermission UpdateRolepermission(Rolepermission rolepermission);
        IEnumerable<Rolepermission> GetRolepermissions();
        bool DeleteRolepermission(int rolepermissionId);
        Rolepermission GetRolepermission(int Id);
        IQueryable<Rolepermission> QueryRolepermission();
    }
}
