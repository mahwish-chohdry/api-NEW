using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface ICustomerBL
    {
        Customer InsertCustomer(Customer customer);
        Customer UpdateCustomer(Customer customer);
        IEnumerable<Customer> GetCustomer();
        bool DeleteCustomer(int customerId);
        Customer GetCustomer(int Id);
        IQueryable<Customer> QueryCustomer();
        Customer GetCustomerByUniqueId(string customerId);
    }
}
