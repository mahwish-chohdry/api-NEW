using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IDefaultSettingsBL
    {
        Defaultsettings InsertDefaultsettings(Defaultsettings Defaultsettings);
        Defaultsettings UpdateDefaultsettings(Defaultsettings Defaultsettings);
        IEnumerable<Defaultsettings> GetDefaultsettings();
        bool DeleteDefaultsettings(int DefaultsettingsId);
        Defaultsettings GetDefaultsettings(int Id);
        IQueryable<Defaultsettings> QueryDefaultsettings();
    }
}
