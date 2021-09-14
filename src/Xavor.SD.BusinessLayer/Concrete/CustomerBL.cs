using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;
using Xavor.SD.BusinessLayer;

namespace Xavor.SD.BusinessLayer
{
    public class CustomerBL : ICustomerBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Customer> repo;
        public CustomerBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Customer>();
        }
        public bool DeleteCustomer(int customerId)
        {
            try
            {
                repo.Delete(customerId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Customer> GetCustomer()
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

        public Customer GetCustomer(int Id)
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

        public Customer GetCustomerByUniqueId(string customerId)
        {
            var customer = GetCustomer().Where(x => x.CustomerId == customerId).FirstOrDefault();
            return customer;
        }

        public Customer InsertCustomer(Customer customer)
        {
            try
            {
                repo.Add(customer);
                uow.SaveChanges();

                return customer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Customer> QueryCustomer()
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

        public Customer UpdateCustomer(Customer customer)
        {
            try
            {
                repo.Update(customer);
                uow.SaveChanges();
                return customer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
