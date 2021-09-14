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
    public class DevicegroupBL : IDevicegroupBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Devicegroup> repo;
        public DevicegroupBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Devicegroup>();
        }
        public bool DeleteDevicegroup(int DevicegroupId)
        {
            try
            {
                repo.Delete(DevicegroupId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public bool DeleteDeviceGroupByGroupId(int group_id)
        {
            var deviceGroups = QueryDevicegroup().Where(dg => dg.GroupId == group_id).ToList<Devicegroup>();
            if(deviceGroups.Count != 0)
            {
                foreach (var dg in deviceGroups)
                {
                    DeleteDevicegroup(dg.Id);
                }

                return true;
            }
            return false;
           
        }

        public IEnumerable<Devicegroup> GetDevicegroup()
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

        public Devicegroup GetDevicegroup(int Id)
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

        public Devicegroup InsertDevicegroup(Devicegroup Devicegroup)
        {
            try
            {
                repo.Add(Devicegroup);
                uow.SaveChanges();

                return Devicegroup;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Devicegroup> QueryDevicegroup()
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

        public bool RemoveDeviceFromGroup(int group_id, List<int> device_ids)
        {

            bool isDeleted = false;
            foreach (var device_id in device_ids)
            {
                var deviceGroups = QueryDevicegroup().Where(dg => dg.GroupId == group_id && dg.DeviceId == device_id).FirstOrDefault();             
                if (deviceGroups != null)
                {
                    isDeleted = DeleteDevicegroup(deviceGroups.Id);
                }
                
            }


            return isDeleted;
        }

        public Devicegroup UpdateDevicegroup(Devicegroup Devicegroup)
        {
            try
            {
                repo.Update(Devicegroup);
                uow.SaveChanges();
                return Devicegroup;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteDeviceGroupByDeviceId(int deviceId)
        {
            var Devicegroups = QueryDevicegroup().Where(dg => dg.DeviceId == deviceId).ToList<Devicegroup>();
            if (Devicegroups.Count != 0)
            {
                foreach (var dg in Devicegroups)
                {
                    DeleteDevicegroup(dg.Id);
                }

                return true;
            }
            return false;

        }
    }
}
