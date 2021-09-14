using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.WebAPI.ViewContracts;
using System.Linq;
using Xavor.SD.Common.Utilities;
using Xavor.SD.ServiceLayer.Validations;
using Microsoft.EntityFrameworkCore;

namespace Xavor.SD.ServiceLayer
{
    public class UserService : IUserService
    {
        private readonly IUserBL _userBL;
        private readonly IUserroleBL _userroleBL;
        private readonly IUserdeviceBL _userdeviceBL;
        private readonly ICustomerBL _customerBL;
        private readonly ICustomerService _customerService;
        private readonly IEmailBL _emailBL;
        private readonly IConfigurationsBL _configurationsBL;
        private readonly IRoleBL _roleBL;
        private readonly IFormBL _formBL;
        private readonly IRolepermissionBL _rolepermissionBL;
        private readonly IRefreshtokensBL _refreshtokensBL;
        private readonly IPersonaBL _personaBL;
        private readonly IPersonapermissionBL _personaPermissionBL;

        public UserService(IUserBL userBL, IUserroleBL userroleBL, IUserdeviceBL userdeviceBL, ICustomerBL customerBL, ICustomerService customerService, IEmailBL emailBL, IConfigurationsBL configurationsBL, IRoleBL roleBL, IFormBL formBL, IRolepermissionBL rolepermissionBL, IRefreshtokensBL refreshtokensBL
            , IPersonaBL personaBL, IPersonapermissionBL personaPermissionBL)
        {
            _userBL = userBL;
            _userroleBL = userroleBL;
            _userdeviceBL = userdeviceBL;
            _customerBL = customerBL;
            _customerService = customerService;
            _emailBL = emailBL;
            _configurationsBL = configurationsBL;
            _roleBL = roleBL;
            _formBL = formBL;
            _rolepermissionBL = rolepermissionBL;
            _refreshtokensBL = refreshtokensBL;
            _personaBL = personaBL;
            _personaPermissionBL = personaPermissionBL;


        }


        public UserResponseDTO AddOperatorAssignFanGroup(OperatorGroupFan dto)
        {
            UserResponseDTO res = new UserResponseDTO();
            try
            {
                User user = new User();
                user.Username = dto.username;
                user.EmailAddress = dto.email;
                user.CustomerId = dto.customer_id;
                user.Password = dto.password;
                user.CreatedDate = DateTime.UtcNow;
                user.UserId = "test";
                user.IsActive = 1;
                user.IsDeleted = 0;
                var AddedUser = _userBL.InsertUser(user);
                var role = AddRoleToOperator(AddedUser);
                AddFanToOperator(AddedUser.Id, dto.device_ids);
                res.user_id = AddedUser.Id;
                res.success = true;
                return res;
            }
            catch (Exception ex)
            {
                res.user_id = 00;
                res.success = false;
                return res;
            }




        }

        public Userrole AddRoleToOperator(User user)
        {
            Userrole userrole = new Userrole();
            userrole.UserId = user.Id;
            userrole.RoleId = 3;
            return _userroleBL.InsertUserrole(userrole);
        }

        public bool AddFanToOperator(int UserId, List<int> FanList)
        {
            foreach (var obj in FanList)
            {
                Userdevice userDevice = new Userdevice();
                userDevice.UserId = UserId;
                userDevice.DeviceId = obj;
                _userdeviceBL.InsertUserDevice(userDevice);
            }
            return true;
        }

        public bool DeleteFanToOperator(int UserId, List<int> FanList)
        {

            Userdevice userDevice = new Userdevice();
            userDevice.UserId = UserId;
            _userdeviceBL.DeleteUserDevicesByUserId(UserId);
            return true;
        }

        public bool UpdateUser(string customerId, string userId, EditProfileDTO user)
        {
            var Customer = _customerBL.QueryCustomer().Where(x => x.CustomerId == customerId).FirstOrDefault();
            var UserResult = _userBL.GetUsers().Where(x => x.CustomerId == Customer.Id && x.UserId == userId).FirstOrDefault();

            if (Customer == null || UserResult == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "User  is not Valid",
                    Data = null
                });
            }

            UserResult.ProfilePicture = user.ProfileImage;
            if (user.UserName != null)
            {
                UserResult.Username = user.UserName;
            }

            _userBL.UpdateUser(UserResult);

