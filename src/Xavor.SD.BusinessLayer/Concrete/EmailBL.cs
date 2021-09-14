using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;

namespace Xavor.SD.BusinessLayer
{
    public class EmailBL : IEmailBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Email> repo;
        public EmailBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Email>();
        }
        public bool DeleteEmail(int EmailId)
        {
            try
            {
                repo.Delete(EmailId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Email> GetEmail()
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

        public Email GetEmail(int Id)
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

        public Email InsertEmail(Email Email)
        {
            try
            {
                repo.Add(Email);
                uow.SaveChanges();

                return Email;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Email> QueryEmail()
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

        public Email UpdateEmail(Email Email)
        {
            try
            {
                repo.Update(Email);
                uow.SaveChanges();
                return Email;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
