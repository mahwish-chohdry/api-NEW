using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.ServiceLayer.Transformations;
using Xavor.SD.ServiceLayer.Validations;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.ServiceLayer
{
    public class GroupsService : IGroupsService
    {
        private readonly IGroupsBL _groupBL;
        private readonly IDevicegroupBL _devicegroupBL;
        private readonly ICustomerBL _customerBL;
        private readonly IDeviceBL _deviceBL;
        private readonly IUserBL _userBL;
        private readonly IUserroleBL _userroleBL;
        private readonly IRoleBL _roleBL;
        private readonly ICommandhistoryBL _commandhistoryBL;
        private readonly IGroupcommandBL _groupCommandBL;
        private readonly IDefaultSettingsBL _defaultSettingsBL;
        private readonly ITransformations _transformation;

        public GroupsService(IGroupsBL groupBL, IDevicegroupBL devicegroupBL, ICustomerBL customerBL, IDeviceBL deviceBL, IUserBL userBL, IUserroleBL userroleBL, IRoleBL roleBL, ICommandhistoryBL commandhistoryBL, IGroupcommandBL groupCommandBL, IDefaultSettingsBL defaultSettingsBL, ITransformations transformation)
        {
            _groupBL = groupBL;
            _devicegroupBL = devicegroupBL;
            _customerBL = customerBL;
            _deviceBL = deviceBL;
            _userBL = userBL;
            _roleBL = roleBL;
            _commandhistoryBL = commandhistoryBL;
            _groupCommandBL = groupCommandBL;
            _userroleBL = userroleBL;
            _defaultSettingsBL = defaultSettingsBL;
            _transformation = transformation;

        }

        //create
        public string SaveGroup(UserDeviceDTO userDevices, string customerUniqueId, string adminUserId)
        {
            var customerId = Validator.ValidateCustomer(customerUniqueId).Id;
            Validator.ValidateAdmin(customerUniqueId, adminUserId);


            var deviceUniqueIdList = userDevices.deviceGroupList.FirstOrDefault().deviceList.Select(s => s.deviceId).Distinct().ToList();
            if (deviceUniqueIdList.Count == 0 || deviceUniqueIdList == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Please add devices to group.",
                    Data = null
                });
            }
            var nonExistingDevicesList = NonExistingDevices(deviceUniqueIdList, customerUniqueId);
            //var OffBoardDevicesList = OffBoardDevices(deviceUniqueIdList);


            if (nonExistingDevicesList.Count == 0)
            {
                var existingGroupAssociationsList = ExistingGroupAssociations(deviceUniqueIdList);

                if (existingGroupAssociationsList.Count == 0)
                {
                    var groupName = userDevices.deviceGroupList.FirstOrDefault().groupName;
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        var existingGroup = _groupBL.GetGroupByNameAndCustomer(groupName, customerId);

                        if (existingGroup != null)
                        {
                            throw new ResponseException(new ResponseDTO
                            {
                                StatusCode = "Warning",
                                Message = $"\"{groupName}\" group already exists. Please change the name of the group.",
                                Data = null
                            });
                        }

                        var groupId = Guid.NewGuid().ToString();
                        var groupSetting = _defaultSettingsBL.QueryDefaultsettings().FirstOrDefault();
                        var commandHistory = _transformation.TransformDefaultSettingToCommandHistory(groupSetting);



                        Groups group = new Groups
                        {
                            Name = groupName,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = adminUserId,
                            CustomerId = customerId,
                            GroupId = groupId,
                            IsActive = 1,
                            IsDeleted = 0,



                        };

                        var AddedGroup = _groupBL.InsertGroups(group);
                        commandHistory.GroupId = AddedGroup.Id;
                        var defaultCommand = _commandhistoryBL.InsertCommandhistory(commandHistory);
                        Groupcommand groupcommand = new Groupcommand
                        {
                            CommandHistoryId = defaultCommand.Id,
                            CustomerId = customerId,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow,
                            GroupId = AddedGroup.Id,


                        };
                        _groupCommandBL.InsertGroupcommand(groupcommand);
                        if (AddedGroup != null)
                        {
                            AddDevices(AddedGroup.Id, deviceUniqueIdList, adminUserId);
                            return groupId;
                        }
                    }
                    else
                    {
                        throw new ResponseException(new ResponseDTO
                        {
                            StatusCode = "Warning",
                            Message = $"Invalid group name: [{groupName}]",
                            Data = null
                        });
                    }
                }
                else
                {
                    throw new ResponseException(new ResponseDTO
                    {
                        StatusCode = "Warning",
                        Message = $"These fans already exist in other groups: [{string.Join(",", existingGroupAssociationsList)}]",
                        Data = null
                    });
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


            return null;
        }

        //update
        public bool SaveGroup(UserDeviceDTO userDevices, string customerUniqueId, string adminUserId, string groupId)
        {
            var customerId = Validator.ValidateCustomer(customerUniqueId).Id;
            Validator.ValidateAdmin(customerUniqueId, adminUserId);
            
            var deviceUniqueIdList = userDevices.deviceGroupList.FirstOrDefault().deviceList.Select(s => s.deviceId).Distinct().ToList();
            if (deviceUniqueIdList.Count == 0)
            {
                DeleteExistingGroup(groupId); //delete group incase no devices assigned
                return true;

            }
            var nonExistingDevicesList = NonExistingDevices(deviceUniqueIdList, customerUniqueId);



            if (nonExistingDevicesList.Count == 0)
            {
                var groupToBeUpdated = _groupBL.GetGroupByUniqueId(groupId);

                if (groupToBeUpdated != null) //update case
                {
                    Validator.ValidateGroupName(userDevices.deviceGroupList.FirstOrDefault().groupName, groupToBeUpdated.GroupId, customerId);

                    groupToBeUpdated.Name = userDevices.deviceGroupList.FirstOrDefault().groupName;
                    groupToBeUpdated.ModifiedBy = customerUniqueId;
                    groupToBeUpdated.ModifiedDate = DateTime.UtcNow;

                    var updatedGroup = _groupBL.UpdateGroups(groupToBeUpdated);
                    _devicegroupBL.DeleteDeviceGroupByGroupId(updatedGroup.Id);
                    AddDevices(updatedGroup.Id, deviceUniqueIdList, adminUserId);

                    return true;
                }
                else
                {
                    throw new ResponseException(new ResponseDTO
                    {
                        StatusCode = "No Content",
                        Message = $"\"{ userDevices.deviceGroupList[0].groupName }\" group does not exist anymore ",
                        Data = null
                    });
                }
            }
            else
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"These fans do not exist: {string.Join(",", nonExistingDevicesList)}",
                    Data = null
                });
            }


            //return false;
        }
        private void DeleteExistingGroup(string groupId)
        {
            var groupToBeDeleted = _groupBL.GetGroupByUniqueId(groupId);
            if (groupToBeDeleted == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "No Content",
                    Message = $"Group doesn't exist anymore",
                    Data = null
                });
            }
            else
            {
                _devicegroupBL.DeleteDeviceGroupByGroupId(groupToBeDeleted.Id);
                _groupCommandBL.DeleteGroupcommandByGroupId(groupToBeDeleted.Id);
                _commandhistoryBL.DeleteCommandhistoryByGroupId(groupToBeDeleted.Id);
                _groupBL.DeleteGroups(groupToBeDeleted.Id);
            }
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

        private List<string> OffBoardDevices(List<string> deviceUniqueIdList)
        {
            var offBoardDevices = new List<string>();

            foreach (var deviceUniqueId in deviceUniqueIdList)
            {
                var offBoardDevice = _deviceBL.GetOffBoardDeviceByDeviceId(deviceUniqueId);

                if (offBoardDevice != null)
                {
                    offBoardDevices.Add(deviceUniqueId);
                }
            }

            return offBoardDevices;
        }

        private List<string> ExistingGroupAssociations(List<string> deviceUniqueIdList)
        {
            var existingDeviceGroupAssocicationsList = new List<string>();

            foreach (var deviceUniqueId in deviceUniqueIdList)
            {
                var deviceId = _deviceBL.GetExistingGroupAssociationByDeviceId(deviceUniqueId);

                if (deviceId != null)
                {
                    existingDeviceGroupAssocicationsList.Add(deviceId);
                }
            }

            return existingDeviceGroupAssocicationsList;
        }

        public void AddDevices(int groupId, List<string> deviceUniqueIdList, string adminUserId)
        {

            foreach (var deviceUniqueId in deviceUniqueIdList)
            {
                Devicegroup devicegroup = new Devicegroup();
                devicegroup.DeviceId = _deviceBL.QueryDevice().Where(x => x.DeviceId == deviceUniqueId).Select(s => s.Id).FirstOrDefault();
                devicegroup.GroupId = groupId;
                devicegroup.CreatedDate = DateTime.UtcNow;
                devicegroup.CreatedBy = adminUserId;
                _devicegroupBL.InsertDevicegroup(devicegroup);
            }
        }
    }
}
