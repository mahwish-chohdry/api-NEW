using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Common;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.BusinessLayer
{
    public interface IDeviceBL
    {
        Device InsertDevice(Device device);
        Device UpdateDevice(Device user);
        IEnumerable<Device> GetDevice();
        IEnumerable<Device> GetDeviceAlongWithStatus();
        IEnumerable<Device> GetDeviceAlongWithStatus(int customerId);
        bool DeleteDevice(string deviceId);
        Device GetDevice(int Id);
        Device GetDevice(string  deviceId);
        IQueryable<Device> QueryDevice();
        bool DeleteDevice(int deviceId);
        IEnumerable<DeviceGroupDTO> GetDeviceGroupList(int Customer_id);
        IEnumerable<UnGroupedDevicesDTO> GetUnGroupedDevices(int customer_id);

        IEnumerable<DeviceGroupDTO> GetDeviceGroupListById(int customer_id, int group_id);

        IEnumerable<DeviceGroupDTO> GetOperatorById(int Customer_id, int user_id);
        IEnumerable<DeviceGroupDTO> GetUserGroupsDevices(int Customer_id, int user_id);
        IEnumerable<DeviceGroupBLDTO> GetDeviceGroup(int Customer_id);
        IEnumerable<DeviceGroupByOperatorDTO> GetTotalDevicesByOperators(string customerId, string AdminUserId);

        Device GetExistingDeviceByCustomerId(string customerId);
        Device GetOffBoardDeviceByDeviceId(string deviceUniqueId);
        string GetExistingGroupAssociationByDeviceId(string deviceUniqueId);
        IEnumerable<Device> GetDevicesByCustomerId(int customerId);
        IEnumerable<Device> GetDevicesByCustomerIdUserId(int customerId, int userId);
        Device GetDeviceByUniqueId(string deviceId);
        int GetDeviceDBId(string deviceId);
        List<Device> GetDevicesByCustomerBatchId(string customerId,string batchId);
        List<Device> GetDevicesByCustomer(string customerId);
        List<Device> GetDevicesByBatchId(string batchId);
        IEnumerable<Device> UpdateDevices(IEnumerable<Device> devices);

    }
}
