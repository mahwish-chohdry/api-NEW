using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;

namespace Xavor.SD.BusinessLayer
{
    public class ConfigurationsBL : IConfigurationsBL
    {
        private readonly IUnitOfWork uow;
        private readonly ICustomerBL _customerBL;
        private SmartFanDbContext context;
        private IRepository<Configurations> repo;
        public ConfigurationsBL(ICustomerBL customerBL)
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Configurations>();
            _customerBL = customerBL;
        }
        public bool DeleteConfigurations(int ConfigurationsId)
        {
            try
            {
                repo.Delete(ConfigurationsId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Configurations> GetConfigurations()
        {
            try
            {
                return repo.GetList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Configurations GetConfiguration(string configurationKey)
        {
            try
            {
                return repo.GetList().Where(x => x.Name.Equals(configurationKey) && x.IsDeleted == 0).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Configurations GetConfiguration(int customerId, string configurationKey)
        {
            try
            {
                return repo.GetList().Where(x => x.Name.Equals(configurationKey) && x.IsDeleted == 0 && x.CustomerId == customerId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Configurations GetConfigurations(int Id)
        {
            try
            {
                if (Id <= default(int))
                    throw new ArgumentException("Invalid id");
                return repo.Find(Id);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Configurations InsertConfigurations(Configurations Configurations)
        {
            try
            {
                repo.Add(Configurations);
                uow.SaveChanges();

                return Configurations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Configurations> QueryConfigurations()
        {
            try
            {
                return repo.Queryable();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Configurations UpdateConfigurations(Configurations Configurations)
        {
            try
            {
                repo.Update(Configurations);
                uow.SaveChanges();
                return Configurations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<ConfigurationsDTO> GetConfigurationsByCustomer(int actualCustomerId)
        {
          
            var configurations = QueryConfigurations().Where(x => x.CustomerId == actualCustomerId).ToList();
            List<ConfigurationsDTO> list = new List<ConfigurationsDTO>();

            foreach (var obj in configurations)
            {
                ConfigurationsDTO dto = new ConfigurationsDTO();
                dto.name = obj.Name;
                dto.value = obj.Value;
                if (obj.Name.Equals("CurrentTime"))
                {
                    dto.value = DateTime.UtcNow.ToString();
                }

                list.Add(dto);
            }
            return list;
        }

        public IEnumerable<ConfigurationsDTO> GetMobileConfigurationsByCustomer(string customerId)
        {
            var customer = _customerBL.QueryCustomer().Where(x => x.CustomerId == customerId).FirstOrDefault();
            if (customer == null)
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "Customer is not Valid ",
                    Data = null
                });
            }
            int actualCustomerId = customer.Id;
            var configurations = QueryConfigurations().Where(x => x.CustomerId == actualCustomerId && x.IsMobileConfiguration==1).ToList();
            List<ConfigurationsDTO> list = new List<ConfigurationsDTO>();

            foreach (var obj in configurations)
            {
                ConfigurationsDTO dto = new ConfigurationsDTO();
                dto.name = obj.Name;
                dto.value = obj.Value;

                list.Add(dto);
            }
            return list;
        }

        public string GetSuperAdminEmails(int actualCustomerId)
        {
            var configurations = GetConfigurationsByCustomer(actualCustomerId);
            var emailRecipients = configurations.Where(x => x.name == "SuperAdmin").Select(x => x.value).FirstOrDefault();
            if (string.IsNullOrEmpty(emailRecipients))
            {
                throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "Super Admin Emails Not Found",
                    Data = null
                });
            }
            return emailRecipients;
        }
    }
}
