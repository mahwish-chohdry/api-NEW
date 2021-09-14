using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer
{
    public interface ICustomerService
    {
        IEnumerable<ConfigurationsDTO> GetConfigurationsByCustomer(string customerId);
        Configurations GetConfigurationsByName(int customerId, string configKey);
        Customer GetCustomerById(int customerId);
        User CreateCustomer(string userId, string customerId, string customerType, string address, string firstName, string lastName, string adminEmail);
        User CreateCustomerUser(string userId, string customerId, string roleType, string firstName, string lastName, string emailaddress);
        bool CreateDevices(string idPrefix,int startRange, int endRange, string customerId,string batchId);

        Customer updateCustomer(int Id, string customerName, string address);
        List<string> GetAllCustomerIds();
        List<Customer> GetAllCustomers();
        List<Persona> GetPersonas();
        Bom uploadBom(Bom bom);

        Firmware uploadfirmware(Firmware firmware);

        string getFirmwareUrl(string customerId, string FirmwareVersion);
        IEnumerable<Personapermission> GetPersonapermissions();
        Personapermission GetPersonapermission(int Id);
        Personapermission InsertPersonapermission(Personapermission personapermission);
        Personapermission UpdatePersonapermission(Personapermission personapermission);
        bool DeletePersonapermission(int personapermissionId);
        Ruleengine GetRuleEngineByCustomerId(int customerId);
    }
}
