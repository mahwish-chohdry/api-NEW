using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.WebAPI.ViewContracts;

namespace Xavor.SD.ServiceLayer
{
    public interface IUserService
    {
        UserResponseDTO AddOperatorAssignFanGroup(OperatorGroupFan dto);
        bool UpdateUser(string customerId, string userId, EditProfileDTO user);
        List<string> GetEmailRecipients(EmailDTO emailDTO);
        bool DeleteUserById(string customerId, string userId);
        IEnumerable<Role> GetRoles();
        User UpdateUser(User user);
        User UpdateUserProfile(User user);
        User UpdateUserPassword(User user);
        IEnumerable<User> GetUser(string customerId);
        IEnumerable<Role> GetRoleByDescription(string description);
        Role GetRoleByNameAndDescription(string roleName, string description);
        IEnumerable<Form> GetForms();
        Form GetForm(int Id);
        Form InsertForm(Form form);
        Form UpdateForm(Form form);
        bool DeleteForm(int formId);
        bool DeleteForm(string formId);
        List<PersonasRolePermissionsDTO> GetRolepermissions();
        Rolepermission GetRolepermission(int Id);
        PersonasRolePermissionsDTO InsertRolepermission(Rolepermission rolepermission);
        Rolepermission UpdateRolepermission(Rolepermission rolepermission);
        bool DeleteRolepermission(int rolepermissionId);

        UserPermissionManagement GetUserPermission(string customerId,string userId);

    }
}
