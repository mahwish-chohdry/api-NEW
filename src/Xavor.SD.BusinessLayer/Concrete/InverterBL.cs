using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;

namespace Xavor.SD.BusinessLayer.Concrete
{
    public class InverterBL : IInverterBL
    {
        private readonly IUnitOfWork _uow;
        private SmartFanDbContext context;
        private IRepository<Inverter> repo;
        public InverterBL()
        {
            context = new SmartFanDbContext();
            _uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = _uow.GetRepository<Inverter>();
        }

        public bool DeleteInverter(int inverterId)
        {
            try
            {
                repo.Delete(inverterId);
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<Inverter> GetInverter()
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

        public Inverter GetInverter(int Id)
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

        public Inverter InsertInverter(Inverter inverter)
        {
            try
            {
                repo.Add(inverter);
                _uow.SaveChanges();

                return inverter;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Inverter> QueryInverter()
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

        public Inverter UpdateInverter(Inverter inverter)
        {
            try
            {
                repo.Update(inverter);
                _uow.SaveChanges();

                return inverter;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
