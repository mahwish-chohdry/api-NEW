using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IAlarmsandwarningsBL
    {
        Alarmsandwarnings InsertAlarmsandwarnings(Alarmsandwarnings Devicealarms);
        Alarmsandwarnings UpdateAlarmsandwarnings(Alarmsandwarnings Devicealarms);
        IEnumerable<Alarmsandwarnings> GetAlarmsandwarnings();
        IEnumerable<Alarmsandwarnings> GetAlarmsandwarnings(string lang,int InverterId);
        IEnumerable<Alarmsandwarnings> GetAlarmsandwarnings(string lang);
        Alarmsandwarnings GetAlarmAndWarningByCode(string code, string lang);
        bool DeleteAlarmsandwarnings(int DevicealarmsId);
        Alarmsandwarnings GetAlarmsandwarnings(int Id);
        IQueryable<Alarmsandwarnings> QueryAlarmsandwarnings();
    }
}
