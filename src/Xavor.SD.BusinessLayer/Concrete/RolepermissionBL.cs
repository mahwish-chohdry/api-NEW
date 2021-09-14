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
    public class RolepermissionBL : IRolepermissionBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Rolepermission> repo;

        public RolepermissionBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Rolepermission>();
        }

        public bool DeleteRolepermission(int rolepermissionId)
        {
            try
            {
                repo.Delete(rolepermissionId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Rolepermission GetRolepermission(int Id)
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

        public IEnumerable<Rolepermission> GetRolepermissions()
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

        public IQueryable<Rolepermission> QueryRolepermission()
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

        public Rolepermission InsertRolepermission(Rolepermission rolepermission)
        {
            try
            {
                repo.Add(rolepermission);
                uow.SaveChanges();

                return rolepermission;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Rolepermission UpdateRolepermission(Rolepermission rolepermission)
        {
            try
            {
                repo.Update(rolepermission);
                uow.SaveChanges();

                return rolepermission;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
