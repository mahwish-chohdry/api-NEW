using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.ServiceLayer;
using Xavor.SD.WebAPI.Controllers;
using Xunit;

namespace Xavor.SD.Test
{
    public class CustomerControllerTest
    {
       
        ICustomerService _customerService;
        ICustomerBL _customerBL;
        IConfigurationsBL _configurationsBL;
        public CustomerControllerTest()
        {
            _customerBL = new CustomerBL();
            _configurationsBL = new ConfigurationsBL(_customerBL);
            _customerService = new CustomerService(_customerBL, _configurationsBL);
        }

        [Fact]
        public void GetConfiguration_ExistingIdPassed_ReturnsNotNull()
        {
            var abc = _customerService.GetConfigurationsByCustomer("xavor1");
            Assert.NotNull(abc);
        }
    }
}
