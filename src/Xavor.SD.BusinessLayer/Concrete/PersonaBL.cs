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
    public class PersonaBL : IPersonaBL
    {
        private readonly IUnitOfWork _uow;
        private SmartFanDbContext context;
        private IRepository<Persona> repo;
        public PersonaBL()
        {
            context = new SmartFanDbContext();
            _uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = _uow.GetRepository<Persona>();
        }

        public IEnumerable<Persona> GetPersonas()
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

        public Persona GetPersonaByName(string personaName)
        {
            var persona = GetPersonas().Where(x => x.PersonaName == personaName).FirstOrDefault();
            return persona;
        }

        public bool DeletePersona(int personaId)
        {
            try
            {
                repo.Delete(personaId);
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Persona GetPersona(int Id)
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

        

        public IQueryable<Persona> QueryPersona()
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

        public Persona InsertPersona(Persona persona)
        {
            try
            {
                repo.Add(persona);
                _uow.SaveChanges();

                return persona;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Persona UpdatePersona(Persona persona)
        {
            try
            {
                repo.Update(persona);
                _uow.SaveChanges();

                return persona;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
