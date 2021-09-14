using System;
using System.Collections.Generic;
using System.Linq;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;

namespace Xavor.SD.BusinessLayer
{
    public class DeviceAlarmsHistoryBL : IDeviceAlarmsHistoryBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Devicealarmshistory> repo;
        public DeviceAlarmsHistoryBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Devicealarmshistory>();
        }
        public bool DeleteDevicealarmshistory(int DevicealarmshistoryId)
        {
            try
            {
                repo.Delete(DevicealarmshistoryId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Devicealarmshistory> GetDevicealarmshistory()
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

        public Devicealarmshistory GetDevicealarmshistory(int Id)
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
        public Devicealarmshistory InsertDevicealarmshistory(Devicealarmshistory Devicealarmshistory)
        {
            try
            {
                repo.Add(Devicealarmshistory);
                uow.SaveChanges();

                return Devicealarmshistory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Devicealarmshistory> GetAlarmWarningReport(int deviceId,string customerId)
        {
            
            try
            {
                var devicealarms = from alarm in context.Devicealarmshistory
                where alarm.Warning != "No Warning" || alarm.Alarm != "No Alarm" && alarm.DeviceId == deviceId
                select alarm;
                return devicealarms.ToList();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IQueryable<Devicealarmshistory> QueryDevicealarmshistory()
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

        public Devicealarmshistory UpdateDevicealarmshistory(Devicealarmshistory Devicealarmshistory)
        {
            try
            {
                repo.Update(Devicealarmshistory);
                uow.SaveChanges();
                return Devicealarmshistory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Devicealarmshistory> GetDevicealarmshistoryByIssueType(int deviceId, string issueType)
        {
            List<Devicealarmshistory> alarmHistoriesFromDb = null;
            if (issueType == "1")
            {
                alarmHistoriesFromDb = QueryDevicealarmshistory().Where(x => x.DeviceId == deviceId && x.Alarm != "No Alarm").OrderByDescending(x => x.CreatedDate).ToList();
            }
            else if(issueType == "0")
            {
                alarmHistoriesFromDb = QueryDevicealarmshistory().Where(x => x.DeviceId == deviceId && x.Warning != "No Warning").OrderByDescending(x => x.CreatedDate).ToList();
            }

            return alarmHistoriesFromDb;
        }

        public bool DeleteDeviceAlarmshistoryByDeviceId(int deviceId)
        {
            var devicealarmshistories = QueryDevicealarmshistory().Where(dg => dg.DeviceId == deviceId).ToList<Devicealarmshistory>();
            if (devicealarmshistories.Count != 0)
            {
                foreach (var dg in devicealarmshistories)
                {
                    DeleteDevicealarmshistory(dg.Id);
                }

                return true;
            }
            return false;

        }
    }
}
