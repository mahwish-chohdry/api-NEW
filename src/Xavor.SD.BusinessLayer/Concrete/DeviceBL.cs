using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.BusinessLayer
{
    public class DeviceBL : IDeviceBL
    {

        private readonly IUserBL _userBL;
        private readonly ICustomerBL _customerBL;
        private readonly IUserroleBL _userroleBL;
        private readonly IDevicegroupBL _devicegroupBL;
        private readonly IUserdeviceBL _userdeviceBL;
        private readonly IInverterBL _inverterBL;
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Device> repo;
        public DeviceBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Device>();
        }
        public DeviceBL(IUnitOfWork uow, IUserBL userBL, ICustomerBL customerBL, IUserroleBL userroleBL, IDevicegroupBL devicegroupBL, IUserdeviceBL userdeviceBL, IInverterBL inverterBL)
        {
            this.uow = uow;
            _userBL = userBL;
            _customerBL = customerBL;
            _userroleBL = userroleBL;
            _devicegroupBL = devicegroupBL;
            _userdeviceBL = userdeviceBL;
            _inverterBL = inverterBL;
            repo = uow.GetRepository<Device>();
        }
        public bool DeleteDevice(string deviceId)
        {
            try
            {
                var device = repo.GetList().Where(x => x.DeviceId == deviceId).FirstOrDefault();
                device.IsDeleted = 1;
                repo.Update(device);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public bool DeleteDevice(int deviceId)
        {
            try
            {
                repo.Delete(deviceId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public IEnumerable<Device> GetDevice()
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

        public IEnumerable<Device> GetDeviceAlongWithStatus()
        {
            try
            {
                return repo.Queryable().Include(x=>x.Devicestatus);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Device GetDevice(int Id)
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



        public IEnumerable<DeviceGroupBLDTO> GetDeviceGroup(int Customer_id)
        {
            try
            {
                if (Customer_id <= default(int))
                    throw new ArgumentException("Invalid id");
                IEnumerable<Device> devices = repo.Queryable().Include(t => t.Devicegroup).Where(x => x.CustomerId == Customer_id && x.IsDeleted == 0).ToList();
                IEnumerable<Devicegroup> devicegroups = uow.GetRepository<Devicegroup>().GetList();
                IEnumerable<Groups> dgroups = uow.GetRepository<Groups>().GetList().Where(x => x.CustomerId == Customer_id).ToList();
                var DeviceResult = from device in devices
                                   join groups in devicegroups on device.Id equals groups.DeviceId into g
                                   from dg in g.DefaultIfEmpty()
                                   select new
                                   {
                                       device.Id,
                                       device.CustomerId,
                                       device.IsActive,
                                       device.Name,
                                       device.IsInstalled,
                                       device.DeviceId,
                                       device.Appassword,
                                       device.Apssid,
                                       device.CurrentFirmwareVersion,
                                       device.LatestFirmwareVersion,
                                       device.InverterId,

                                       groupId = dg == null ? null : dg.GroupId,

                                   };
                var Result = from Device in DeviceResult
                             join groups in dgroups on Device.groupId equals groups.Id into d
                             from ed in d.DefaultIfEmpty()
                             select new
                             {
                                 Device.CustomerId,
                                 Device.IsActive,
                                 Device.Name,
                                 Device.DeviceId,
                                 gId = Device.groupId,
                                 Device.IsInstalled,
                                 dId = Device.Id,
                                 Device.Appassword,
                                 Device.Apssid,
                                 Device.CurrentFirmwareVersion,
                                 Device.LatestFirmwareVersion,
                                 Device.InverterId,
                                 GroupName = ed == null ? null : ed.Name,
                                 groupId = ed == null ? null : ed.GroupId,

                             };

                List<DeviceGroupBLDTO> DeviceDto = new List<DeviceGroupBLDTO>();
                foreach (var obj in Result)
                {
                    var temp = new DeviceGroupBLDTO();
                    var deviceInverter = new Inverter();
                    if (obj.InverterId != null)
                    {
                        deviceInverter = _inverterBL.GetInverter(Convert.ToInt32(obj.InverterId));
                    }
                    temp.customer_id = (int)obj.CustomerId;
                    temp.device_ap_name = obj.Name;
                    temp.device_id = obj.DeviceId;
                    temp.IsConfigured = (byte)obj.IsActive;
                    temp.IsInstalled = (byte)obj.IsInstalled;
                    temp.group_name = obj.GroupName;
                    temp.group_id = obj.groupId;
                    temp.gId = obj.gId;
                    temp.dId = obj.dId;
                    temp.Appassword = obj.Appassword;
                    temp.Apssid = obj.Apssid;
                    temp.CurrentFirmwareVersion = obj.CurrentFirmwareVersion;
                    temp.LatestFirmwareVersion = obj.LatestFirmwareVersion;
                    temp.InverterId = deviceInverter.InverterId;
                    DeviceDto.Add(temp);

                }
                return DeviceDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<DeviceGroupBLDTO> GetDeviceGroupList(int Customer_id)
        {
            try
            {
                if (Customer_id <= default(int))
                    throw new ArgumentException("Invalid id");
                IEnumerable<Device> devices = repo.Queryable().Include(t => t.Devicegroup).Where(x => x.CustomerId == Customer_id).ToList();
                IEnumerable<Devicegroup> devicegroups = uow.GetRepository<Devicegroup>().GetList();
                IEnumerable<Userdevice> userdevice = uow.GetRepository<Userdevice>().GetList();
                IEnumerable<User> usertable = uow.GetRepository<User>().GetList();

                IEnumerable<Groups> dgroups = uow.GetRepository<Groups>().GetList();
                var DeviceResult = from device in devices
                                   join groups in devicegroups on device.Id equals groups.DeviceId into g
                                   from dg in g.DefaultIfEmpty()
                                   select new
                                   {
                                       device.Id,
                                       device.CustomerId,
                                       device.IsActive,
                                       device.Name,
                                       groupId = dg == null ? null : dg.GroupId,

                                   };
                var Result = from Device in DeviceResult
                             join groups in dgroups on Device.groupId equals groups.Id into d
                             from ed in d.DefaultIfEmpty()
                             select new
                             {
                                 Device.CustomerId,
                                 Device.Id,
                                 Device.IsActive,
                                 Device.Name,
                                 Device.groupId,
                                 GroupName = ed == null ? "-1" : ed.Name,

                             };
                var Result2 = from result in Result
                              join userd in userdevice on result.Id equals userd.DeviceId into d
                              from ed in d.DefaultIfEmpty()
                              select new
                              {
                                  result.CustomerId,
                                  result.Id,
                                  result.IsActive,
                                  result.Name,
                                  result.GroupName,
                                  result.groupId,
                                  UserId = ed == null ? null : ed.UserId
                              };
                var Result3 = from result2 in Result2
                              join user in usertable on result2.UserId equals user.Id into d
                              from ed in d.DefaultIfEmpty()

                              select new
                              {
                                  result2.CustomerId,
                                  result2.Id,
                                  result2.IsActive,
                                  result2.Name,
                                  result2.GroupName,
                                  result2.UserId,
                                  result2.groupId,
                                  OperatorName = ed == null ? null : ed.Username

                              };



                List<DeviceGroupBLDTO> DeviceDto = new List<DeviceGroupBLDTO>();

                foreach (var obj in Result3)
                {
                    var temp = new DeviceGroupBLDTO();
                    temp.customer_id = (int)obj.CustomerId;
                    temp.device_ap_name = obj.Name;

                    temp.IsConfigured = (byte)obj.IsActive;
                    temp.group_name = obj.GroupName;
                    if (obj.groupId != null)
                    {
                        //temp.group_id = (int)obj.groupId;
                    }

                    if (obj.UserId != null)
                    {
                        temp.operator_id = (int)obj.UserId;
                        temp.operator_name = obj.OperatorName;
                    }

                    DeviceDto.Add(temp);


                }
                return DeviceDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<DeviceGroupBLDTO> GetDeviceGroupListById(int Customer_id, int group_id)
        {
            try
            {
                if (Customer_id <= default(int))
                    throw new ArgumentException("Invalid id");
                IEnumerable<Device> devices = repo.Queryable().Include(t => t.Devicegroup).Where(x => x.CustomerId == Customer_id).ToList();
                IEnumerable<Devicegroup> devicegroups = uow.GetRepository<Devicegroup>().GetList();
                IEnumerable<Groups> dgroups = uow.GetRepository<Groups>().GetList();
                var DeviceResult = from device in devices
                                   join groups in devicegroups on device.Id equals groups.DeviceId into g
                                   from dg in g.DefaultIfEmpty()
                                   select new
                                   {
                                       device.Id,
                                       device.CustomerId,
                                       device.IsActive,
                                       device.Name,
                                       groupId = dg == null ? null : dg.GroupId,

                                   };
                var secResult = from Device in DeviceResult
                                join groups in dgroups on Device.groupId equals groups.Id into d
                                from ed in d.DefaultIfEmpty()
                                select new
                                {
                                    Device.CustomerId,
                                    Device.Id,
                                    Device.IsActive,
                                    Device.Name,
                                    GroupId = ed == null ? -1 : (int?)ed.Id,
                                    GroupName = ed == null ? null : ed.Name,

                                };
                var Result = secResult.Where(x => x.GroupId == group_id || x.GroupId == -1);


                List<DeviceGroupBLDTO> DeviceDto = new List<DeviceGroupBLDTO>();

                foreach (var obj in Result)
                {
                    var temp = new DeviceGroupBLDTO();
                    temp.customer_id = (int)obj.CustomerId;
                    temp.device_ap_name = obj.Name;

                    temp.IsConfigured = (byte)obj.IsActive;
                    temp.group_name = obj.GroupName;
                    //temp.group_id = obj.GroupId.Value;
                    DeviceDto.Add(temp);
                }
                return DeviceDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<DeviceGroupBLDTO> GetOperatorById(int Customer_id, int user_id)
        {
            try
            {
                if (Customer_id <= default(int))
                    throw new ArgumentException("Invalid id");
                IEnumerable<Device> devices = repo.Queryable().Include(t => t.Devicegroup).Where(x => x.CustomerId == Customer_id).ToList();
                IEnumerable<Devicegroup> devicegroups = uow.GetRepository<Devicegroup>().GetList();
                IEnumerable<Userdevice> userdevice = uow.GetRepository<Userdevice>().GetList();
                IEnumerable<User> usertable = uow.GetRepository<User>().GetList();

                IEnumerable<Groups> dgroups = uow.GetRepository<Groups>().GetList();
                var DeviceResult = from device in devices
                                   join groups in devicegroups on device.Id equals groups.DeviceId into g
                                   from dg in g.DefaultIfEmpty()
                                   select new
                                   {
                                       device.Id,
                                       device.CustomerId,
                                       device.IsActive,
                                       device.Name,
                                       groupId = dg == null ? null : dg.GroupId,

                                   };
                var Result = from Device in DeviceResult
                             join groups in dgroups on Device.groupId equals groups.Id into d
                             from ed in d.DefaultIfEmpty()
                             select new
                             {
                                 Device.CustomerId,
                                 Device.Id,
                                 Device.IsActive,
                                 Device.Name,
                                 Device.groupId,
                                 GroupName = ed == null ? "-1" : ed.Name,

                             };
                var Result2 = from result in Result
                              join userd in userdevice on result.Id equals userd.DeviceId into d
                              from ed in d.DefaultIfEmpty()
                              select new
                              {
                                  result.CustomerId,
                                  result.Id,
                                  result.IsActive,
                                  result.Name,
                                  result.GroupName,
                                  result.groupId,
                                  UserId = ed == null ? null : ed.UserId
                              };
                var Result3 = from result2 in Result2
                              join user in usertable on result2.UserId equals user.Id into d
                              from ed in d.DefaultIfEmpty()

                              select new
                              {
                                  result2.CustomerId,
                                  result2.Id,
                                  result2.IsActive,
                                  result2.Name,
                                  result2.GroupName,
                                  result2.UserId,
                                  result2.groupId,
                                  OperatorName = ed == null ? null : ed.Username

                              };
                var FinalResult = Result3.Where(x => x.UserId == user_id);


                List<DeviceGroupBLDTO> DeviceDto = new List<DeviceGroupBLDTO>();

                foreach (var obj in FinalResult)
                {
                    var temp = new DeviceGroupBLDTO();
                    temp.customer_id = (int)obj.CustomerId;
                    temp.device_ap_name = obj.Name;

                    temp.IsConfigured = (byte)obj.IsActive;
                    temp.group_name = obj.GroupName;
                    if (obj.groupId != null)
                    {
                        //  temp.group_id = (int)obj.groupId;
                    }

                    if (obj.UserId != null)
                    {
                        temp.operator_id = (int)obj.UserId;
                        temp.operator_name = obj.OperatorName;
                    }

                    DeviceDto.Add(temp);


                }
                return DeviceDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<DeviceGroupByOperatorDTO> GetTotalDevicesByOperators(string customerId, string AdminUserId)
        {

            var customer = _customerBL.QueryCustomer().Where(x => x.CustomerId == customerId).FirstOrDefault();
            if (customer == null)
            {
                return null;
            }
            var admin = _userBL.QueryUsers().Where(x => x.UserId == AdminUserId).FirstOrDefault();
            //if (admin != null)
            //{
            //    var adminRole = _userroleBL.QueryUserrole().Where(x => x.UserId == admin.Id && x.RoleId == 1).FirstOrDefault();
            //    if (adminRole == null)
            //    {
            //        return null;
            //    }
            //}

            int actualCustomerId = customer.Id;
            var users = _userBL.QueryUsers().Where(x => x.CustomerId == actualCustomerId);
            var usersrole = _userroleBL.QueryUserrole();
            if (users == null || users.Count() == 0)
            {
                return null;
            }

            var usersOperators = from t1 in users
                                 join t2 in usersrole on t1.Id equals t2.UserId
                                 where t2.RoleId == 2
                                 select new
                                 {
                                     t1.Id,
                                     t1.Username,
                                     t1.ProfilePicture,
                                     t1.UserId
                                 };


            //IEnumerable<Userdevice> userdevice = uow.GetRepository<Userdevice>().GetList();


            var AllOperators = from user in usersOperators
                               join ud in _userdeviceBL.QueryDevice() on user.Id equals ud.UserId into g
                               from dg in g.DefaultIfEmpty()
                               select new
                               {
                                   user.Id,
                                   user.Username,
                                   user.ProfilePicture,
                                   user.UserId,
                                   UserIdInDG = dg == null ? null : dg.UserId

                               };
            var DistinctOperators = AllOperators.Distinct();
            List<DeviceGroupByOperatorDTO> response = new List<DeviceGroupByOperatorDTO>();
            foreach (var obj in DistinctOperators)
            {
                var dto = new DeviceGroupByOperatorDTO();
                dto.userId = obj.UserId;
                dto.userName = obj.Username;
                dto.profilePicture = obj.ProfilePicture;
                if (obj.UserIdInDG != null)
                {
                    dto.totaldevices = AllOperators.Where(x => x.UserIdInDG == obj.Id).Count();
                }
                else
                {
                    dto.totaldevices = 0;
                }


                response.Add(dto);
            }

            return response;
        }

        public IEnumerable<UnGroupedDevicesDTO> GetUnGroupedDevices(int customer_id)
        {
            IEnumerable<Device> devices = repo.Queryable().Include(t => t.Devicegroup).Where(x => x.CustomerId == customer_id).ToList();
            List<UnGroupedDevicesDTO> unGroupedDevices = new List<UnGroupedDevicesDTO>();
            foreach (var device in devices)
            {
                UnGroupedDevicesDTO dto = new UnGroupedDevicesDTO();
                if (device.Devicegroup.Count == 0)
                {
                    dto.customer_id = device.CustomerId.Value;
                    dto.device_ap_name = device.Name;
                    dto.device_id = device.Id;
                    dto.isActive = Convert.ToByte(device.IsActive);
                    unGroupedDevices.Add(dto);
                }
            }
            return unGroupedDevices;
        }

        public IEnumerable<DeviceGroupDTO> GetUserGroupsDevices(int Customer_id, int user_id)
        {

            throw new NotImplementedException();
        }

        public Device InsertDevice(Device device)
        {
            try
            {
                var customer = uow.GetRepository<Customer>().Find((int)device.CustomerId);
                device.CreatedBy = customer.Name;
                repo.Add(device);
                uow.SaveChanges();

                return device;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Device> QueryDevice()
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

        public Device UpdateDevice(Device device)
        {
            try
            {
                var customer = uow.GetRepository<Customer>().Find((int)device.CustomerId);
                device.CreatedBy = customer.Name;
                repo.Update(device);
                uow.SaveChanges();
                return device;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        IEnumerable<DeviceGroupDTO> IDeviceBL.GetDeviceGroupList(int Customer_id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<DeviceGroupDTO> IDeviceBL.GetDeviceGroupListById(int customer_id, int group_id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<DeviceGroupDTO> IDeviceBL.GetOperatorById(int Customer_id, int user_id)
        {
            throw new NotImplementedException();
        }

        public Device GetDevice(string deviceId)
        {
            try
            {
                return repo.Queryable().Where(x => x.DeviceId.Equals(deviceId)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Device GetExistingDeviceByCustomerId(string customerId)
        {
            var existingDevice = (from d in QueryDevice()
                                  join c in _customerBL.QueryCustomer() on d.CustomerId equals c.Id
                                  where c.CustomerId == customerId
                                  select d).FirstOrDefault();
            return existingDevice;
        }

        public Device GetOffBoardDeviceByDeviceId(string deviceUniqueId)
        {
            var offBoardDevice = QueryDevice().Where(x => x.IsInstalled == 0 && x.DeviceId == deviceUniqueId).FirstOrDefault();
            return offBoardDevice;
        }

        public string GetExistingGroupAssociationByDeviceId(string deviceUniqueId)
        {
            var deviceId = (from d in QueryDevice()
                            join dg in _devicegroupBL.QueryDevicegroup() on d.Id equals dg.DeviceId
                            where d.DeviceId == deviceUniqueId
                            select d.DeviceId).FirstOrDefault();
            return deviceId;
        }

        public IEnumerable<Device> GetDevicesByCustomerId(int customerId)
        {
            var devices = repo.Queryable().Where(x => x.CustomerId == customerId);
            return devices;
        }

        public IEnumerable<Device> GetDeviceAlongWithStatus(int customerId)
        {
            var devices = repo.Queryable().Where(x => x.CustomerId == customerId).Include(x=>x.Devicestatus) ;
            return devices;
        }

        

        public IEnumerable<Device> GetDevicesByCustomerIdUserId(int customerId, int userId)
        {
            var customerDevices = repo.Queryable().Where(x => x.CustomerId == customerId);
            var allDevices = customerDevices;
            //var allDevices = from device in customerDevices
            //                 join ud in _userdeviceBL.QueryDevice() on device.Id equals ud.DeviceId
            //                 where ud.UserId == userId
            //                 select device;

            return allDevices;
        }

        public Device GetDeviceByUniqueId(string deviceId)
        {
            var device = QueryDevice().Where(x => x.DeviceId == deviceId).FirstOrDefault();
            return device;
        }

        public int GetDeviceDBId(string deviceId)
        {
            var deviceDbId = QueryDevice().Where(x => x.DeviceId == deviceId).Select(s => s.Id).FirstOrDefault();
            return deviceDbId;
        }

        public IEnumerable<Device> UpdateDevices(IEnumerable<Device> devices)
        {
            repo.UpdateList(devices);
            uow.SaveChanges();
            return devices;
        }

        public List<Device> GetDevicesByCustomerBatchId(string customerId, string batchId)
        {
            var customer = _customerBL.QueryCustomer().Where(x => x.CustomerId == customerId).FirstOrDefault();
            return repo.Queryable().Where(x => x.CustomerId == customer.Id && x.BatchId == batchId).ToList();

        }

        public List<Device> GetDevicesByCustomer(string customerId)
        {
            var customer = _customerBL.QueryCustomer().Where(x => x.CustomerId == customerId).FirstOrDefault();
            return repo.Queryable().Where(x => x.CustomerId == customer.Id).ToList();
        }

        public List<Device> GetDevicesByBatchId(string batchId)
        {

            return repo.Queryable().Where(x => x.BatchId == batchId).ToList();
        }
    }
}
