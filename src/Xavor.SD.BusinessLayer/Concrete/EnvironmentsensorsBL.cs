using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;

namespace Xavor.SD.BusinessLayer.Concrete
{
    public class EnvironmentsensorsBL : IEnvironmentsensorsBL
    {

        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Environmentsensors> repo;
        public EnvironmentsensorsBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Environmentsensors>();
        }

        public IEnumerable<Environmentsensors> GetEnvironmentsensors()
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
    }
}
