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
   public class EnvironmentstandardsBL : IEnvironmentstandardsBL
    {

        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Environmentstandards> repo;
        public EnvironmentstandardsBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Environmentstandards>();
        }

        public IEnumerable<Environmentstandards> GetEnvironmentstandards()
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
