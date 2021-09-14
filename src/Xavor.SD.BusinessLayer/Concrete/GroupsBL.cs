using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;

namespace Xavor.SD.BusinessLayer
{
    public class GroupsBL : IGroupsBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Groups> repo;
        public GroupsBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Groups>();
        }
        public bool DeleteGroups(int GroupsId)
        {
            try
            {
                bool isDeleted = repo.Delete(GroupsId);
                uow.SaveChanges();
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public List<Groups> GetAllGroups(int customer_id)
        {
            var list = QueryGroups().Where(group => group.CustomerId == customer_id).ToList<Groups>();
            if(list.Count !=0)
            {
                return list;
            }
            return null;
        }

        public Groups GetGroupByName(string groupName)
        {
            var existingGroup = QueryGroups().Where(x => x.Name == groupName).FirstOrDefault();
            return existingGroup;
        }
        public Groups GetGroupByNameAndCustomer(string groupName, int customerId)
        {
            var existingGroup = QueryGroups().Where(x => x.Name == groupName && x.CustomerId == customerId).FirstOrDefault();
            return existingGroup;
        }


        public Groups GetGroupByUniqueId(string groupId)
        {
            var _group = QueryGroups()?.Where(x => x.GroupId == groupId).FirstOrDefault();
            return _group;
        }

        public int GetGroupDBId(string groupId)
        {
            var groupDbId = QueryGroups().Where(x => x.GroupId == groupId).Select(s => s.Id).FirstOrDefault();
            return groupDbId;
        }

        public IEnumerable<Groups> GetGroups()
        {
            try
            {
                return repo.GetList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Groups GetGroups(int Id)
        {
            try
            {
                if (Id <= default(int))
                    throw new ArgumentException("Invalid id");
                return repo.Find(Id);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Groups InsertGroups(Groups Groups)
        {
            try
            {
                repo.Add(Groups);
                uow.SaveChanges();

                return Groups;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Groups> QueryGroups()
        {
            try
            {
                return repo.Queryable();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Groups UpdateGroups(Groups Groups)
        {
            try
            {
                repo.Update(Groups);
                uow.SaveChanges();
                return Groups;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
