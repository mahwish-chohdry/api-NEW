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
    public class DefaultSettingsBL : IDefaultSettingsBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Defaultsettings> repo;
        public DefaultSettingsBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Defaultsettings>();
        }
        public bool DeleteDefaultsettings(int DefaultsettingsId)
        {
            try
            {
                repo.Delete(DefaultsettingsId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Defaultsettings> GetDefaultsettings()
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

        public Defaultsettings GetDefaultsettings(int Id)
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

        public Defaultsettings InsertDefaultsettings(Defaultsettings Defaultsettings)
        {
            try
            {
                repo.Add(Defaultsettings);
                uow.SaveChanges();

                return Defaultsettings;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Defaultsettings> QueryDefaultsettings()
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

        public Defaultsettings UpdateDefaultsettings(Defaultsettings Defaultsettings)
        {
            try
            {
                repo.Update(Defaultsettings);
                uow.SaveChanges();
                return Defaultsettings;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
