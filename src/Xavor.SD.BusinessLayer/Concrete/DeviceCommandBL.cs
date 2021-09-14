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
    public class DevicecommandBL : IDevicecommandBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Devicecommand> repo;
        public DevicecommandBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Devicecommand>();
        }
        public bool DeleteDevicecommand(int DevicecommandId)
        {
            try
            {
                repo.Delete(DevicecommandId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Devicecommand> GetDevicecommand()
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

        public Devicecommand GetDevicecommand(int Id)
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

        public Devicecommand InsertDevicecommand(Devicecommand Devicecommand)
        {
            try
            {
                repo.Add(Devicecommand);
                uow.SaveChanges();

                return Devicecommand;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Devicecommand> QueryDevicecommand()
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

        public Devicecommand UpdateDevicecommand(Devicecommand Devicecommand)
        {
            try
            {
                repo.Update(Devicecommand);
                uow.SaveChanges();
                return Devicecommand;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void AddOrUpdateDevicecommand(Devicecommand deviceCommand)
        {
            var existingDevicecommand = QueryDevicecommand().Where(x => x.DeviceId == deviceCommand.DeviceId && x.CustomerId == deviceCommand.CustomerId).FirstOrDefault();
            if (existingDevicecommand != null) // Update case
            {
                existingDevicecommand.DeviceId = deviceCommand.DeviceId;
                existingDevicecommand.CommandHistoryId = deviceCommand.CommandHistoryId;
                existingDevicecommand.IsGrouped = deviceCommand.IsGrouped;
                existingDevicecommand.ModifiedDate = DateTime.UtcNow;
                UpdateDevicecommand(existingDevicecommand);
            }
            else // Insert case
            {
                InsertDevicecommand(deviceCommand);
            }
        }

        public Devicecommand GetGroupedDevicecommand(int deviceId)
        {
            var deviceCommad = QueryDevicecommand().Where(x => x.DeviceId == deviceId && x.IsGrouped == 1).FirstOrDefault();
            return deviceCommad;
        }

        public bool DeleteDevicecommandByDeviceId(int deviceId)
        {
            var Devicecommands = QueryDevicecommand().Where(dg => dg.DeviceId == deviceId).ToList<Devicecommand>();
            if (Devicecommands.Count != 0)
            {
                foreach (var dg in Devicecommands)
                {
                    DeleteDevicecommand(dg.Id);
                }

                return true;
            }
            return false;

        }
    }
}
