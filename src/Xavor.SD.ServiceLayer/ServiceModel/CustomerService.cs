using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.BusinessLayer;
using Xavor.SD.Model;
using Xavor.SD.ServiceLayer.Validations;
using Xavor.SD.Common.Utilities;
using System.Linq;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.ServiceLayer.Transformations;

namespace Xavor.SD.ServiceLayer
{
    public class CustomerService : ICustomerService
    {
        private ICustomerBL _customerBL;
        private readonly IConfigurationsBL _configurationsBL;
        private readonly IUserBL _userBL;
        private readonly IUserroleBL _userroleBL;
        private readonly IRoleBL _roleBL;
        private readonly IDeviceBL _deviceBL;
        private readonly IUserdeviceBL _userdeviceBL;
        private readonly IDefaultSettingsBL _defaultSettingsBL;
        private readonly IDevicestatusBL _devicestatusBL;
        private readonly ITransformations _transformations;
        private readonly IDeviceBatchNumberBL _deviceBatchNumberBL;
        private readonly IBomBL _BomBL;
        private readonly IFirmwareBL _firmwareBL;
        private readonly IPersonaBL _personaBL;
        private readonly IPersonapermissionBL _personapermissionBL;
        private readonly IRuleEngineBL _ruleEngineBL;
        private readonly string xavor = "XAVOR"; //using this for superadmin Id

        public CustomerService(ICustomerBL customerBL, IConfigurationsBL configurationsBL, IUserBL userBL, IUserroleBL userroleBL, IRoleBL roleBL, IDeviceBL deviceBL, IUserdeviceBL userdeviceBL, IDefaultSettingsBL defaultSettingsBL, IDevicestatusBL devicestatusBL,
            ITransformations transformations, IDeviceBatchNumberBL deviceBatchNumberBL, IBomBL bomBL, IFirmwareBL firmwareBL, IPersonaBL personaBL, IPersonapermissionBL personapermissionBL, IRuleEngineBL ruleEngineBL)
        {
            _customerBL = customerBL;
            _configurationsBL = configurationsBL;
            _userBL = userBL;
            _userroleBL = userroleBL;
            _roleBL = roleBL;
            _deviceBL = deviceBL;
            _userdeviceBL = userdeviceBL;
            _defaultSettingsBL = defaultSettingsBL;
            _devicestatusBL = devicestatusBL;
            _transformations = transformations;
            _deviceBatchNumberBL = deviceBatchNumberBL;
            _BomBL = bomBL;
            _firmwareBL = firmwareBL;
            _personaBL = personaBL;
            _personapermissionBL = personapermissionBL;
            _ruleEngineBL = ruleEngineBL;
        }

        public CustomerService(ICustomerBL customerBL, IConfigurationsBL configurationsBL)
        {
            _customerBL = customerBL;
            _configurationsBL = configurationsBL;
        }

        public IEnumerable<ConfigurationsDTO> GetConfigurationsByCustomer(string customerId)
        {
            int actualCustomerId = Validator.ValidateCustomer(customerId).Id;
            var response = _configurationsBL.GetConfigurationsByCustomer(actualCustomerId);
            return response;
        }

        public Configurations GetConfigurationsByName(int customerId, string configKey)
        {
            //int actualCustomerId = Validator.ValidateCustomer(customerId);
            return _configurationsBL.GetConfiguration(customerId, configKey);
        }

        public Customer GetCustomerById(int customerId)
        {
            return _customerBL.GetCustomer(customerId);
        }

