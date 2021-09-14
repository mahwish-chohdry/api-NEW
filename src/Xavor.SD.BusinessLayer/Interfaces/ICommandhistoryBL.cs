using System.Collections.Generic;
using System.Linq;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface ICommandhistoryBL
    {
        bool DeleteCommandhistory(int CommandhistoryId);
        Commandhistory GetCommandhistory(int Id);
        IEnumerable<Commandhistory> GetCommandhistorys();
        Commandhistory InsertCommandhistory(Commandhistory Commandhistory);
        IQueryable<Commandhistory> QueryCommandhistory();
        Commandhistory UpdateCommandhistory(Commandhistory Commandhistory);
        Commandhistory GetUnExceutedCommand(int deviceid);
        Commandhistory GetLastExceutedCommand(int deviceid);
        bool DeleteCommandhistoryByGroupId(int group_id);
        bool DeleteCommandhistoryByDeviceId(int deviceId);
    }
}