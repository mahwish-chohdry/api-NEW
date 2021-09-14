using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.ServiceLayer;
using Xavor.SD.WebAPI.ViewContracts;
using Xavor.SD.BusinessLayer;
using Xavor.SD.Common.Utilities;
using Xavor.SD.ServiceLayer.Validations;
using Warnings = Xavor.SD.Common.Utilities.Warnings;
using Alarms = Xavor.SD.Common.Utilities.Alarms;
using Xavor.SD.ServiceLayer.Transformations;

namespace Xavor.SD.ServiceLayer
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceBL _deviceBL;
        private readonly IUserBL _userBL;
        private readonly IGroupsBL _groupsBL;
        private readonly IUserroleBL _userroleBL;
        private readonly IRoleBL _roleBL;
        private readonly IUserdeviceBL _userdeviceBL;
        private readonly ICustomerBL _customerBL;
        private readonly ICommandhistoryBL _commandhistoryBL;
        private readonly IDevicestatusBL _deviceStatusBL;
        private readonly IDevicestatushistoryBL _deviceStatusHistoryBL;
        private readonly IIOTHubService _iotHubService;
        private readonly IGroupcommandBL _groupCommandBL;
        private readonly IDevicecommandBL _deviceCommandBL;
        private readonly IAlarmsandwarningsBL _alarmsandWarningsBL;
        private readonly IDevicealarmsBL _devicealarmsBL;
        private readonly ITransformations _transformations;
        private readonly IDeviceAlarmsHistoryBL _deviceAlarmsHistoryBL;
        private readonly IDevicegroupBL _devicegroupBL;
        private readonly IDefaultSettingsBL _defaultSettingsBL;
        private readonly IDeviceBatchNumberBL _deviceBatchNumberBL;
        private readonly IInverterBL _inverterBL;


        public DeviceService(IDeviceBL deviceBL, IUserBL userBL, IGroupsBL groupsBL, IUserroleBL userroleBL, IUserdeviceBL userdeviceBL, ICustomerBL customerBL, IRoleBL roleBL, ICommandhistoryBL commandhistoryBL,
            IDevicestatusBL deviceStatusBL, IDevicestatushistoryBL deviceStatusHistoryBL, IIOTHubService iotHubService, IGroupcommandBL groupCommandBL, IDevicecommandBL deviceCommandBL, IAlarmsandwarningsBL alarmsandWarningsBL, IDevicealarmsBL devicealarmsBL,
            ITransformations transformations, IDeviceAlarmsHistoryBL deviceAlarmsHistoryBL, IDevicegroupBL devicegroupBL, IDeviceBatchNumberBL deviceBatchNumber, IInverterBL inverterBL)
        {
            _deviceBL = deviceBL;
            _userBL = userBL;
            _groupsBL = groupsBL;
            _userroleBL = userroleBL;
            _roleBL = roleBL;
            _userdeviceBL = userdeviceBL;
            _customerBL = customerBL;
            _commandhistoryBL = commandhistoryBL;
            _deviceStatusBL = deviceStatusBL;
            _deviceStatusHistoryBL = deviceStatusHistoryBL;
            _iotHubService = iotHubService;
            _groupCommandBL = groupCommandBL;
            _deviceCommandBL = deviceCommandBL;
            _alarmsandWarningsBL = alarmsandWarningsBL;
            _devicealarmsBL = devicealarmsBL;
            _transformations = transformations;
            _deviceAlarmsHistoryBL = deviceAlarmsHistoryBL;
            _devicegroupBL = devicegroupBL;
            _deviceBatchNumberBL = deviceBatchNumber;
            _inverterBL = inverterBL;
        }

        public bool SaveUserDevices(UserDeviceDTO userDevices, string customerUniqueId, string adminUserId)
        {
            var customerId = Validator.ValidateCustomer(customerUniqueId).Id;
            Validator.ValidateAdmin(customerUniqueId, adminUserId);
            Validator.ValidatePassword(userDevices.password, adminUserId);

            var deviceUniqueIdList = new List<string>();

            if (userDevices.deviceGroupList.Count > 0)
            {
                foreach (var group in userDevices.deviceGroupList)
                {
                    deviceUniqueIdList.AddRange(group.deviceList.Where(x => x.hasPermission == true).Select(s => s.deviceId).Distinct().ToList());
                }

                var nonExistingDevicesList = NonExistingDevices(deviceUniqueIdList, customerUniqueId);

                if (nonExistingDevicesList.Count == 0)
                {
                    var _operator = _userBL.GetOperatorByUniqueId(userDevices.userId);

                    if (_operator != null) //update case
                    {
                        throw new ResponseException(new ResponseDTO
                        {
                            StatusCode = "Warning",
                            Message = $"User [{userDevices.userId}] already exists",
                            Data = null
                        });
                    }

                    var user = new User
                    {
                        Username = userDevices.userName,
                        CustomerId = customerId,
                        Password = userDevices.password,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = adminUserId,
                        ModifiedDate = DateTime.UtcNow,
                        UserId = userDevices.userId,
                        EmailAddress = userDevices.userId,
                        IsActive = 1,
                        IsDeleted = 0
                    };

                    var addedUser = _userBL.InsertUser(user);

                    AddRoleToOperator(addedUser);

                    AddDevicesToOperator(addedUser.Id, deviceUniqueIdList);

                    return true;
                }
                else
                {
                    throw new ResponseException(new ResponseDTO
                    {
                        StatusCode = "Warning",
                        Message = $"These fans do not exist: [{string.Join(",", nonExistingDevicesList)}]",
                        Data = null
                    });
                }
            }
            else
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"deviceGroupList is empty",
                    Data = null
                });
            }

        }

        public bool UpdateUserDevices(UserDeviceDTO userDevices, string customerUniqueId, string adminUserId)
        {
            var result = false;
            var customerId = Validator.ValidateCustomer(customerUniqueId).Id;
            Validator.ValidateAdmin(customerUniqueId, adminUserId);

            var deviceUniqueIdList = new List<string>();

            if (userDevices.deviceGroupList.Count > 0)
            {
                foreach (var group in userDevices.deviceGroupList)
                {
                    deviceUniqueIdList.AddRange(group.deviceList.Where(x => x.hasPermission == true).Select(s => s.deviceId).Distinct().ToList());
                }

                var nonExistingDevicesList = NonExistingDevices(deviceUniqueIdList, customerUniqueId);

                if (nonExistingDevicesList.Count == 0)
                {
                    //var _operator = _userBL.GetOperatorByUniqueId(userDevices.userId);
                    var _operator = Validator.ValidateOperator(customerUniqueId, userDevices.userId);

                    if (_operator != null) //update case
                    {
                        _operator.Username = userDevices.userName;
                        _operator.ModifiedBy = adminUserId;
                        _operator.ModifiedDate = DateTime.UtcNow;

                        var updatedUser = _userBL.UpdateUser(_operator);

                        DeleteUserDevicesByUserId(updatedUser.Id);

                        AddDevicesToOperator(updatedUser.Id, deviceUniqueIdList);

                        result = true;
                    }
                }
                else
                {
                    throw new ResponseException(new ResponseDTO
                    {
                        StatusCode = "Warning",
                        Message = $"These fans do not exist: [{string.Join(",", nonExistingDevicesList)}]",
                        Data = null
                    });
                }
            }
            else
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"deviceGroupList is empty",
                    Data = null
                });
            }
            return result;
        }

        public void AddRoleToOperator(User user)
        {
            Userrole userrole = new Userrole
            {
                UserId = user.Id,
                RoleId = _roleBL.QueryRole().Where(x => x.Role1 == "Operator").Select(s => s.Id).FirstOrDefault()
            };
            _userroleBL.InsertUserrole(userrole);
        }

        private List<string> NonExistingDevices(List<string> deviceUniqueIdList, string customerId)
        {
            var nonExistingDevices = new List<string>();

            foreach (var deviceUniqueId in deviceUniqueIdList)
            {
                var existingDevice = _deviceBL.GetExistingDeviceByCustomerId(customerId);

                if (existingDevice == null)
                {
                    nonExistingDevices.Add(deviceUniqueId);
                }
            }

            return nonExistingDevices;
        }

        public IEnumerable<DeviceGroupByOperatorDTO> GetTotalDevicesByOperators(string customerId, string AdminUserId)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(AdminUserId))
            {
                return null;
            }
            Validator.ValidateCustomer(customerId);
            Validator.ValidateAdmin(customerId, AdminUserId);
            var res = _deviceBL.GetTotalDevicesByOperators(customerId, AdminUserId);
            return res;
        }

        public void AddDevicesToOperator(int userId, List<string> deviceUniqueIdList)
        {
            foreach (var uniqueId in deviceUniqueIdList)
            {
                Userdevice userDevice = new Userdevice();
                userDevice.UserId = userId;
                userDevice.DeviceId = _deviceBL.GetDeviceDBId(uniqueId);
                _userdeviceBL.InsertUserDevice(userDevice);
            }
        }

        public void DeleteUserDevicesByUserId(int userId)
        {
            var userdeviceList = _userdeviceBL.QueryDevice().Where(x => x.UserId == userId).ToList();

            if (userdeviceList.Count > 0)
            {
                foreach (var userdevice in userdeviceList)
                {
                    _userdeviceBL.DeleteUserDevice(userdevice.Id);
                }
            }
        }

        public bool DeleteDevice(string CustomerId, string Device_id)
        {
            var customer = _customerBL.QueryCustomer().Where(x => x.CustomerId == CustomerId).FirstOrDefault();
            var device = _deviceBL.GetDevice().Where(x => x.DeviceId == Device_id && x.CustomerId == customer.Id).FirstOrDefault();
            if (device == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "Device Does not exist",
                    Data = null
                });
            }
            try
            {
                return _deviceBL.DeleteDevice(Device_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DeviceDTO> GetDeviceStats(string customerId, string userId)
        {
            var userUniqueId = 0;
            var customerUniqueId = 0;
            customerUniqueId = Validator.ValidateCustomer(customerId).Id;
            if (!string.IsNullOrEmpty(userId))
                userUniqueId = Validator.ValidateUser(customerId, userId).Id;

            IEnumerable<Device> devices;
            var result = new List<DeviceDTO>();
            if (string.IsNullOrEmpty(customerId))
            {
                devices = _deviceBL.GetDevice();//.OrderByDescending(x => x.CreatedDate).ToList();
            }
            else
            {
                if (string.IsNullOrEmpty(userId))
                {
                    devices = _deviceBL.GetDevicesByCustomerId(customerUniqueId);//.OrderByDescending(x => x.CreatedDate).ToList();
                }
                else
                {
                    devices = _deviceBL.GetDevicesByCustomerIdUserId(customerUniqueId, userUniqueId);//.OrderByDescending(x => x.CreatedDate).ToList();
                }
            }



            foreach (var d in devices)
            {
                DeviceDTO device = new DeviceDTO();
                device.customerId = customerId;
                device.deviceName = d.Name;
                device.deviceId = d.DeviceId;
                device.hasPermission = true;
                device.IsConfigured = d.IsInstalled == 0 ? false : true;
                //device.apSsid = d.Apssid;
                //device.apPassword = d.Appassword;
                device.currentFirmwareVersion = d.CurrentFirmwareVersion;
                device.latestFirmwareVersion = d.LatestFirmwareVersion;
                device.CreatedDate = d.CreatedDate;
                device.ModifiedDate = d.ModifiedDate;

                var deviceStatus = new DevicestatusDTO();
                var status = _deviceStatusBL.QueryDevicestatus().Where(x => x.DeviceId == d.Id).FirstOrDefault();
                status = TransformDeviceStatus(status);
                DateTime modifiedDate = (DateTime)status.ModifiedDate;
                device.connectivityStatus = Utility.GetStatusMessage(modifiedDate);
                //var timeResult = DateTime.UtcNow.Subtract(modifiedDate);
                //var minutes = timeResult.TotalMinutes;
                //if (minutes > 10)
                //    device.connectivityStatus = "Offline";
                //else if (minutes < 10 && minutes > 5)
                //    device.connectivityStatus = "Idle";
                //else
                //    device.connectivityStatus = "Online";

                deviceStatus.connectivityStatus = device.connectivityStatus;
                if (status != null)
                {
                    deviceStatus.DeviceId = device.deviceId;
                    deviceStatus.alarm = status.Alarm;
                    deviceStatus.warning = status.Warnings;

                    if (device.IsConfigured)
                    {
                        deviceStatus.AutoEndTime = status.AutoEndTime;
                        deviceStatus.AutoStartTime = status.AutoStartTime;
                        deviceStatus.AutoTemp = status.AutoTemp == 1 ? true : false;
                        deviceStatus.AutoTimer = status.AutoTimer == 1 ? true : false;
                        deviceStatus.CommandType = status.CommandType;
                        deviceStatus.HasPreviousSetting = status.HasPreviousSetting == 1 ? true : false;


                        deviceStatus.Temp = status.Temp;
                        deviceStatus.Humidity = status.Humidity;
                        deviceStatus.Pressure = status.Pressure;
                        deviceStatus.Iaq = status.Iaq;
                        deviceStatus.IaqAccuracy = status.IaqAccuracy;
                        deviceStatus.StaticIaq = status.StaticIaq;
                        deviceStatus.StaticIaqAccuracy = status.StaticIaqAccuracy;
                        deviceStatus.Co2Concentration = status.Co2Concentration;
                        deviceStatus.Co2ConcentrationAccuracy = status.Co2ConcentrationAccuracy;
                        deviceStatus.VocConcentration = status.VocConcentration;
                        deviceStatus.VocConcentrationAccuracy = status.VocConcentrationAccuracy;
                        deviceStatus.GasPercentage = status.GasPercentage;

                        deviceStatus.IdealTemp = status.IdealTemp;
                        deviceStatus.IsExecuted = status.IsExecuted == 1 ? true : false;
                        deviceStatus.MaintenanceHours = status.MaintenanceHours;
                        deviceStatus.MaxTemp = status.MaxTemp;
                        deviceStatus.MinTemp = status.MinTemp;
                        deviceStatus.OverrideSettings = status.OverrideSettings == 1 ? true : false;
                        deviceStatus.PowerStatus = status.PowerStatus == 1 ? true : false;
                        deviceStatus.Speed = status.Speed;
                        deviceStatus.TimeZone = status.TimeZone;
                        deviceStatus.UsageHours = Convert.ToInt32(status.RunningTime) / 3600;
                        deviceStatus.ModifiedDate = status.ModifiedDate;
                    }
                    device.deviceStatus = deviceStatus;
                }
                result.Add(device);
            }
            return result;
        }

        public IEnumerable<Device> GetAllDevices(string customerId)
        {
            IEnumerable<Device> devices;
            if (string.IsNullOrEmpty(customerId))
            {
                devices = _deviceBL.GetDeviceAlongWithStatus().OrderByDescending(x => x.CreatedDate);
            }
            else
            {
                int customerUniqueId = Validator.ValidateCustomer(customerId).Id;
                devices = _deviceBL.GetDeviceAlongWithStatus(customerUniqueId).OrderByDescending(x => x.CreatedDate);
            }
            return TransformDevicesStatus(devices);
        }

        public IEnumerable<Device> GetAllDevices(int Customer_id)
        {
            try
            {
                return _deviceBL.GetDevice().OrderByDescending(x => x.CreatedDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<DeviceGroupDTO> GetAllDevices(string customerID, string userId)
        {
            int customerUniqueId = Validator.ValidateCustomer(customerID).Id;
            Validator.ValidateUser(customerID, userId);

            UserDeviceDTO userDeviceDTO = new UserDeviceDTO();
            var devicegroup = _deviceBL.GetDeviceGroup(customerUniqueId);
            AdminTranformGroupDeviceData(devicegroup, userDeviceDTO, true, true, true);

            return userDeviceDTO.deviceGroupList;
        }

        public IEnumerable<UserDeviceDTO> GetDevices(string customerID, string userId, string operatorId = null, bool getAllDevices = false, bool UnGrouped = false, string groupId = null)
        {
            int actualCustomerId = Validator.ValidateCustomer(customerID).Id;
            var user = Validator.ValidateUser(customerID, userId);


            List<UserDeviceDTO> userDeviceDTOList = new List<UserDeviceDTO>();
            UserDeviceDTO userDeviceDTO = new UserDeviceDTO();

            userDeviceDTO.userId = user.UserId;
            userDeviceDTO.userName = user.Username;
            var devicegroup = _deviceBL.GetDeviceGroup(actualCustomerId);
            if (string.IsNullOrEmpty(groupId))
            {
                if (UnGrouped)
                {
                    AdminTranformGroupDeviceData(devicegroup, userDeviceDTO, true, true, false);
                }
                else
                {
                    AdminTranformGroupDeviceData(devicegroup, userDeviceDTO, true, true, true, true);
                }
            }
            else
            {
                Validator.ValidateGroup(customerID, groupId);
                AdminTranformGroupDeviceData(devicegroup, userDeviceDTO, true, true, true, true);
                userDeviceDTO.deviceGroupList = userDeviceDTO.deviceGroupList.Where(x => x.groupId == groupId || x.groupId == "-1" || x.groupId == "0").ToList();
            }

            userDeviceDTOList.Add(userDeviceDTO);
            return userDeviceDTOList;
        }

        public void TranformGroupData(IEnumerable<DeviceGroupBLDTO> data, DeviceGroupDTO group, bool devicestatus = false)
        {
            group.deviceList = new List<DeviceDTO>();
            foreach (var obj in data)
            {
                DeviceDTO device = new DeviceDTO();
                device.customerId = obj.customer_id.ToString();
                device.deviceName = obj.device_ap_name;
                device.deviceId = obj.device_id;
                device.hasPermission = true;
                device.IsConfigured = obj.IsInstalled == 0 ? false : true;
                device.apSsid = obj.Apssid;
                device.apPassword = obj.Appassword;
                //firmware information
                device.currentFirmwareVersion = obj.CurrentFirmwareVersion;
                device.latestFirmwareVersion = obj.LatestFirmwareVersion;
                //inverter Information
                device.inverterId = obj.InverterId;
                if (devicestatus) //if (devicestatus == true)
                {
                    var status = _deviceStatusBL.QueryDevicestatus().Where(x => x.DeviceId == obj.dId).FirstOrDefault();
                    status = TransformDeviceStatus(status);
                    DateTime modifiedDate = (DateTime)status.ModifiedDate;
                    device.connectivityStatus = Utility.GetStatusMessage(modifiedDate);
                    //var timeResult = DateTime.UtcNow.Subtract(date1);
                    //var minutes = timeResult.TotalMinutes;
                    //if (minutes > 10)
                    //    device.connectivityStatus = "Offline";
                    //else if (minutes < 10 && minutes > 5)
                    //    device.connectivityStatus = "Idle";
                    //else
                    //    device.connectivityStatus = "Online";

                    var deviceStatus = new DevicestatusDTO();
                    if (status != null)
                    {
                        deviceStatus.DeviceId = device.deviceId;

                        if (obj.InverterId == "Holip")
                        {
                            //Holip logic to convert alarm and warning code to description
                            deviceStatus.alarm = status.Alarm;
                            deviceStatus.warning = status.Warnings;
                            if (status.Alarm != "No Alarm" && status.Alarm != null)
                            {
                                var alarmCode = "E." + status.Alarm;
                                var alarmWarning = _alarmsandWarningsBL.QueryAlarmsandwarnings().Where(x => x.Code == alarmCode).FirstOrDefault();
                                if (alarmWarning != null) { deviceStatus.alarm = alarmWarning.Description; }

                            }
                            if (status.Warnings != "No Warning" && status.Warnings != null)
                            {
                                var warningCode = "A." + status.Warnings;
                                var alarmWarning = _alarmsandWarningsBL.QueryAlarmsandwarnings().Where(x => x.Code == warningCode).FirstOrDefault();
                                if (alarmWarning != null) { deviceStatus.warning = alarmWarning.Description; }

                            }
                        }
                        else if (obj.InverterId == "Schneider")
                        {
                            //Schneider logic to convert alarm and warning code to description 
                            if (status.Alarm != "No Alarm" && status.Alarm != null)
                            {
                                var alarmCode = Convert.ToInt32(status.Alarm);
                                var AlarmDBCode = Enum.ToObject(typeof(ScheniederAlarms), alarmCode).ToString();

                                var alarm = _alarmsandWarningsBL.QueryAlarmsandwarnings().Where(x => x.Code == AlarmDBCode).FirstOrDefault();

                                deviceStatus.alarm = alarm.Description;


                            }
                        }


                        if (device.IsConfigured)
                        {
                            deviceStatus.AutoEndTime = status.AutoEndTime;
                            deviceStatus.AutoStartTime = status.AutoStartTime;
                            deviceStatus.AutoTemp = status.AutoTemp == 1 ? true : false;
                            deviceStatus.AutoTimer = status.AutoTimer == 1 ? true : false;
                            deviceStatus.CommandType = status.CommandType;
                            deviceStatus.HasPreviousSetting = status.HasPreviousSetting == 1 ? true : false;


                            deviceStatus.Temp = status.Temp;
                            deviceStatus.Humidity = status.Humidity;
                            deviceStatus.Pressure = status.Pressure;
                            deviceStatus.Iaq = status.Iaq;
                            deviceStatus.IaqAccuracy = status.IaqAccuracy;
                            deviceStatus.StaticIaq = status.StaticIaq;
                            deviceStatus.StaticIaqAccuracy = status.StaticIaqAccuracy;
                            deviceStatus.Co2Concentration = status.Co2Concentration;
                            deviceStatus.Co2ConcentrationAccuracy = status.Co2ConcentrationAccuracy;
                            deviceStatus.VocConcentration = status.VocConcentration;
                            deviceStatus.VocConcentrationAccuracy = status.VocConcentrationAccuracy;
                            deviceStatus.GasPercentage = status.GasPercentage;

                            deviceStatus.IdealTemp = status.IdealTemp;
                            deviceStatus.IsExecuted = status.IsExecuted == 1 ? true : false;
                            deviceStatus.MaintenanceHours = status.MaintenanceHours;
                            deviceStatus.MaxTemp = status.MaxTemp;
                            deviceStatus.MinTemp = status.MinTemp;
                            deviceStatus.OverrideSettings = status.OverrideSettings == 1 ? true : false;
                            deviceStatus.PowerStatus = status.PowerStatus == 1 ? true : false;
                            deviceStatus.Speed = status.Speed;
                            deviceStatus.TimeZone = status.TimeZone;
                            deviceStatus.UsageHours = Convert.ToInt32(status.RunningTime) / 3600;
                        }
                    }
                    device.deviceStatus = deviceStatus;
                }

                group.deviceList.Add(device);
            }
        }

        public IEnumerable<Device> TransformDevicesStatus(IEnumerable<Device> devices)
        {
            foreach (var device in devices)
            {
                if (Convert.ToBoolean(device.IsInstalled))
                {
                    if (device.Devicestatus != null)
                    {
                        foreach (var status in device.Devicestatus)
                        {
                            TransformDeviceStatus(status);
                        }
                    }
                }
            }
            return devices;
        }

        public Devicestatus TransformDeviceStatus(Devicestatus status)
        {
            status.Temp = Math.Round(Convert.ToDouble(status.Temp), 2, MidpointRounding.AwayFromZero);
            status.Humidity = Math.Round(Convert.ToDouble(status.Humidity), 2, MidpointRounding.AwayFromZero);
            status.Iaq = Math.Round(Convert.ToDouble(status.Iaq), 2, MidpointRounding.AwayFromZero);
            status.StaticIaq = Math.Round(Convert.ToDouble(status.StaticIaq), 2, MidpointRounding.AwayFromZero);
            status.Co2Concentration = Math.Round(Convert.ToDouble(status.Co2Concentration), 2, MidpointRounding.AwayFromZero);
            status.VocConcentration = Math.Round(Convert.ToDouble(status.VocConcentration), 2, MidpointRounding.AwayFromZero);
            status.GasPercentage = Math.Round(Convert.ToDouble(status.GasPercentage), 2, MidpointRounding.AwayFromZero);
            status.UsageHours = Convert.ToInt32(status.RunningTime) / 3600;
            return status;
        }

        public void AdminTranformGroupDeviceData(IEnumerable<DeviceGroupBLDTO> data, UserDeviceDTO userDeviceDTO, bool Ungrouped = true, bool isBoard = true, bool IsAllGroup = true, bool deviceStatus = false)
        {
            var GroupedData = data.Where(x => x.gId != null).ToList();
            var UngroupedData = data.Where(x => x.gId == null && x.IsInstalled == 1).ToList();
            var ofBoardData = data.Where(x => x.IsInstalled == 0 && x.gId == null).ToList();
            userDeviceDTO.deviceGroupList = new List<DeviceGroupDTO>();

            if (Ungrouped && UngroupedData.Count > 0) //if (Ungrouped == true && UngroupedData.Count > 0)
            {
                DeviceGroupDTO UnGroup = new DeviceGroupDTO();
                UnGroup.fullGroupAccess = true;
                UnGroup.groupName = "UnGrouped";
                UnGroup.groupId = "-1";
                TranformGroupData(UngroupedData, UnGroup, deviceStatus);
                userDeviceDTO.deviceGroupList.Add(UnGroup);

            }

            if (IsAllGroup) //if (IsAllGroup == true)
            {
                foreach (var obj in GroupedData)
                {
                    if (userDeviceDTO.deviceGroupList.Where(x => x.groupId == obj.group_id).Count() == 0)
                    {
                        DeviceGroupDTO Group = new DeviceGroupDTO();
                        Group.fullGroupAccess = true;
                        Group.groupName = obj.group_name;
                        Group.groupId = obj.group_id;
                        //
                        int groupDbId = _groupsBL.GetGroupDBId(obj.group_id);
                        var groupCommand = (from c in _groupCommandBL.QueryGroupcommand()
                                            join u in _commandhistoryBL.QueryCommandhistory() on c.CommandHistoryId equals u.Id
                                            where c.GroupId == groupDbId
                                            select u).FirstOrDefault();
                        if (groupCommand != null) { Group.groupPowerStatus = groupCommand.PowerStatus == 1 ? true : false; }

                        //

                        var temp = data.Where(x => x.gId == obj.gId);
                        TranformGroupData(temp, Group, deviceStatus);
                        userDeviceDTO.deviceGroupList.Add(Group);
                    }

                }
            }

            if (isBoard && ofBoardData.Count > 0) //if (isBoard == true && ofBoardData.Count > 0)
            {
                DeviceGroupDTO OffBoard = new DeviceGroupDTO();
                OffBoard.fullGroupAccess = true;
                OffBoard.groupName = "OffBoard";
                OffBoard.groupId = "0";
                TranformGroupData(ofBoardData, OffBoard, deviceStatus);
                userDeviceDTO.deviceGroupList.Add(OffBoard);
            }


        }

        public IEnumerable<UnGroupedDevicesDTO> GetUnGroupedDevices(int customer_id)
        {
            var unGroupedDevices = _deviceBL.GetUnGroupedDevices(customer_id);
            return unGroupedDevices;
        }

        public void GroupFullAccessPermission(List<DeviceGroupDTO> data)
        {
            foreach (var obj in data)
            {
                if (obj.deviceList.Where(x => x.hasPermission == false).Count() > 0)
                {
                    obj.fullGroupAccess = false;
                }
            }
        }

        public List<DeviceGroupDTO> RemoveUnauthorizedDevices(List<DeviceGroupDTO> data)
        {
            List<DeviceGroupDTO> newGroup = new List<DeviceGroupDTO>();
            foreach (var obj in data)
            {
                List<DeviceDTO> newData = new List<DeviceDTO>();
                foreach (var device in obj.deviceList)
                {
                    if (device.hasPermission) //if (device.hasPermission == true)
                    {
                        newData.Add(device);
                    }
                }
                obj.deviceList = newData;
                if (obj.deviceList.Count > 0)
                {
                    newGroup.Add(obj);
                }
            }
            return newGroup;

        }

        public IEnumerable<UserDeviceDTO> GetAllUserDevices(string customerID, string userId, string operatorId, bool alldevices = true)
        {
            Validator.ValidateCustomer(customerID);
            UserDeviceDTO userDeviceDTO = new UserDeviceDTO();
            var customer = _customerBL.GetCustomerByUniqueId(customerID);
            var opUser = _userBL.GetOperatorByCustomerId(customer.Id, operatorId);
            if (opUser == null)
                Validator.ValidateOperator(customerID, operatorId);

            var userDeviceMaping = _userdeviceBL.GetUserDeviceMappingsByUser(opUser.Id);
            var device = _deviceBL.GetDevicesByCustomerId(customer.Id);
            var userDevices = (from u in userDeviceMaping join d in device on u.DeviceId equals d.Id select d).ToList();


            var devicegroup = _deviceBL.GetDeviceGroup(customer.Id);
            userDeviceDTO.userId = opUser.UserId;
            userDeviceDTO.userName = opUser.Username;
            if (alldevices) //if (alldevices == true)
            {
                AdminTranformGroupDeviceData(devicegroup, userDeviceDTO, true, true, true);
            }
            else
            {
                AdminTranformGroupDeviceData(devicegroup, userDeviceDTO, true, true, true, true);

            }

            List<UserDeviceDTO> userDeviceDTOList = new List<UserDeviceDTO>();
            _transformations.TransformOperatorDevices(userDeviceDTO.deviceGroupList, userDevices);
            GroupFullAccessPermission(userDeviceDTO.deviceGroupList);
            if (!alldevices) //if (alldevices == false)
            {
                userDeviceDTO.deviceGroupList = RemoveUnauthorizedDevices(userDeviceDTO.deviceGroupList);
            }
            userDeviceDTOList.Add(userDeviceDTO);
            return userDeviceDTOList;
        }

        public bool SendDevicecommand(StatusDTO deviceStatus, string customerUniqueId, string userId, string deviceId)
        {

            var customerId = Validator.ValidateCustomer(customerUniqueId).Id;
            var user = Validator.ValidateUser(customerUniqueId, userId);
            var device = Validator.ValidateDevice(customerUniqueId, deviceId);



            if (deviceStatus.CommandType.ToLower() == "maintenance")
            {
                UpdateMaintenanceStatus(device.Id, deviceStatus.MaintenanceHours);

                return true;
            }

            if (deviceStatus.CommandType.ToLower() == "runninghour")
            {
                ResetRunningHours(customerUniqueId, userId, deviceId);

                return true;
            }


            var commandHistory = SetCommandhistory(deviceStatus, device.Id);

            var commandId = Guid.NewGuid().ToString();

            commandHistory.CommandId = commandId;
            commandHistory.CreatedDate = DateTime.UtcNow;
            commandHistory.ModifiedDate = DateTime.UtcNow;

            Devicestatus updatedDevicestatus = null;
            StatusDTO updatedStatusDTO = null;

            var insertedCommandhistory = _commandhistoryBL.InsertCommandhistory(commandHistory);
            if (insertedCommandhistory != null)
            {
                // Adding entries into Devicecommand Table 
                var deviceCommandToBeAdded = SetDevicecommand(device.Id, customerId, insertedCommandhistory.Id);
                if (!Constants.statusFromCloud) //if (Constants.statusFromCloud == false)
                {
                    updatedDevicestatus = _deviceStatusBL.UpdateDevicestatus(deviceStatus, deviceStatus.CommandType);
                }
                if (updatedDevicestatus != null)
                {
                    updatedStatusDTO = _transformations.TransformUpdatedDevicestatusToStatusDTO(updatedDevicestatus, deviceStatus);
                }
                _deviceCommandBL.AddOrUpdateDevicecommand(deviceCommandToBeAdded);
                Commandhistory commandHist = _commandhistoryBL.GetUnExceutedCommand(device.Id);
                if (commandHist != null)
                {
                    updatedStatusDTO.CommandId = commandHist.CommandId;
                    Object CloudMessage = _transformations.TransformToCloudMessage(updatedStatusDTO, customerUniqueId, device.DeviceId);
                    _iotHubService.SendCloudtoDeviceMsg(CloudMessage, device.DeviceId);
                }
                return true;
            }
            return false;


        }
        public Devicecommand SetDevicecommand(int deviceId, int customerId, int commandHistoryId, byte IsGrouped = 0)
        {
            return new Devicecommand()
            {
                CommandHistoryId = commandHistoryId,
                DeviceId = deviceId,
                IsGrouped = IsGrouped,
                CustomerId = customerId,
                CreatedDate = DateTime.UtcNow

            };
        }

        public bool SendGroupcommand(StatusDTO deviceStatus, string customerUniqueId, string userId, string groupId)
        {
            var customerId = Validator.ValidateCustomer(customerUniqueId).Id;
            var user = Validator.ValidateUser(customerUniqueId, userId);
            var group = Validator.ValidateGroup(customerUniqueId, groupId);


            var deviceGroup = GetAllUserDevices(customerUniqueId, userId, userId, true).FirstOrDefault().deviceGroupList.Where(x => x.groupId == groupId).FirstOrDefault();
            if (deviceGroup == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Group Settings could not be applied.",
                    Data = null
                });
            }
            var UsergroupDevices = deviceGroup.deviceList.Where(x => x.hasPermission == true);

            if (UsergroupDevices == null || UsergroupDevices.Count() == 0)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Group Settings could not be applied",
                    Data = null
                });
            }

            var deviceUniqueIdList = UsergroupDevices.Select(s => s.deviceId).ToList();

            if (deviceStatus.CommandType.ToLower() == "group")
            {
                List<string> newDeviceList = new List<string>();

                foreach (var obj in deviceUniqueIdList)
                {
                    var device = _deviceBL.GetDeviceByUniqueId(obj);
                    var deviceCommad = _deviceCommandBL.GetGroupedDevicecommand(device.Id);
                    if (deviceCommad != null)
                    {
                        newDeviceList.Add(obj);
                    }
                }

                deviceUniqueIdList = newDeviceList;
            }
            else if (deviceStatus.CommandType.ToLower() == "groupoverride")
            {

            }
            else if (deviceStatus.CommandType.ToLower() == "runninghour")
            {
                ResetGroupRunningHours(customerUniqueId, userId, groupId);
                return true;
            }
            else if (deviceStatus.CommandType.ToLower() == "grouppowerstatus")
            {
            }
            else
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "Invalid Command Type",
                    Data = null
                });
            }

            var groupDbIdd = _groupsBL.GetGroupDBId(groupId);
            int Groupcommand = -1;

            if (deviceStatus.resetRunningHour && deviceStatus.CommandType != "groupPowerStatus") //if (deviceStatus.resetRunningHour == true && deviceStatus.CommandType != "groupPowerStatus")
            {
                //ResetGroupRunningHours(customerUniqueId, userId,groupId);
                foreach (var deviceId in deviceUniqueIdList)
                {
                    ResetRunningHoursOfDevice(deviceId);
                }
            }
            var CommandType = deviceStatus.CommandType.ToLower();
            foreach (var deviceUniqueId in deviceUniqueIdList)
            {
                var deviceDbId = _deviceBL.GetDeviceDBId(deviceUniqueId);
                var curDevicestatus = _deviceStatusBL.GetDevicestatusByDeviceId(deviceDbId);
                if (curDevicestatus != null)
                {
                    if (string.IsNullOrEmpty(groupId) || deviceDbId == 0)
                    {
                        throw new ResponseException(new ResponseDTO
                        {
                            StatusCode = "Warning",
                            Message = $"Invalid device Id: [{deviceDbId}]",
                            Data = null
                        });
                    }

                    // Updating Device Status
                    deviceStatus.DeviceId = deviceUniqueId;
                    if (!Constants.statusFromCloud) //if (Constants.statusFromCloud == false)
                    {
                        if (CommandType == "grouppowerstatus")
                        {
                            if (curDevicestatus != null && curDevicestatus.AutoTimer == 0)
                            {
                                _deviceStatusBL.UpdateDevicestatus(deviceStatus, "grouppowerstatus");
                            }

                        }
                        else { _deviceStatusBL.UpdateDevicestatus(deviceStatus); }


                    }

                    if (CommandType == "grouppowerstatus")
                    {
                        //deviceStatus.CommandType = "manual";
                        if (curDevicestatus != null && curDevicestatus.AutoTimer == 0)
                        {
                            Groupcommand = ApplyCommand(deviceStatus, deviceDbId, deviceUniqueId, groupId, customerUniqueId, customerId, "manual");
                        }

                    }
                    else
                    {

                        UpdateMaintenanceStatus(deviceDbId, deviceStatus.MaintenanceHours);
                        //deviceStatus.CommandType = "auto";
                        Groupcommand = ApplyCommand(deviceStatus, deviceDbId, deviceUniqueId, groupId, customerUniqueId, customerId, "auto");

                    }
                }
            }
            if (deviceGroup.deviceList.Count() == UsergroupDevices.Count())
            {
                if (Groupcommand != -1)
                {
                    Groupcommand groupCommand = new Groupcommand();
                    groupCommand.CommandHistoryId = Groupcommand;
                    groupCommand.CustomerId = customerId;
                    groupCommand.GroupId = groupDbIdd;
                    groupCommand.ModifiedDate = DateTime.UtcNow;
                    _groupCommandBL.AddOrUpdateGroupcommand(groupCommand);
                }
            }
            else
            {
                var msg = "";
                if (deviceStatus.CommandType.ToLower() == "groupoverride")
                {
                    msg = "Device level settings applied successfully. You do not have full-access to apply group level settings.";
                }
                else
                {
                    msg = "You do not have full-access to apply group level settings.";
                }
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = msg,
                    Data = null
                });
            }
            return true;
        }
        void ResetRunningHoursOfDevice(string deviceId)
        {
            var device = _deviceBL.GetDeviceByUniqueId(deviceId);
            var deviceStatus = _deviceStatusBL.QueryDevicestatus().Where(x => x.DeviceId == device.Id).FirstOrDefault();

            if (deviceStatus != null)
            {
                deviceStatus.RunningTime = 0;
                if (_deviceStatusBL.UpdateDevicestatus(deviceStatus) != null)
                {
                    device.LastMaintenanceDate = DateTime.UtcNow;
                    _deviceBL.UpdateDevice(device);
                }
            }
        }
        public int ApplyCommand(StatusDTO deviceStatus, int deviceDbId, string deviceUniqueId, string groupId, string customerUniqueId, int customerId, string Command)
        {
            var groupDbId = _groupsBL.GetGroupDBId(groupId);
            Commandhistory commandHistory = null;
            if (Command.ToLower().Equals("manual"))
            {
                var existingCommandHistory = _commandhistoryBL.QueryCommandhistory().Where(x => x.GroupId == groupDbId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                commandHistory = _transformations.TransformExistingCommandHistory(deviceStatus.PowerStatus.Value, existingCommandHistory, deviceDbId);
            }
            else
            {
                commandHistory = SetCommandhistory(deviceStatus, deviceDbId);
            }


            commandHistory.CommandType = Command;

            var commandId = Guid.NewGuid().ToString();



            commandHistory.CommandId = commandId;
            commandHistory.CreatedDate = DateTime.UtcNow;
            commandHistory.ModifiedDate = DateTime.UtcNow;
            commandHistory.GroupId = groupDbId;

            var commandResult = _commandhistoryBL.InsertCommandhistory(commandHistory);
            if (commandResult != null)
            {
                var deviceCommandToBeAdded = SetDevicecommand(deviceDbId, customerId, commandResult.Id, 1);
                _deviceCommandBL.AddOrUpdateDevicecommand(deviceCommandToBeAdded);
                Commandhistory commandHist = _commandhistoryBL.GetUnExceutedCommand(deviceDbId);
                commandHist.Group = null;
                if (commandHist != null)
                {
                    deviceStatus.CommandType = Command;
                    deviceStatus.CommandId = commandHist.CommandId;
                    Object CloudMessage = _transformations.TransformToCloudMessage(deviceStatus, customerUniqueId, deviceUniqueId);

                    _iotHubService.SendCloudtoDeviceMsg(CloudMessage, deviceUniqueId);
                }
            }
            return commandResult.Id;
        }

        public bool SendAcknowledgement(string commandId)
        {
            if (!string.IsNullOrEmpty(commandId))
            {
                var commandHistory = _commandhistoryBL.QueryCommandhistory().Where(x => x.CommandId == commandId).FirstOrDefault();
                if (commandHistory != null)
                {
                    commandHistory.IsExecuted = 1;
                    commandHistory.ModifiedDate = DateTime.UtcNow;
                    if (_commandhistoryBL.UpdateCommandhistory(commandHistory) != null)
                    {
                        return true;
                    }
                }
                else
                {
                    throw new ResponseException(new ResponseDTO
                    {
                        StatusCode = "Warning",
                        Message = $"No Command history found against commandId: [{commandId}]",
                        Data = null
                    });
                }
            }
            else
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Command Id is empty",
                    Data = null
                });
            }

            return false;
        }



        private Commandhistory SetCommandhistory(StatusDTO deviceStatus, int deviceDbId)
        {
            return new Commandhistory()
            {
                DeviceId = deviceDbId,
                PowerStatus = Convert.ToByte(deviceStatus.PowerStatus),
                Temp = deviceStatus.Temp,
                Speed = deviceStatus.Speed,
                Humidity = deviceStatus.Humidity,
                AutoTemp = Convert.ToByte(deviceStatus.AutoTemp),
                AutoTimer = Convert.ToByte(deviceStatus.AutoTimer),
                AutoStartTime = deviceStatus.AutoStartTime,
                AutoEndTime = deviceStatus.AutoEndTime,
                HasPreviousSetting = Convert.ToByte(deviceStatus.HasPreviousSetting),
                IdealTemp = deviceStatus.IdealTemp,
                MaintenanceHours = deviceStatus.MaintenanceHours,
                MaxTemp = deviceStatus.MaxTemp,
                MinTemp = deviceStatus.MinTemp,
                OverrideSettings = Convert.ToByte(deviceStatus.OverrideSettings),
                TimeZone = deviceStatus.TimeZone,
                UsageHours = deviceStatus.UsageHours,
                IsExecuted = 0,
                CommandType = deviceStatus.CommandType,
            };
        }

        private void SetDevicestatus(Commandhistory commandHistory, ref Devicestatus deviceStatus)
        {
            deviceStatus.PowerStatus = Convert.ToByte(commandHistory.PowerStatus);
            deviceStatus.Temp = commandHistory.Temp;
            deviceStatus.Speed = commandHistory.Speed;
            deviceStatus.Humidity = commandHistory.Humidity;
            deviceStatus.AutoTemp = Convert.ToByte(commandHistory.AutoTemp);
            deviceStatus.AutoTimer = Convert.ToByte(commandHistory.AutoTimer);
            deviceStatus.AutoStartTime = commandHistory.AutoStartTime;
            deviceStatus.AutoEndTime = commandHistory.AutoEndTime;
            deviceStatus.HasPreviousSetting = Convert.ToByte(commandHistory.HasPreviousSetting);
            deviceStatus.IdealTemp = commandHistory.IdealTemp;
            deviceStatus.MaintenanceHours = commandHistory.MaintenanceHours;
            deviceStatus.MaxTemp = commandHistory.MaxTemp;
            deviceStatus.MinTemp = commandHistory.MinTemp;
            deviceStatus.OverrideSettings = commandHistory.OverrideSettings;
            deviceStatus.TimeZone = commandHistory.TimeZone;
            deviceStatus.UsageHours = commandHistory.UsageHours;
            deviceStatus.IsExecuted = 0;
            deviceStatus.CommandType = commandHistory.CommandType;

        }

        public Devicestatus AddOrUpdateDevicestatus(SmartDeviceContainer smartDevice)
        {
            if (smartDevice != null)
            {
                return _deviceStatusBL.UpdateDevicestatus(smartDevice);
            }
            throw new Exception();
        }

        public GroupcommandDTO GetGroupcommand(string customerUniqueId, string userId, string groupId)
        {



            var customerId = Validator.ValidateCustomer(customerUniqueId).Id;
            var user = Validator.ValidateUser(customerUniqueId, userId);
            var group = Validator.ValidateGroup(customerUniqueId, groupId);


            var groupCommand = (from c in _groupCommandBL.QueryGroupcommand()
                                join u in _commandhistoryBL.QueryCommandhistory() on c.CommandHistoryId equals u.Id
                                where c.GroupId == @group.Id && c.CustomerId == customerId
                                select u).FirstOrDefault();
            if (groupCommand != null)
            {
                var groupCommandDTO = SetGroupcommandDto(groupCommand);
                return groupCommandDTO;
            }
            else
            {
                var groupSetting = _defaultSettingsBL.QueryDefaultsettings().FirstOrDefault();
                var commandHistory = _transformations.TransformDefaultSettingToCommandHistory(groupSetting);
                var groupCommandDTO = SetGroupcommandDto(groupCommand);
                return groupCommandDTO;
            }


        }

        public GroupcommandDTO SetGroupcommandDto(Commandhistory commandHistory)
        {
            return new GroupcommandDTO()
            {
                PowerStatus = commandHistory.PowerStatus == 1 ? true : false,
                Temp = commandHistory.Temp,
                Speed = commandHistory.Speed,
                Humidity = commandHistory.Humidity,
                AutoTemp = commandHistory.AutoTemp == 1 ? true : false,
                AutoTimer = commandHistory.AutoTimer == 1 ? true : false,
                AutoStartTime = commandHistory.AutoStartTime,
                AutoEndTime = commandHistory.AutoEndTime,
                HasPreviousSetting = commandHistory.HasPreviousSetting == 1 ? true : false,
                IdealTemp = commandHistory.IdealTemp,
                MaintenanceHours = commandHistory.MaintenanceHours,
                MaxTemp = commandHistory.MaxTemp,
                MinTemp = commandHistory.MinTemp,
                OverrideSettings = commandHistory.OverrideSettings == 1 ? true : false,
                TimeZone = commandHistory.TimeZone,
                UsageHours = commandHistory.UsageHours,
                IsExecuted = commandHistory.IsExecuted == 1 ? true : false,
                CommandType = commandHistory.CommandType,
                CreatedDate = commandHistory.CreatedDate,
                ModifiedDate = commandHistory.ModifiedDate,
            };
        }
        public DevicecommandDTO SetDevicecommandDto(Commandhistory commandHistory)
        {
            return new DevicecommandDTO()
            {
                PowerStatus = commandHistory.PowerStatus == 1 ? true : false,
                Temp = commandHistory.Temp,
                Speed = commandHistory.Speed,
                Humidity = commandHistory.Humidity,
                AutoTemp = commandHistory.AutoTemp == 1 ? true : false,
                AutoTimer = commandHistory.AutoTimer == 1 ? true : false,
                AutoStartTime = commandHistory.AutoStartTime,
                AutoEndTime = commandHistory.AutoEndTime,
                HasPreviousSetting = commandHistory.HasPreviousSetting == 1 ? true : false,
                IdealTemp = commandHistory.IdealTemp,
                MaintenanceHours = commandHistory.MaintenanceHours,
                MaxTemp = commandHistory.MaxTemp,
                MinTemp = commandHistory.MinTemp,
                OverrideSettings = commandHistory.OverrideSettings == 1 ? true : false,
                TimeZone = commandHistory.TimeZone,
                UsageHours = commandHistory.UsageHours,
                IsExecuted = commandHistory.IsExecuted == 1 ? true : false,
                CommandType = commandHistory.CommandType,
                CreatedDate = commandHistory.CreatedDate,
                ModifiedDate = commandHistory.ModifiedDate
            };
        }

        public DevicecommandDTO GetDevicecommand(string customerUniqueId, string userId, string deviceId)
        {


            //validations
            var customerId = Validator.ValidateCustomer(customerUniqueId).Id;
            var user = Validator.ValidateUser(customerUniqueId, userId);
            var device = Validator.ValidateDevice(customerUniqueId, deviceId);

            var deviceCommand = (from c in _deviceCommandBL.QueryDevicecommand()
                                 join u in _commandhistoryBL.QueryCommandhistory() on c.CommandHistoryId equals u.Id
                                 where c.DeviceId == @device.Id && c.CustomerId == customerId
                                 select u).FirstOrDefault();
            if (deviceCommand != null)
            {
                var deviceCommandDTO = SetDevicecommandDto(deviceCommand);
                return deviceCommandDTO;
            }
            else
            {
                var groupSetting = _defaultSettingsBL.QueryDefaultsettings().FirstOrDefault();
                var commandHistory = _transformations.TransformDefaultSettingToCommandHistory(groupSetting);
                var deviceCommandDTO = SetDevicecommandDto(commandHistory);
                return deviceCommandDTO;
            }


        }

        public void ResetRunningHours(string customerUniqueId, string userId, string deviceId)
        {
            var customerId = Validator.ValidateCustomer(customerUniqueId);
            var user = Validator.ValidateUser(customerUniqueId, userId);
            var device = Validator.ValidateDevice(customerUniqueId, deviceId);

            var deviceStatus = _deviceStatusBL.QueryDevicestatus().Where(x => x.DeviceId == device.Id).FirstOrDefault();

            if (deviceStatus != null)
            {
                deviceStatus.RunningTime = 0;
                if (_deviceStatusBL.UpdateDevicestatus(deviceStatus) != null)
                {
                    device.LastMaintenanceDate = DateTime.UtcNow;
                    _deviceBL.UpdateDevice(device);
                }
            }
            else
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"No device status found",
                    Data = null
                });
            }

        }

        public IEnumerable<UsageDTO> GetDeviceUsage(string customerUniqueId, string userId, string deviceId, string date, int days)
        {
            var customerId = Validator.ValidateCustomer(customerUniqueId);
            //var user = Validator.ValidateUser(customerUniqueId, userId);
            var device = Validator.ValidateDevice(customerUniqueId, deviceId);
            DateTime dateTime = DateTime.Parse(date);

            dateTime = dateTime.Date;
            var lastDaysDate = dateTime.AddDays(-days);

            var deviceStatusHistory = (from dh in _deviceStatusHistoryBL.QueryDevicestatushistory()
                                       where dh.DeviceId == device.Id && DateTime.Parse(dh.CreatedDate.ToString()).Date > lastDaysDate.Date && DateTime.Parse(dh.CreatedDate.ToString()).Date <= dateTime
                                       select new
                                       {
                                           dh.RunningTime,
                                           dh.CreatedDate.Value.Date

                                       }).ToList();

            var usageList = (from dh in deviceStatusHistory
                             group dh by dh.Date into usage
                             select new UsageDTO
                             {
                                 Date = usage.Key,
                                 RunningHours = usage.Sum(x => x.RunningTime) / 3600
                             }).ToList();

            return usageList;
        }

        public IEnumerable<UsageDTO> GetAllUsage(string customerUniqueId, string userId, string date, int days)
        {
            var customerId = Validator.ValidateCustomer(customerUniqueId);
            //var user = Validator.ValidateUser(customerUniqueId, userId);
            DateTime dateTime = DateTime.Parse(date);

            dateTime = dateTime.Date;
            var lastDaysDate = dateTime.AddDays(-days);

            var deviceStatusHistory = (from dh in _deviceStatusHistoryBL.QueryDevicestatushistory()
                                       join d in _deviceBL.QueryDevice() on dh.DeviceId equals d.Id
                                       where d.CustomerId == customerId.Id && DateTime.Parse(dh.CreatedDate.ToString()).Date > lastDaysDate.Date && DateTime.Parse(dh.CreatedDate.ToString()).Date <= dateTime
                                       select new
                                       {
                                           dh.DeviceId,
                                           dh.RunningTime,
                                           dh.CreatedDate.Value.Date

                                       }).ToList();

            var usageList = (from dh in deviceStatusHistory
                             group dh by dh.Date into usage
                             select new UsageDTO
                             {
                                 Date = usage.Key,
                                 RunningHours = usage.Sum(x => x.RunningTime) / 3600
                             }).ToList();

            return usageList;
        }


        public IEnumerable<UsageDTO> GetDeviceUsage(string customerUniqueId, string deviceId, string date)
        {
            //  var customerId = Validator.ValidateCustomer(customerUniqueId);

            var device = _deviceBL.QueryDevice().Where(x => x.DeviceId == deviceId).FirstOrDefault();
            DateTime dateTime = DateTime.Parse(date);

            var deviceStatusHistory = (from dh in _deviceStatusHistoryBL.QueryDevicestatushistory()
                                       where dh.DeviceId == device.Id && DateTime.Parse(dh.CreatedDate.ToString()).Date == dateTime
                                       select new
                                       {
                                           dh.RunningTime,
                                           dh.CreatedDate.Value.Date

                                       }).ToList();

            var usageList = (from dh in deviceStatusHistory
                             group dh by dh.Date into usage
                             select new UsageDTO
                             {
                                 Date = usage.Key,
                                 RunningHours = usage.Sum(x => x.RunningTime) / 3600
                             }).ToList();

            return usageList;
        }

        private void UpdateMaintenanceStatus(int deviceId, int maintenanceHours)
        {
            var storedDevicestatus = _deviceStatusBL.GetDevicestatusByDeviceId(deviceId);

            if (storedDevicestatus == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Device Status not initialized!",
                    Data = null
                });
            }

            storedDevicestatus.MaintenanceHours = maintenanceHours;

            _deviceStatusBL.UpdateDevicestatus(storedDevicestatus);
        }

        private int GetCustomer(string customerUniqueId)
        {
            var customerId = _customerBL.QueryCustomer().Where(x => x.CustomerId == customerUniqueId).Select(s => s.Id).FirstOrDefault();
            if (string.IsNullOrEmpty(customerUniqueId) || customerId == 0)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Invalid customer Id: [{customerUniqueId}]",
                    Data = null
                });
            }

            return customerId;
        }

        private User GetUser(string customerUniqueId, string userId)
        {
            var user = (from c in _customerBL.QueryCustomer()
                        join u in _userBL.QueryUsers() on c.Id equals u.CustomerId
                        where c.CustomerId == customerUniqueId && u.UserId == userId
                        select u).FirstOrDefault();
            if (user == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"User: [{userId}] does not belong to customer [{customerUniqueId}]",
                    Data = null
                });
            }
            return user;
        }

        private Groups GetGroup(string customerUniqueId, string groupId)
        {
            var group = (from c in _customerBL.QueryCustomer()
                         join u in _groupsBL.QueryGroups() on c.Id equals u.CustomerId
                         where c.CustomerId == customerUniqueId && u.GroupId == groupId
                         select u).FirstOrDefault();
            if (group == null)
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

        public Devicestatus GetDevicestatus(string customerUniqueId, string deviceId)
        {
            var customerId = Validator.ValidateCustomer(customerUniqueId);

            var device = Validator.ValidateDevice(customerUniqueId, deviceId);
            return _deviceStatusBL.GetDevicestatusByDeviceId(device.Id);

        }
        public Devicealarms GetDevicealarms(string customerUniqueId, string deviceId)
        {
            var customerId = Validator.ValidateCustomer(customerUniqueId);

            var device = Validator.ValidateDevice(customerUniqueId, deviceId);
            return _devicealarmsBL.GetDevicealarmsByDeviceId(deviceId, customerUniqueId);

        }

        public void SendDeviceState(string customerUniqueId, string deviceId)
        {
            var Status = GetDevicestatus(customerUniqueId, deviceId);

            var device = _deviceBL.QueryDevice().Where(x => x.DeviceId == deviceId).FirstOrDefault();


            var autoState = _transformations.TransformStatusToAuto(Status);
            var manualState = _transformations.TransformStatusToManual(Status);
            autoState.CustomerId = customerUniqueId;
            autoState.DeviceId = deviceId;

            manualState.CustomerId = customerUniqueId;
            manualState.DeviceId = deviceId;

            //Setting firmware Version 
            autoState.CurrentFirmwareVersion = device.CurrentFirmwareVersion;
            autoState.LatestFirmwareVersion = device.LatestFirmwareVersion;

            manualState.CurrentFirmwareVersion = device.CurrentFirmwareVersion;
            manualState.LatestFirmwareVersion = device.LatestFirmwareVersion;

            //InverterId
            if (device.InverterId != null)
            {
                autoState.InverterId = _inverterBL.GetInverter(Convert.ToInt32(device.InverterId)).InverterId;
                manualState.InverterId = _inverterBL.GetInverter(Convert.ToInt32(device.InverterId)).InverterId;
            }
            _iotHubService.SendCloudtoDeviceMsg(autoState, deviceId);

            _iotHubService.SendCloudtoDeviceMsg(manualState, deviceId);

        }


        public void ResetGroupRunningHours(string customerUniqueId, string userId, string groupId)
        {
            var deviceGroup = GetAllUserDevices(customerUniqueId, userId, userId, true).FirstOrDefault().deviceGroupList.Where(x => x.groupId == groupId).FirstOrDefault();
            var UsergroupDevices = deviceGroup.deviceList.Where(x => x.hasPermission == true);
            var deviceUniqueIdList = UsergroupDevices.Select(s => s.deviceId).ToList();
            foreach (var obj in deviceUniqueIdList)
            {
                var device = _deviceBL.QueryDevice().Where(x => x.DeviceId == obj).FirstOrDefault();

                var deviceStatus = _deviceStatusBL.QueryDevicestatus().Where(x => x.DeviceId == device.Id).FirstOrDefault();

                if (deviceStatus != null)
                {
                    deviceStatus.RunningTime = 0;
                    if (_deviceStatusBL.UpdateDevicestatus(deviceStatus) != null)
                    {

                        device.LastMaintenanceDate = DateTime.UtcNow;
                        _deviceBL.UpdateDevice(device);
                    }
                }
            }
        }

        public bool CustomizeDeviceName(string customerId, string userId, string deviceId, string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Invalid device name.",
                    Data = null
                });
            }

            var customerDbId = Validator.ValidateCustomer(customerId);
            var device = Validator.ValidateDevice(customerId, deviceId);
            Validator.ValidateUserDevice(userId, deviceId);
            Validator.ValidateDeviceName(deviceName, deviceId);
            device.Name = deviceName;
            if (_deviceBL.UpdateDevice(device) != null)
            {
                return true;
            }
            return false;
        }

        public bool CustomizeDeviceName(string customerId, string deviceId, string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Invalid device name.",
                    Data = null
                });
            }

            var customerDbId = Validator.ValidateCustomer(customerId);
            var device = Validator.ValidateDevice(customerId, deviceId);
            Validator.ValidateDeviceName(deviceName, deviceId);
            device.Name = deviceName;
            if (_deviceBL.UpdateDevice(device) != null)
            {
                return true;
            }
            return false;
        }

        public Device GetDeviceByUniqueId(string deviceId)
        {
            return _deviceBL.GetDeviceByUniqueId(deviceId);
        }
        public bool ChangeDeviceSpeed(string customerId, string deviceId, int speed)
        {
            Validator.ValidateCustomer(customerId);
            var device = Validator.ValidateDevice(customerId, deviceId);
            var existingDeviceStatus = _deviceStatusBL.GetDevicestatusByDeviceId(device.Id);
            if (existingDeviceStatus != null)
            {
                existingDeviceStatus.Speed = speed;
                existingDeviceStatus.ModifiedDate = DateTime.UtcNow;
                _deviceStatusBL.UpdateDevicestatus(existingDeviceStatus);
                return true;
            }

            return false;
        }
        public bool ChangeDevicePowerStatus(string customerId, string deviceId, short powerStatus)
        {
            Validator.ValidateCustomer(customerId);
            var device = Validator.ValidateDevice(customerId, deviceId);
            var existingDeviceStatus = _deviceStatusBL.GetDevicestatusByDeviceId(device.Id);
            if (existingDeviceStatus != null)
            {
                existingDeviceStatus.PowerStatus = powerStatus;
                existingDeviceStatus.ModifiedDate = DateTime.UtcNow;
                _deviceStatusBL.UpdateDevicestatus(existingDeviceStatus);
                return true;
            }

            return false;
        }

        public bool DeleteDeviceById(string customerId, string deviceId)
        {
            Validator.ValidateCustomer(customerId);
            var device = Validator.ValidateDevice(customerId, deviceId);
            _deviceAlarmsHistoryBL.DeleteDeviceAlarmshistoryByDeviceId(device.Id);
            _commandhistoryBL.DeleteCommandhistoryByDeviceId(device.Id);
            _deviceCommandBL.DeleteDevicecommandByDeviceId(device.Id);
            _deviceStatusHistoryBL.DeleteDeviceStatusHistoryByDeviceId(device.Id); //no FK relation
            _deviceStatusBL.DeleteDevicestatusByDeviceId(device.Id);
            _devicegroupBL.DeleteDeviceGroupByDeviceId(device.Id);
            _userdeviceBL.DeleteUserDeviceByDeviceId(device.Id);
            _devicealarmsBL.DeleteDeviceAlarmsByDeviceId(device.DeviceId); //no FK relation
            _deviceBL.DeleteDevice(device.Id);

            return true;
        }

        public IEnumerable<Devicebatchnumber> GetAllBatchNumber()
        {
            return _deviceBatchNumberBL.GetDeviceBatchNumber();
            throw new NotImplementedException();
        }

        public bool UpdateDeviceFirmware(string firmwareVersion, string batchId)
        {


            var deviceList = _deviceBL.QueryDevice().Where(x => x.BatchId == batchId).ToList();

            foreach (Device deviceObj in deviceList)
            {
                deviceObj.LatestFirmwareVersion = firmwareVersion;


            }


            _deviceBL.UpdateDevices(deviceList);
            return true;
        }

        public bool UpdateDeviceState(string firmwareVersion, string deviceId, string inverterId)
        {
            var device = _deviceBL.QueryDevice().Where(x => x.DeviceId == deviceId).FirstOrDefault();

            if (device.CurrentFirmwareVersion != firmwareVersion && firmwareVersion != null)
            {
                device.CurrentFirmwareVersion = firmwareVersion;


            }
            if (inverterId != null)
            {
                var inverter = _inverterBL.QueryInverter().Where(x => x.InverterId == inverterId).FirstOrDefault();
                device.InverterId = inverter.Id;
            }

            _deviceBL.UpdateDevice(device);

            return true;
        }
    }

}


