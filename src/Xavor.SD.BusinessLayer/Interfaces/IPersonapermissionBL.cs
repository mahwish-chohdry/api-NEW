using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IPersonapermissionBL
    {
        Personapermission InsertPersonapermission(Personapermission personapermission);
        Personapermission UpdatePersonapermission(Personapermission personapermission);
        IEnumerable<Personapermission> GetPersonapermissions();
        bool DeletePersonapermission(int personapermissionId);
        Personapermission GetPersonapermission(int Id);
        IQueryable<Personapermission> QueryPersonapermission();
    }
}
