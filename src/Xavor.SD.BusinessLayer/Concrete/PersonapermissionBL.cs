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
    public class PersonapermissionBL : IPersonapermissionBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Personapermission> repo;

        public PersonapermissionBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Personapermission>();
        }

        public bool DeletePersonapermission(int personapermissionId)
        {
            try
            {
                repo.Delete(personapermissionId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Personapermission GetPersonapermission(int Id)
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

        public IEnumerable<Personapermission> GetPersonapermissions()
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

        public IQueryable<Personapermission> QueryPersonapermission()
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

        public Personapermission InsertPersonapermission(Personapermission personapermission)
        {
            try
            {
                repo.Add(personapermission);
                uow.SaveChanges();

                return personapermission;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Personapermission UpdatePersonapermission(Personapermission personapermission)
        {
            try
            {
                repo.Update(personapermission);
                uow.SaveChanges();

                return personapermission;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
