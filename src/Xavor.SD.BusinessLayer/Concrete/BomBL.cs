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
    public class BomBL : IBomBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Bom> repo;

        public BomBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Bom>();
        }
        public bool DeleteBom(int bomId)
        {
            try
            {
                repo.Delete(bomId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Bom> GetBom()
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

        public Bom GetBom(int Id)
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

        public Bom InsertBom(Bom bom)
        {
            try
            {
                repo.Add(bom);
                uow.SaveChanges();

                return bom;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Bom> QueryBom()
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

        public Bom UpdateBom(Bom bom)
        {
            try
            {
                repo.Update(bom);
                uow.SaveChanges();
                return bom;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
