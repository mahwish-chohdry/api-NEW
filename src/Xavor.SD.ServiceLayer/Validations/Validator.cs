using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.BusinessLayer.Concrete;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer.Validations
{
    public static class Validator
    {
        public static Customer ValidateCustomer(string customerUniqueId)
        {
            ICustomerBL _customerBL = new CustomerBL();

            var customer = _customerBL.QueryCustomer().Where(x => x.CustomerId == customerUniqueId).FirstOrDefault();
            if (customer == null || string.IsNullOrEmpty(customer.CustomerId) || customer.Id == 0)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Invalid customer Id: [{customerUniqueId}]",
                    Data = null
                });
            }
            return customer;
        }


        public static User ValidateUser(string customerUniqueId, string userId, string userMsg = "User")
        {
            ICustomerBL _customerBL = new CustomerBL();
            IUserBL _userBL = new UserBL();

            var user = (from c in _customerBL.QueryCustomer()
                        join u in _userBL.QueryUsers() on c.Id equals u.CustomerId
                        where c.CustomerId == customerUniqueId && u.UserId == userId
                        select u).FirstOrDefault();
            if (user == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"" + userMsg + $": [{userId}] does not belong to customer [{customerUniqueId}]",
                    Data = null
                });
            }
            return user;
        }

        public static Device ValidateDevice(string customerUniqueId, string deviceId)
        {
            ICustomerBL _customerBL = new CustomerBL();
            IDeviceBL _deviceBL = new DeviceBL();

            var device = (from c in _customerBL.QueryCustomer()
                          join u in _deviceBL.QueryDevice() on c.Id equals u.CustomerId
                          where c.CustomerId == customerUniqueId && u.DeviceId == deviceId
                          select u).FirstOrDefault();
            if (device == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Device: [{deviceId}] does not belong to customer [{customerUniqueId}]",
                    Data = null
                });
            }
            return device;
        }

        public static Groups ValidateGroup(string customerUniqueId, string groupId)
        {
            ICustomerBL _customerBL = new CustomerBL();
            IGroupsBL _groupsBL = new GroupsBL();

            var group = (from g in _groupsBL.QueryGroups()
                         where g.GroupId == groupId
                         select g).FirstOrDefault();
            if (group == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "No Content",
                    Message = $"Group does not exist anymore.",
                    Data = null
                });
            }

            var customerGroup = (from c in _customerBL.QueryCustomer()
                                 where c.CustomerId == customerUniqueId && c.Id == @group.CustomerId
                                 select c).FirstOrDefault();
            if (customerGroup == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Group: [{groupId}] does not belong to customer [{customerUniqueId}]",
                    Data = null
                });
            }
            return group;
        }

        public static void ValidatePassword(string password, string adminUserId)
        {
            if (password.Length < 6)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"The user [{adminUserId}] password is not valid",
                });
            }
        }

        public static User ValidateAdmin(string customerUniqueId, string adminUserId)
        {
            var user = Validator.ValidateUser(customerUniqueId, adminUserId, "Admin");
            if (!IsAdmin(adminUserId))
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"The user [{adminUserId}] is not an admin",
                    Data = null
                });
            }
            else
            {
                return user;
            }
        }
        public static User ValidateOperator(string customerUniqueId, string operatorId)
        {
            var user = Validator.ValidateUser(customerUniqueId, operatorId, "Operator");
            if (!IsOperator(operatorId))
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"The user [{operatorId}] is not an operator",
                    Data = null
                });
            }
            else
            {
                return user;
            }
        }

        public static void ValidateDeviceName(string deviceName, string existingDeviceId)
        {
            IDeviceBL _deviceBL = new DeviceBL();
            var alreadyExist = _deviceBL.QueryDevice().Where(x => x.Name == deviceName).FirstOrDefault();
            if (alreadyExist != null && existingDeviceId != alreadyExist.DeviceId)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"The device Name \"{deviceName}\" already exist.",
                    Data = null
                });
            }
        }

        public static void ValidateGroupName(string groupName, string existingActualGroupId, int customerId)
        {
            IGroupsBL _groupBL = new GroupsBL();
            var existingGroup = _groupBL.GetGroupByNameAndCustomer(groupName, customerId);

            if (existingGroup != null && existingGroup.GroupId != existingActualGroupId)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"\"{groupName}\" group already exists. Please change the name of the group.",
                    Data = null
                });
            }
            else if (string.IsNullOrEmpty(groupName))
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Invalid group name.",
                    Data = null
                });
            }
        }

        public static Userdevice ValidateUserDevice(string userId, string deviceId)
        {
            IUserBL _userBL = new UserBL();
            IUserdeviceBL _userdeviceBL = new UserdeviceBL();
            IDeviceBL _deviceBL = new DeviceBL();

            var ud = (from u in _userBL.QueryUsers()
                      join ur in _userdeviceBL.QueryDevice() on u.Id equals ur.UserId
                      join r in _deviceBL.QueryDevice() on ur.DeviceId equals r.Id
                      where u.UserId == userId && r.DeviceId == deviceId
                      select ur).FirstOrDefault();
            if (ud == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Device: [{deviceId}] does not belong to user [{userId}]",
                    Data = null
                });
            }
            return ud;
        }

        private static bool IsAdmin(string adminUserId)
        {
            IUserBL _userBL = new UserBL();
            IUserroleBL _userroleBL = new UserroleBL();
            IRoleBL _roleBL = new RoleBL();

            if (string.IsNullOrEmpty(adminUserId))
            {
                return false;
            }

            var role = (from u in _userBL.QueryUsers()
                        join ur in _userroleBL.QueryUserrole() on u.Id equals ur.UserId
                        join r in _roleBL.QueryRole() on ur.RoleId equals r.Id
                        where u.UserId == adminUserId
                        select r.Role1).FirstOrDefault();

            if (role == "Admin")
            {
                return true;
            }

            return false;
        }

        public static Inverter ValidateInverter(string invertorId)
        {
          IInverterBL _inverterBL = new InverterBL();
           var inverter = _inverterBL.QueryInverter().Where(x => x.InverterId == invertorId).FirstOrDefault();
            if (inverter == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Inverter does not exist",
                    Data = null
                });
            }
            else
            {
                return inverter;
            }


        }
        private static bool IsOperator(string operatorId)
        {
            IUserBL _userBL = new UserBL();
            IUserroleBL _userroleBL = new UserroleBL();
            IRoleBL _roleBL = new RoleBL();

            if (string.IsNullOrEmpty(operatorId))
            {
                return false;
            }

            var role = (from u in _userBL.QueryUsers()
                        join ur in _userroleBL.QueryUserrole() on u.Id equals ur.UserId
                        join r in _roleBL.QueryRole() on ur.RoleId equals r.Id
                        where u.UserId == operatorId
                        select r.Role1).FirstOrDefault();

            if (role == "Operator")
            {
                return true;
            }

            return false;
        }
    }
}
