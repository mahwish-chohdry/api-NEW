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
    public class RoleBL : IRoleBL
    {
        private readonly IUnitOfWork _uow;
        private SmartFanDbContext context;
        private IRepository<Role> repo;
        public RoleBL()
        {
            context = new SmartFanDbContext();
            _uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = _uow.GetRepository<Role>();
        }

        public bool DeleteRole(int roleId)
        {
            try
            {
                repo.Delete(roleId);
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Role GetRole(int Id)
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

        public IEnumerable<Role> GetRoles()
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

        public IEnumerable<Role> GetRoleByDescription(string description)
        {
            try
            {
                return repo.Queryable().Where(x => x.Description == description);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Role GetRoleByNameAndDescription(string roleName, string description)
        {
            try
            {
                return repo.Queryable().Where(x => x.Role1 == roleName && x.Description == description).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IQueryable<Role> QueryRole()
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

        public Role InsertRole(Role role)
        {
            try
            {
                repo.Add(role);
                _uow.SaveChanges();

                return role;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Role UpdateRole(Role role)
        {
            try
            {
                repo.Update(role);
                _uow.SaveChanges();

                return role;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
