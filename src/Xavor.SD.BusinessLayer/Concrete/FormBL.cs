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
    public class FormBL : IFormBL
    {
        private readonly IUnitOfWork _uow;
        private SmartFanDbContext context;
        private IRepository<Form> repo;
        public FormBL()
        {
            context = new SmartFanDbContext();
            _uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = _uow.GetRepository<Form>();
        }

        public IEnumerable<Form> GetForms()
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

        public bool DeleteForm(int formId)
        {
            try
            {
                repo.Delete(formId);
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Form GetForm(int Id)
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

        

        public IQueryable<Form> QueryForm()
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

        public Form InsertForm(Form form)
        {
            try
            {
                repo.Add(form);
                _uow.SaveChanges();

                return form;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Form UpdateForm(Form form)
        {
            try
            {
                repo.Update(form);
                _uow.SaveChanges();

                return form;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
