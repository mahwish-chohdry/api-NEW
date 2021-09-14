using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;

namespace Xavor.SD.BusinessLayer
{
    public class UserroleBL : IUserroleBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Userrole> repo;

        public UserroleBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Userrole>();
        }

        public bool DeleteUserrole(int userroleId)
        {
            try
            {
                repo.Delete(userroleId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Userrole GetUserrole(int Id)
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

        public IEnumerable<Userrole> GetUserrole()
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

        public IQueryable<Userrole> QueryUserrole()
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

        public Userrole InsertUserrole(Userrole userrole)
        {
            try
            {
                repo.Add(userrole);
                uow.SaveChanges();

                return userrole;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Userrole UpdateUserrole(Userrole userrole)
        {
            try
            {
                repo.Update(userrole);
                uow.SaveChanges();

                return userrole;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteUserRoleByUserId(int userId)
        {
            var userRolesList = QueryUserrole().Where(dg => dg.UserId == userId).ToList<Userrole>();
            if (userRolesList.Count != 0)
            {
                foreach (var dg in userRolesList)
                {
                    DeleteUserrole(dg.Id);
                }

                return true;
            }
            return false;

        }
    }
}
