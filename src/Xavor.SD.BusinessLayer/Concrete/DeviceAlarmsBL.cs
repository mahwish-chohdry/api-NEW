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
    public class DevicealarmsBL : IDevicealarmsBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Devicealarms> repo;
        public DevicealarmsBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Devicealarms>();
        }
        public bool DeleteDevicealarms(int DevicealarmsId)
        {
            try
            {
                repo.Delete(DevicealarmsId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Devicealarms> GetDevicealarms()
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

        public Devicealarms GetDevicealarms(int Id)
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

        public Devicealarms InsertDevicealarms(Devicealarms Devicealarms)
        {
            try
            {
                repo.Add(Devicealarms);
                uow.SaveChanges();

                return Devicealarms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Devicealarms> QueryDevicealarms()
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

        public Devicealarms UpdateDevicealarms(Devicealarms Devicealarms)
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
        public Devicealarms GetDevicealarmsByDeviceId(string deviceId, string customerUniqueId)
        {
            var storedDevicestatus = QueryDevicealarms().Where(x => x.DeviceId == deviceId && x.CustomerId == customerUniqueId).FirstOrDefault();
            return storedDevicestatus;
        }

        public bool DeleteDeviceAlarmsByDeviceId(string deviceId)
        {
            var Devicealarmslist = QueryDevicealarms().Where(dg => dg.DeviceId == deviceId).ToList<Devicealarms>();
            if (Devicealarmslist.Count != 0)
            {
                foreach (var dg in Devicealarmslist)
                {
                    DeleteDevicealarms(dg.Id);
                }

                return true;
            }
            return false;

        }
    }
}