        public User CreateCustomer(string userId, string customerId, string customerType, string address, string firstName, string lastName, string adminEmail)
        {

            if (_customerBL.GetCustomerByUniqueId(customerId) != null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Customer [{customerId}] already exists",
                    Data = null
                });
            }
            var parentUser = _userBL.GetUserByUserId(userId);
            if (parentUser == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Parent User [{userId}] does not exist.",
                    Data = null
                });
            }

            var parentCustomer = _customerBL.GetCustomer(Convert.ToInt32(parentUser.CustomerId));
            if (parentCustomer == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Parent Customer [{userId}] does not exist.",
                    Data = null
                });
            }

            var persona = _personaBL.GetPersonaByName(customerType);
            if (persona == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Customer type [{customerType}] does not exist.",
                    Data = null
                });
            }

            if (_userBL.GetOperatorByUniqueId(adminEmail) != null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"User [{adminEmail}] already exists. Try again with a different email.",
                    Data = null
                });
            }

            Customer customer = new Customer();
            customer.Name = customerId;
            customer.CustomerId = customerId.Replace(" ", "_");
            customer.ParentId = parentUser.CustomerId;
            customer.PersonaId = persona.Id;
            customer.CustomerType = customerType;
            customer.Address = address;
            customer.CreatedBy = parentCustomer.CustomerId;
            customer.CreatedDate = DateTime.UtcNow;
            customer.IsActive = 1;
            customer.IsDeleted = 0;

            var createdCustomer = _customerBL.InsertCustomer(customer);
            InsertConfigurationsOfCustomer(createdCustomer.Id);
            var createdAdmin = CreateAdmin(createdCustomer, parentUser, firstName, lastName, adminEmail);

            return createdAdmin;
        }
        public User CreateCustomerUser(string userId, string customerId, string roleType, string firstName, string lastName, string emailaddress)
        {

            var customer = _customerBL.GetCustomerByUniqueId(customerId);
            if (customer == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Customer [{customerId}] does not exist.",
                    Data = null
                });
            }

            var parentUser = _userBL.GetUserByUserId(userId);
            if (parentUser == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Parent User [{userId}] does not exist.",
                    Data = null
                });
            }

            var role = _roleBL.GetRoleByNameAndDescription(roleType, customer.CustomerType);
            if (role == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Role type [{roleType}] does not exist.",
                    Data = null
                });
            }

            if (_userBL.GetOperatorByUniqueId(emailaddress) != null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"User [{emailaddress}] already exists. Try again with a different email.",
                    Data = null
                });
            }

            var createdUser = CreateCustomerUser(customer, parentUser, roleType, firstName, lastName, emailaddress);

            return createdUser;
        }
        public User CreateAdmin(Customer customer, User parentUser, string firstName, string lastName, string adminEmail)
        {
            User user = new User();
            user.CustomerId = customer.Id;
            user.ParentId = parentUser.Id;
            user.UserId = adminEmail;
            user.Username = adminEmail;
            user.EmailAddress = adminEmail;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.CreatedBy = parentUser.UserId;
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = 1;
            user.IsDeleted = 0;
            user.Password = GetRandomPassword(7).Encrypt();

            var createdAdmin = _userBL.InsertUser(user);
            if (createdAdmin != null)
            {
                Userrole userrole = new Userrole();
                userrole.UserId = createdAdmin.Id;
                userrole.RoleId = _roleBL.QueryRole().Where(x => x.Role1 == "Admin" && x.Description == customer.CustomerType).Select(s => s.Id).FirstOrDefault();
                _userroleBL.InsertUserrole(userrole);
            }

            return createdAdmin;

        }

        public User CreateCustomerUser(Customer customer, User parentUser, string roleType, string firstName, string lastName, string emailaddress)
        {
            User user = new User();
            user.CustomerId = customer.Id;
            user.ParentId = parentUser.Id;
            user.UserId = emailaddress;
            user.Username = emailaddress;
            user.EmailAddress = emailaddress;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.CreatedBy = parentUser.UserId;
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = 1;
            user.IsDeleted = 0;
            user.Password = GetRandomPassword(7).Encrypt();

            var createdUser = _userBL.InsertUser(user);
            if (createdUser != null)
            {
                Userrole userrole = new Userrole();
                userrole.UserId = createdUser.Id;
                userrole.RoleId = _roleBL.QueryRole().Where(x => x.Role1 == roleType && x.Description == customer.CustomerType).Select(s => s.Id).FirstOrDefault();
                _userroleBL.InsertUserrole(userrole);
            }

            return createdUser;

        }

        private void InsertConfigurationsOfCustomer(int customerId)
        {
            var actualSuperadmin = _customerBL.GetCustomerByUniqueId(xavor);
            var SA_Configurations = _configurationsBL.GetConfigurations().Where(x => x.CustomerId == actualSuperadmin.Id).ToList();

            foreach (var exConf in SA_Configurations)
            {
                var newConfiguration = new Configurations();

                newConfiguration.Value = exConf.Value;
                newConfiguration.Name = exConf.Name;
                newConfiguration.IsActive = exConf.IsActive;
                newConfiguration.IsDeleted = exConf.IsDeleted;
                newConfiguration.IsMobileConfiguration = exConf.IsMobileConfiguration;
                newConfiguration.CustomerId = customerId;

                _configurationsBL.InsertConfigurations(newConfiguration);
            }
        }

        private string GetRandomPassword(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool CreateDevices(string idPrefix, int startRange, int endRange, string customerId, string batchId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                customerId = xavor;
            }


            var customer = _customerBL.GetCustomerByUniqueId(customerId);
            if (customer == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Customer [{customerId}] does not exist.",
                    Data = null
                });
            }
            var admin = GetAdminByCustomer(customer);

            var batch = _deviceBatchNumberBL.QueryDeviceBatchNumber().Where(x => x.BatchId == batchId).FirstOrDefault();
            if (batch == null)
            {
                var newBatch = new Devicebatchnumber();
                newBatch.BatchId = batchId;
                newBatch.BatchName = batchId;
                _deviceBatchNumberBL.InsertDeviceBatchNumber(newBatch);
            }

            int totalDevices = endRange - startRange + 1;

            for (int currentRange = startRange; currentRange <= endRange; currentRange++)
            {
                var deviceUniqueId = idPrefix + currentRange.ToString();
                var device = _deviceBL.GetDeviceByUniqueId(deviceUniqueId);
                if (device != null) // device already exists
                {
                    var currentCustomerOfDevice = _customerBL.GetCustomer(device.CustomerId.Value);
                    if (currentCustomerOfDevice.CustomerId == xavor && customer.CustomerId != xavor) //current customer of device is xavor(default) and new customer is not null
                    {

                        device.CustomerId = customer.Id; //update customer of a device
                        _deviceBL.UpdateDevice(device);
                        Userdevice userdevice = new Userdevice();
                        userdevice.UserId = admin.Id;
                        userdevice.DeviceId = device.Id;


                        _userdeviceBL.InsertUserDevice(userdevice); // assigning device to admin of new customer
                    }
                    else if (currentCustomerOfDevice.CustomerId != xavor) // device belongs to another customer
                    {
                        continue;
                    }
                }
                else //create device
                {
                    CreateNewDevice(customer, admin, deviceUniqueId, batchId);
                }
            }

            return true;
        }
        private Device CreateNewDevice(Customer customer, User user, string deviceUniqueId, string batchId)
        {
            Device newDevice = new Device();
            newDevice.Name = deviceUniqueId;
            newDevice.DeviceId = deviceUniqueId;
            newDevice.DeviceCode = deviceUniqueId;
            newDevice.CreatedBy = customer.CustomerId;
            newDevice.CreatedDate = DateTime.UtcNow;
            newDevice.IsActive = 1;
            newDevice.IsDeleted = 0;
            newDevice.CustomerId = customer.Id;
            newDevice.IsInstalled = 0;
            newDevice.Apssid = deviceUniqueId;
            newDevice.BatchId = batchId;
            //assigning the Default Firmware Version
            newDevice.CurrentFirmwareVersion = "V0";

            //APPassword
            //getting super admin's db id to get the SU_APPassward value            
            int superAdminsDbId = _customerBL.GetCustomerByUniqueId(xavor).Id;
            newDevice.Appassword = GetConfigurationsByName(superAdminsDbId, "SU_APPassword").Value;

            var createdDevice = _deviceBL.InsertDevice(newDevice); // creating device
            Userdevice userdevice = new Userdevice();
            userdevice.UserId = user.Id;
            userdevice.DeviceId = createdDevice.Id;
            _userdeviceBL.InsertUserDevice(userdevice); // assigning device to admin
            InsertDefaultSettings(createdDevice.DeviceId);
            return createdDevice;

        }
        private void InsertDefaultSettings(string deviceUniqueId)
        {
            var newDeviceStatus = _transformations.TransformDefaultSettingsToDeviceStatus(deviceUniqueId);

            _devicestatusBL.InsertDevicestatus(newDeviceStatus);


        }
        private User GetAdminByCustomer(Customer customer)
        {

            //need to get super admin in case no customer is specified in this call or customer.CustomerId == xavor
            string adminRole = "Admin";
            if (customer.CustomerId == xavor)
            {
                adminRole = "SuperAdmin";
            }

            var user = (from c in _customerBL.QueryCustomer()
                        join u in _userBL.QueryUsers() on c.Id equals u.CustomerId
                        join ur in _userroleBL.QueryUserrole() on u.Id equals ur.UserId
                        join r in _roleBL.QueryRole() on ur.RoleId equals r.Id
                        where c.Id == customer.Id && r.Role1 == adminRole
                        select u).FirstOrDefault();

            if (user == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = $"Admin of customer: [{customer.CustomerId}] does not exist",
                    Data = null
                });
            }
            return user;
        }

        public List<string> GetAllCustomerIds()
        {
            var customerIds = _customerBL.QueryCustomer().Where(x => x.CustomerId != xavor).Select(x => x.CustomerId).ToList();
            return customerIds;
        }

        public List<Customer> GetAllCustomers()
        {
            var customers = _customerBL.QueryCustomer().Where(x => x.CustomerId != xavor).OrderByDescending(x => x.CreatedDate).ToList();
            return customers;
        }

        public List<Persona> GetPersonas()
        {
            var customers = _personaBL.QueryPersona().OrderBy(x => x.PersonaName).ToList();
            return customers;
        }

        public Customer updateCustomer(int Id, string customerName, string address)
        {
            var currentCustomer = _customerBL.QueryCustomer().Where(x => x.Id == Id).FirstOrDefault();
            if (currentCustomer == null)
            {
                throw null;
            }
            if (!string.IsNullOrEmpty(customerName))
            {
                currentCustomer.Name = customerName;
            }
            if (!string.IsNullOrEmpty(address))
            {
                currentCustomer.Address = address;
            }

            currentCustomer.ModifiedDate = DateTime.UtcNow;
            return _customerBL.UpdateCustomer(currentCustomer);
        }

        public Bom uploadBom(Bom bom)
        {
            var bomResult = _BomBL.QueryBom().Where(x => x.BatchId == bom.BatchId && x.CustomerId == bom.CustomerId).FirstOrDefault();
            if (bomResult == null)
            {
                return _BomBL.InsertBom(bom);
            }
            else
            {
                bomResult.BomData = bom.BomData;
                bomResult.BomType = bom.BomType;
                bomResult.FileFormat = bom.FileFormat;
                bomResult.ModifiedDate = DateTime.Now;
                return _BomBL.UpdateBom(bomResult);
            }

        }



        public Firmware uploadfirmware(Firmware firmware)
        {
            var firmwareResult = _firmwareBL.QueryFirmware().Where(x => x.BatchId == firmware.BatchId && x.CustomerId == firmware.CustomerId).FirstOrDefault();
            if (firmwareResult == null)
            {

                return _firmwareBL.InsertFirmware(firmware);

            }
            else
            {
                firmwareResult.FirmwareData = firmware.FirmwareData;
                firmwareResult.FirmwareVersion = firmware.FirmwareVersion;
                firmwareResult.FileFormat = firmware.FileFormat;
                firmwareResult.ModifiedDate = DateTime.Now;
                return _firmwareBL.UpdateFirmware(firmwareResult);
            }


        }

        public string getFirmwareUrl(string customerId, string FirmwareVersion)
        {
            var firmware = _firmwareBL.QueryFirmware().Where(x => x.CustomerId == customerId && x.FirmwareVersion == x.FirmwareVersion).OrderByDescending(o => o.ModifiedDate).FirstOrDefault();

            var Url = firmware.FirmwareData;

            return Url;
        }

        public IEnumerable<Personapermission> GetPersonapermissions()
        {
            return _personapermissionBL.GetPersonapermissions();
        }

        public Personapermission GetPersonapermission(int Id)
        {
            return _personapermissionBL.GetPersonapermission(Id);
        }

        public Personapermission InsertPersonapermission(Personapermission personapermission)
        {
            return _personapermissionBL.InsertPersonapermission(personapermission);

        }

        public Personapermission UpdatePersonapermission(Personapermission personapermission)
        {
            return _personapermissionBL.UpdatePersonapermission(personapermission);

        }

        public bool DeletePersonapermission(int personapermissionId)
        {
            return _personapermissionBL.DeletePersonapermission(personapermissionId);

        }

        public Ruleengine GetRuleEngineByCustomerId(int customerId)
        {
            return _ruleEngineBL.GetRuleEngineByCustomerId(customerId);
        }
    }
}
