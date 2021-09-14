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
    public class PermissionBL : IPermissionBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Permission> repo;

        public PermissionBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Permission>();
        }

        public bool DeletePermission(int PermissionId)
        {
            try
            {
                repo.Delete(PermissionId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Permission GetPermission(int Id)
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

        public IEnumerable<Permission> GetPermission()
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

        public IQueryable<Permission> QueryPermission()
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

        public Permission InsertPermission(Permission permission)
        {
            try
            {
                repo.Add(permission);
                uow.SaveChanges();

                return permission;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Permission UpdatePermission(Permission permission)
        {
            try
            {
                repo.Update(permission);
                uow.SaveChanges();

                return permission;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
