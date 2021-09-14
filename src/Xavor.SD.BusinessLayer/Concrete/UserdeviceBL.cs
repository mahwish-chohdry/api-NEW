using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;

namespace Xavor.SD.BusinessLayer.Concrete
{
    public class UserdeviceBL : IUserdeviceBL
    {
        private readonly IUnitOfWork _uow;
        private SmartFanDbContext context;
        private IRepository<Userdevice> repository;

        public UserdeviceBL()
        {
            context = new SmartFanDbContext();
            _uow = new UnitOfWork<SmartFanDbContext>(context);
            repository = _uow.GetRepository<Userdevice>();
        }


        public bool DeleteUserDevicesByUserId(int UserId)
        {
            var Userdevice = QueryDevice().Where(dg => dg.UserId == UserId).ToList<Userdevice>();
            if (Userdevice.Count != 0)
            {
                foreach (var dg in Userdevice)
                {
                    DeleteUserDevice(dg.Id);
                }

                return true;
            }
            return false;

        }
        public bool DeleteUserDevice(int UserDeviceId)
        {
            try
            {
                repository.Delete(UserDeviceId);
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Userdevice> GetUserDevice()
        {
            throw new NotImplementedException();
        }

        public Userdevice InsertUserDevice(Userdevice userdevice)
        {
            try
            {

                repository.Add(userdevice);
                _uow.SaveChanges();

                return userdevice;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IQueryable<Userdevice> QueryDevice()
        {
            try
            {
                return repository.Queryable();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Userdevice UpdateUserDevice(Userdevice userdevice)
        {
            throw new NotImplementedException();
        }

        public bool UserDevice(int userdeviceId)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Userdevice> GetUserDeviceMappingsByUser(int userId)
        {
            var userDeviceMaping = QueryDevice().Where(x => x.UserId == userId);
            return userDeviceMaping;
        }

        public bool DeleteUserDeviceByDeviceId(int deviceId)
        {
            var Userdevices = QueryDevice().Where(dg => dg.DeviceId == deviceId).ToList<Userdevice>();
            if (Userdevices.Count != 0)
            {
                foreach (var dg in Userdevices)
                {
                    DeleteUserDevice(dg.Id);
                }

                return true;
            }
            return false;

        }
    }
}
