
using System;
using System.Collections.Generic;
using Xavor.SD.Common;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.ServiceLayer
{
    public interface IDeviceService
    {
        void AddDevicesToOperator(int userId, List<string> deviceUniqueIdList);
        void AddRoleToOperator(User user);
        bool DeleteDevice(string customerId, string Device_id);
        void DeleteUserDevicesByUserId(int userId);
        List<DeviceDTO> GetDeviceStats(string customerId, string userId);
        IEnumerable<Device> GetAllDevices(string customerId);
        IEnumerable<Device> GetAllDevices(int Customer_id);
        IEnumerable<DeviceGroupDTO> GetAllDevices(string customerID, string userId);
        IEnumerable<UserDeviceDTO> GetAllUserDevices(string customerID, string userId, string operatorId, bool alldevices = true);
        IEnumerable<UserDeviceDTO> GetDevices(string customerID, string userId, string operatorId = null, bool getAllDevices = false, bool UnGrouped = false, string groupId = null);
        IEnumerable<DeviceGroupByOperatorDTO> GetTotalDevicesByOperators(string customerId, string AdminUserId);
        IEnumerable<UnGroupedDevicesDTO> GetUnGroupedDevices(int customer_id);
        void GroupFullAccessPermission(List<DeviceGroupDTO> data);
        bool SaveUserDevices(UserDeviceDTO userDevices, string customerUniqueId, string adminUserId);
        bool UpdateUserDevices(UserDeviceDTO userDevices, string customerUniqueId, string adminUserId);
        bool SendDevicecommand(StatusDTO deviceStatus, string customerId, string userId, string deviceId);
        bool SendGroupcommand(StatusDTO deviceSatus, string customerUniqueId, string userId, string groupId);
        bool SendAcknowledgement(string commandId);
        Devicestatus AddOrUpdateDevicestatus(SmartDeviceContainer smartDevices);
        GroupcommandDTO GetGroupcommand(string customerUniqueId, string userId, string groupId);
        DevicecommandDTO GetDevicecommand(string customerUniqueId, string userId, string deviceId);
        void ResetRunningHours(string customerUniqueId, string userId, string deviceId);
        void ResetGroupRunningHours(string customerUniqueId, string userId, string groupId);
        IEnumerable<UsageDTO> GetDeviceUsage(string customerUniqueId, string userId, string deviceId, string date, int days);
        IEnumerable<UsageDTO> GetAllUsage(string customerUniqueId, string userId, string date, int days);
        Devicestatus GetDevicestatus(string customerId, string deviceID);
        Devicealarms GetDevicealarms(string customerUniqueId, string deviceId);
        void SendDeviceState(string customerUniqueId, string deviceId);
        bool CustomizeDeviceName(string customerId, string userId, string deviceId, string deviceName);
        bool CustomizeDeviceName(string customerId, string deviceId, string deviceName);
        Device GetDeviceByUniqueId(string deviceId);
        bool ChangeDeviceSpeed(string customerId, string deviceId, int speed);
        bool ChangeDevicePowerStatus(string customerId, string deviceId, short powerStatus);
        bool DeleteDeviceById(string customerId, string deviceId);

        IEnumerable<Devicebatchnumber> GetAllBatchNumber();
        bool UpdateDeviceFirmware(string firmwareVersion, string batchId);
        bool UpdateDeviceState(string firmwareVersion, string deviceId,string inverterId);

        IEnumerable<UsageDTO> GetDeviceUsage(string customerUniqueId, string deviceId, string date);
    }
}
