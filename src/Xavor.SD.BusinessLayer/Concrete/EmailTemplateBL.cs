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
    public class EmailTemplateBL : IEmailTemplateBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<EmailTemplate> repo;
        public EmailTemplateBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<EmailTemplate>();
        }
        
        public IEnumerable<EmailTemplate> GetEmailTemplate()
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

        public EmailTemplate GetEmailTemplateByCode(string code, string lang)
        {
            try
            {
                return repo.Queryable().Where(x => x.Code == code && x.Language == lang).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmailTemplate GetEmailTemplate(int Id)
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

        public IQueryable<EmailTemplate> QueryEmailTemplate()
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
    }
}
