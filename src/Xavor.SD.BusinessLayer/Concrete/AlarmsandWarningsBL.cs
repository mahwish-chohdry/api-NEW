using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;

namespace Xavor.SD.BusinessLayer
{
    public class AlarmsandwarningsBL : IAlarmsandwarningsBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Alarmsandwarnings> repo;
        public AlarmsandwarningsBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Alarmsandwarnings>();
        }
        public bool DeleteAlarmsandwarnings(int alarmsId)
        {
            try
            {
                repo.Delete(alarmsId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Alarmsandwarnings> GetAlarmsandwarnings()
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

        public IEnumerable<Alarmsandwarnings> GetAlarmsandwarnings(string lang,int inverterId)
        {
            try
            {
                return repo.Queryable().Where(x => x.Language == lang&& x.InverterId == inverterId ).Include(x=>x.Inverter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<Alarmsandwarnings> GetAlarmsandwarnings(string lang)
        {
            try
            {
                return repo.Queryable().Where(x => x.Language == lang ).Include(x => x.Inverter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Alarmsandwarnings GetAlarmsandwarnings(int Id)
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

        public Alarmsandwarnings GetAlarmAndWarningByCode(string code, string lang)
        {
            try
            {
                return repo.Queryable().Where(x => x.Code == code && x.Language == lang).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Alarmsandwarnings InsertAlarmsandwarnings(Alarmsandwarnings alarmsandWarnings)
        {
            try
            {
                repo.Add(alarmsandWarnings);
                uow.SaveChanges();

                return alarmsandWarnings;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Alarmsandwarnings> QueryAlarmsandwarnings()
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

        public Alarmsandwarnings UpdateAlarmsandwarnings(Alarmsandwarnings Devicealarms)
        {
            try
            {
                repo.Update(Devicealarms);
                uow.SaveChanges();
                return Devicealarms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
