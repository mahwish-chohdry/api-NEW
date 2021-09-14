using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IGroupcommandBL
    {
        Groupcommand InsertGroupcommand(Groupcommand Groupcommand);
        Groupcommand UpdateGroupcommand(Groupcommand Groupcommand);
        IEnumerable<Groupcommand> GetGroupcommand();
        bool DeleteGroupcommand(int GroupcommandId);
        Groupcommand GetGroupcommand(int Id);
        IQueryable<Groupcommand> QueryGroupcommand();
        void AddOrUpdateGroupcommand(Groupcommand groupCommand);
        bool DeleteGroupcommandByGroupId(int group_id);
    }
}
