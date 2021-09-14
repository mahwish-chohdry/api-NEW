using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IGroupsBL
    {
        Groups InsertGroups(Groups Groups);
        Groups UpdateGroups(Groups Groups);
        IEnumerable<Groups> GetGroups();
        bool DeleteGroups(int GroupsId);
        Groups GetGroups(int Id);
        IQueryable<Groups> QueryGroups();
        List<Groups> GetAllGroups(int customer_id);

        Groups GetGroupByName(string groupName);
        Groups GetGroupByUniqueId(string groupId);
        int GetGroupDBId(string groupId);
        Groups GetGroupByNameAndCustomer(string groupName, int customerId);


    }
}