            return true;
        }

        public List<string> GetEmailRecipients(EmailDTO emailDTO)
        {
            int actualCustomerId = Validator.ValidateCustomer(emailDTO.customerId).Id;
            Validator.ValidateUser(emailDTO.customerId, emailDTO.userId);

            var emailRecipients = _configurationsBL.GetSuperAdminEmails(actualCustomerId);

            Email newEmailEntry = SetNewEmailEntry(emailDTO, emailRecipients);
            _emailBL.InsertEmail(newEmailEntry);

            var emailRecipientsList = emailRecipients.Split(",").ToList();
            return emailRecipientsList;
        }


        private Email SetNewEmailEntry(EmailDTO emailDTO, string emailRecipients)
        {
            return new Email()
            {
                From = emailDTO.userId,
                To = emailRecipients,
                Subject = emailDTO.subject,
                Body = emailDTO.body,
                CreatedDate = DateTime.UtcNow
            };
        }

        public bool DeleteUserById(string customerId, string userId)
        {
            int customerDbId = Validator.ValidateCustomer(customerId).Id;
            var user = Validator.ValidateUser(customerId, userId);
            var role = (from u in _userBL.QueryUsers()
                        join c in _customerBL.QueryCustomer() on u.CustomerId equals c.Id
                        join ur in _userroleBL.QueryUserrole() on u.Id equals ur.UserId
                        join r in _roleBL.QueryRole() on ur.RoleId equals r.Id
                        where u.UserId == userId && u.CustomerId == customerDbId
                        select r.Role1).FirstOrDefault();
            if (role == "Admin" || role == "SuperAdmin")
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "Cannot delete admin.",
                    Data = null
                });
            }

            _refreshtokensBL.DeleteUserRefreshTokensByUserId(user.Id);
            _userdeviceBL.DeleteUserDevicesByUserId(user.Id);
            _userroleBL.DeleteUserRoleByUserId(user.Id);
            _userBL.DeleteUser(user.Id);

            return true;
        }

        public IEnumerable<Role> GetRoles()
        {
            return _roleBL.GetRoles().OrderBy(x => x.Role1);
        }

        public IEnumerable<Role> GetRoleByDescription(string description)
        {
            return _roleBL.GetRoleByDescription(description).OrderBy(x => x.Role1);
        }

        public Role GetRoleByNameAndDescription(string roleName, string description)
        {
            return _roleBL.GetRoleByNameAndDescription(roleName, description);
        }

        public IEnumerable<Form> GetForms()
        {
            return _formBL.GetForms().OrderBy(x => x.FormName);
        }

        public Form GetForm(int Id)
        {
            return _formBL.GetForm(Id);
        }

        public Form InsertForm(Form form)
        {
            return _formBL.InsertForm(form);

        }

        public Form UpdateForm(Form form)
        {
            return _formBL.UpdateForm(form);

        }

        public bool DeleteForm(int formId)
        {
            return _formBL.DeleteForm(formId);

        }

        public bool DeleteForm(string formId)
        {
            var form = _formBL.QueryForm().Where(x => x.FormId == formId).FirstOrDefault(); ;
            return _formBL.DeleteForm(form.Id);

        }

        public List<PersonasRolePermissionsDTO> GetRolepermissions()
        {
            var rolePermission = _rolepermissionBL.QueryRolepermission().Include(x => x.Role);
            var personaRolePermissionList = new List<PersonasRolePermissionsDTO>();

            foreach (var obj in rolePermission)
            {
                var item = new PersonasRolePermissionsDTO();
                item.CanDelete = obj.CanDelete;
                item.CanExport = obj.CanExport;
                item.CanInsert = obj.CanInsert;
                item.CanUpdate = obj.CanUpdate;
                item.CanView   = obj.CanView;
                item.CreatedDate = obj.CreatedDate;
                item.FormId = obj.FormId;
                item.Id = obj.Id;
                item.RoleId = obj.RoleId;
                item.PersonaId = obj.Role.Description;
                item.PersonaName = obj.Role.Description;
                item.IsActive = obj.IsActive;
                personaRolePermissionList.Add(item);
            }
            return personaRolePermissionList;
        }

        public Rolepermission GetRolepermission(int Id)
        {
            return _rolepermissionBL.GetRolepermission(Id);
        }

        public PersonasRolePermissionsDTO InsertRolepermission(Rolepermission rolepermission)
        {
            var rolePermission = _rolepermissionBL.InsertRolepermission(rolepermission);
            var role = _roleBL.QueryRole().Where(x=>x.Id == rolePermission.RoleId).FirstOrDefault();
            var PersonasRolePermissionsDTO = new PersonasRolePermissionsDTO();
            PersonasRolePermissionsDTO.CanDelete = rolePermission.CanDelete;
            PersonasRolePermissionsDTO.CanExport = rolePermission.CanExport;
            PersonasRolePermissionsDTO.CanInsert = rolePermission.CanInsert;
            PersonasRolePermissionsDTO.CanUpdate = rolePermission.CanUpdate;
            PersonasRolePermissionsDTO.CanView =   rolePermission.CanView;
            PersonasRolePermissionsDTO.CreatedDate = rolePermission.CreatedDate;
            PersonasRolePermissionsDTO.FormId = rolePermission.FormId;
            PersonasRolePermissionsDTO.Id = rolePermission.Id;
            PersonasRolePermissionsDTO.RoleId = rolePermission.RoleId;
            PersonasRolePermissionsDTO.PersonaId = role.Description;
            PersonasRolePermissionsDTO.PersonaName = role.Description;
            PersonasRolePermissionsDTO.IsActive = rolePermission.IsActive;
            return PersonasRolePermissionsDTO;
        }

        public Rolepermission UpdateRolepermission(Rolepermission rolepermission)
        {
            return _rolepermissionBL.UpdateRolepermission(rolepermission);

        }

        public bool DeleteRolepermission(int rolepermissionId)
        {
            return _rolepermissionBL.DeleteRolepermission(rolepermissionId);

        }
        public List<Form> getFormList(int personaId)
        {
            var personaPermission = _personaPermissionBL.QueryPersonapermission().Where(x => x.PersonaId == personaId).ToList();
            var formList = new List<Form>();
            foreach (var personaPermissionObj in personaPermission)
            {
                formList.Add(_formBL.QueryForm().Where(x => x.Id == personaPermissionObj.FormId).FirstOrDefault());
            }
            return formList;
        }
        public UserPermissionManagement GetUserPermission(string customerId, string userId)
        {
            var userManagement = new UserPermissionManagement();
            userManagement.PersonaPermission = new List<personaFormPermission>();
            userManagement.RolePermission = new List<roleFormPermission>();
            var customer = Validator.ValidateCustomer(customerId);
            var user = Validator.ValidateUser(customerId, userId);
            var role = _userroleBL.QueryUserrole().Where(x => x.UserId == user.Id).FirstOrDefault();
            var persona = _personaBL.QueryPersona().Where(x => x.Id == customer.PersonaId).FirstOrDefault();
            var userRole = _roleBL.QueryRole().Where(x => x.Id == role.RoleId).FirstOrDefault();
            var formList = new List<Form>();
            if (persona != null)
            {
                userManagement.PersonaName = persona.PersonaName;
                formList = getFormList(persona.Id);
                foreach (var formListObj in formList)
                {
                    var personaFormPermission = new personaFormPermission();
                    personaFormPermission.FormName = formListObj.FormId;
                    userManagement.PersonaPermission.Add(personaFormPermission);
                }

            }
            if (userRole != null)
            {
                userManagement.RoleName = userRole.Role1;
                foreach (var formObj in formList)
                {
                    var rolePermission = _rolepermissionBL.QueryRolepermission().Where(x => x.RoleId == role.RoleId && x.FormId == formObj.Id).FirstOrDefault();
                    var formPermission = new roleFormPermission();
                    formPermission.FormId = formObj.FormId;
                    formPermission.FormName = formObj.FormName;
                    if (rolePermission != null)
                    {
                        formPermission.CanDelete = rolePermission.CanDelete;
                        formPermission.CanInsert = rolePermission.CanInsert;
                        formPermission.CanView = rolePermission.CanView;
                        formPermission.CanUpdate = rolePermission.CanUpdate;
                        formPermission.CanExport = rolePermission.CanExport;
                    }
                    userManagement.RolePermission.Add(formPermission);
                }

            }

            return userManagement;



        }

        public IEnumerable<User> GetUser(string customerId)
        {
            if (customerId == null)
            {
                return _userBL.QueryUsers().OrderByDescending(x => x.CreatedDate).ToList();
            }
            else
            {
                var customer = Validator.ValidateCustomer(customerId);
                return _userBL.QueryUsers().Where(x => x.CustomerId == customer.Id).OrderByDescending(x => x.CreatedDate).ToList();
            }
        }

        string GetUserPassword(int id)
        {
            var password = _userBL.GetUser(id).Password;
            return password;

        }

        public User UpdateUser(User user)
        {
            //user.Password = GetUserPassword(user.Id);
            user.ModifiedDate = DateTime.UtcNow;
            return _userBL.UpdateUser(user);
        }

        public User UpdateUserProfile(User user)
        {
            var userProfile = _userBL.QueryUsers().Where(x => x.UserId == user.UserId).FirstOrDefault();
            if (!string.IsNullOrEmpty(user.FirstName))
                userProfile.FirstName = user.FirstName;
            if (!string.IsNullOrEmpty(user.LastName))
                userProfile.LastName = user.LastName;
            if (!string.IsNullOrEmpty(user.Username))
                userProfile.Username = user.Username;
            if (!string.IsNullOrEmpty(user.EmailAddress))
                userProfile.EmailAddress = user.EmailAddress;
            if (!string.IsNullOrEmpty(user.ProfilePicture))
                userProfile.ProfilePicture = user.ProfilePicture;
            userProfile.ModifiedDate = DateTime.UtcNow;
            return _userBL.UpdateUser(userProfile);
        }

        public User UpdateUserPassword(User user)
        {
            var oldUserProfile = _userBL.QueryUsers().Where(x => x.Id == user.Id).FirstOrDefault();
            oldUserProfile.Password = user.Password.Encrypt();
            oldUserProfile.ModifiedDate = DateTime.UtcNow;
            return _userBL.UpdateUser(oldUserProfile);


        }
    }
}
