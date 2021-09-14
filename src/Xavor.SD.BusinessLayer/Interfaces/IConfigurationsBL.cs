using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IConfigurationsBL
    {
        Configurations InsertConfigurations(Configurations Configurations);
        Configurations UpdateConfigurations(Configurations Configurations);
        IEnumerable<Configurations> GetConfigurations();
        bool DeleteConfigurations(int ConfigurationsId);
        Configurations GetConfigurations(int Id);
        Configurations GetConfiguration(int customerId, string configurationKey);
        Configurations GetConfiguration(string configurationKey);
        IQueryable<Configurations> QueryConfigurations();
        IEnumerable<ConfigurationsDTO> GetConfigurationsByCustomer(int customerId);
        string GetSuperAdminEmails(int actualCustomerId);
    }
}
